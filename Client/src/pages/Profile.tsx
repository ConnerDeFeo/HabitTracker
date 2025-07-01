import { useNavigate } from "react-router-dom";
import Button from "../components/General/Button";
import Container from "../components/General/Container";
import UserService from "../services/UserService";
import UserDto from "../types/UserDto";
import { ChangeEvent, useEffect, useRef, useState } from "react";
import PhotoService from "../services/PhotoService";
import AddProfilePic from "../components/Profile/AddProfilePic";

//Profile page the user sees
const Profile =(props:{user: UserDto, setUser: (user:UserDto)=>void})=>{
    const {user,setUser} = props;
    const navigate = useNavigate();

    const fileInputRef = useRef<HTMLInputElement | null>(null);
    const [imageUrl, setImageUrl] = useState<string>("");
    const [modalOpen, setModalOpen] = useState<boolean>(false);
    const canvasRef = useRef<HTMLCanvasElement>(null);
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

    return(
        <Container content={
            <div className="grid mx-auto text-4xl md:text-6xl gap-5">
                <div 
                    className="h-40 w-40 mx-auto border-3 flex justify-center items-center relative rounded-full cursor-pointer"
                    onClick={()=>setModalOpen(true)}
                >
                    <input type="file" accept="image/*" className="hidden" ref={fileInputRef} onChange={(e)=>handleFileChange(e)}/>
                    {imageUrl ? 
                        <canvas ref={canvasRef} className="border w-[150px] h-[150px] object-contain rounded-full"/>
                        :
                        <img src={"/UploadImage.png"} alt="uplaod image" className="h-10 w-10 cursor-pointer" onClick={()=>setModalOpen(true)}/>
                    }
                </div>
                <p>Username: {user.username}</p>
                <p>Date Joined: {user.dateCreated}</p>
                <Button label="Logout" onClick={logout} className="w-30 ml-auto"/>
                {modalOpen && 
                    <AddProfilePic
                        imgSrc={imageUrl}
                        imgRef={imgRef}
                        canvasRef={canvasRef}
                        onClose={()=>setModalOpen(false)}
                    />
                }
            </div>
        }/>
    );
}

export default Profile;