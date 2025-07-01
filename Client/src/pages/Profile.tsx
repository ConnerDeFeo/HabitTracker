import { useNavigate } from "react-router-dom";
import Button from "../components/General/Button";
import Container from "../components/General/Container";
import UserService from "../services/UserService";
import UserDto from "../types/UserDto";
import { ChangeEvent, useEffect, useRef, useState } from "react";
import PhotoService from "../services/PhotoService";
import Modal from "../components/General/Modal";
import ImageCropper from "../components/Profile/ImageCropper";
import { convertToPixelCrop, Crop, PixelCrop } from "react-image-crop";
import SetCrop from "../components/Profile/SetCrop";

//Profile page the user sees
const Profile =(props:{user: UserDto, setUser: (user:UserDto)=>void})=>{
    const {user,setUser} = props;
    const navigate = useNavigate();

    const fileInputRef = useRef<HTMLInputElement | null>(null);
    const [imageUrl, setImageUrl] = useState<string>("");
    const [modalOpen, setModalOpen] = useState<boolean>(false);
    const [crop, setCrop] = useState<Crop>({
        unit: '%', // Can be 'px' or '%'
        x: 200,
        y: 100,
        width: 25,
        height: 25,
    })
    const modifiedImgRef = useRef<HTMLCanvasElement>(null);
    const imgRef = useRef<HTMLImageElement>(null);

    //On load, fetch profile photo
    useEffect(()=>{
        const fetchImage = async () => {
            const response = await PhotoService.getProfilePhoto();
            if (response.status==200) {
                const data = await response.text();
                setImageUrl(data);
            }
        };
        fetchImage();
    },[]);

    //Upload file to the backend
    const handleFileChange = async (e: ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if(file){
            const resp = await PhotoService.uploadProfilePhoto(file);
            if(resp.status==200){
                const url = await resp.text();
                setImageUrl(url);
            }
        }
    };

    const logout = async ()=>{
        await UserService.Logout();
        sessionStorage.setItem("loggedIn","false");
        setUser({username:"", dateCreated: "" });
        navigate('/');
    }

    console.log("crop", crop);
    console.log("converted", crop as PixelCrop);

    return(
        <Container content={
            <div className="grid mx-auto text-4xl md:text-6xl gap-5">
                <div 
                    className="h-40 w-40 mx-auto border-3 flex justify-center items-center relative rounded-full cursor-pointer"
                    onClick={()=>setModalOpen(true)}
                >
                    <input type="file" accept="image/*" className="hidden" ref={fileInputRef} onChange={(e)=>handleFileChange(e)}/>
                    {imageUrl ? 
                        <img src={imageUrl} alt="Profile pic" className="h-full w-full object-cover rounded-full" ref={imgRef}/>
                        :
                        <img src={"/UploadImage.png"} alt="uplaod image" className="h-10 w-10 cursor-pointer" onClick={()=>fileInputRef.current?.click()}/>
                    }
                </div>
                <p>Username: {user.username}</p>
                <p>Date Joined: {user.dateCreated}</p>
                <Button label="Logout" onClick={logout} className="w-30 ml-auto"/>
                {modalOpen && 
                    <Modal content={
                        <>
                            <ImageCropper imageSrc={imageUrl} crop={crop} setCrop={setCrop}/>
                            <Button label="Confirm" className="w-30" onClick={
                                ()=>{
                                    if(imgRef.current && modifiedImgRef.current)
                                        SetCrop(
                                            imgRef.current,
                                            modifiedImgRef.current,
                                            convertToPixelCrop(crop,imgRef.current.width,imgRef.current.height)
                                        )
                                }}/>
                            {crop && <canvas ref={modifiedImgRef} className="border w-[150px] h-[150px] object-contain"/>}
                        </>
                    } onClose={()=>setModalOpen(false)}/>}
            </div>
        }/>
    );
}

export default Profile;