import { ChangeEvent, SyntheticEvent, useRef, useState } from "react";
import ReactCrop, { centerCrop, convertToPixelCrop, Crop, makeAspectCrop } from "react-image-crop";
import Button from "../General/Button";
import Modal from "../General/Modal";
import ConvertToCanvas from "./ConvertToCanvas";
import heic2any from "heic2any";
import UploadProfilePicture from "./UploadProfilePicture";
import UserDto from "../../types/UserDto";
import UserService from "../../services/UserService";

//When use gos to upload a new pfp
const EditUser = (
    props:{ 
        onClose: ()=>void,
        hidden:boolean,
        updateUrl: ()=>void,
        user:UserDto,
        setUser:React.Dispatch<React.SetStateAction<UserDto | undefined>>
    }
) =>{
    const {onClose, hidden, updateUrl,user,setUser} = props;
    const canvas: HTMLCanvasElement = document.createElement("canvas");
    canvas.width=150;
    canvas.height=150;
    const ASPECT = 1;
    const MINWIDTH = 150;

    const imgRef = useRef<HTMLImageElement | null>(null);
    const fileInputRef = useRef<HTMLInputElement | null>(null);
    
    const [newUsername,setNewUserName] = useState<string>(user.username);
    const [message,setMessage] = useState<string>("");
    const [imgSrc,setImgSrc] = useState<string>("");
    const [crop, setCrop] = useState<Crop>({
        unit: '%', // Can be 'px' or '%'
        x: 200,
        y: 100,
        width: 25,
        height: 25,
    });

    //Once file is selected, set the current uplaodedimg to that file
    const handleFileChange = async (e: ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if(file){

            let fileToUse = file;

            // Convert HEIC to JPEG because HEIC sucks why does apple use it god damn
            if (file.type === "image/heic" || file.name.endsWith(".heic")) {
                const converted = await heic2any({
                    blob: file,
                    toType: "image/jpeg",
                    quality: 0.9,
                });
                fileToUse = new File([converted as BlobPart], "converted.jpg", { type: "image/jpeg" });
            }

            const reader = new FileReader();
            reader.addEventListener('load',()=>{
                const uploadedFile = reader.result?.toString() || "";
                setImgSrc(uploadedFile);
            });
            reader.readAsDataURL(fileToUse);
        }
    };

    //Once image is uploaded, set image crop to default to center of picture
    const onImageLoad = (e:SyntheticEvent<HTMLImageElement, Event>)=>{
        const {width,height} = e.currentTarget;
        const cropWidthAsPercent = (MINWIDTH / width) * 100;
        const crop = makeAspectCrop(
            {
                unit:'%',
                width:cropWidthAsPercent
            },
            ASPECT,
            width,
            height
        )   
        const centeredCrop = centerCrop(crop,width,height);
        setCrop(centeredCrop);
    }

    //On completion of the image being uploaded and cropped, send to backend for url
    const handleFileUpload = async () => {
        UploadProfilePicture(canvas,onClose, updateUrl);
    };

    const setUsername = async ()=>{
        if(newUsername.length >20){
            setMessage("Username to long");
            return;
        }
        if(user.username==="Guest"){
            setMessage("You Can't change this username!");
            return;
        }
        const resp = await UserService.ChangeUsername(newUsername);
        if(resp.ok ){
            user.username = newUsername;
            setUser((prevUser)=>{
                if(!prevUser)
                    return prevUser;
                return{
                    ...prevUser,
                    username:newUsername
                }
            });
            setMessage("")
        }
        else    
            setMessage("username taken");
    }


    return(
        <Modal content={
            <div className="grid gap-y-5 relative">
                <p className="absolute right-35 top-0 text-4xl text-red-500">{message}</p>
                <div className="flex justify-between items-center mt-10 relative">
                    <img src="/x.webp" alt="exit modal" className="cursor-pointer h-15 w-15" onClick={onClose}/>
                    <div className="flex items-center">
                        <input className="text-2xl p-3 habitBorder mr-30" value={newUsername} onChange={(e)=>setNewUserName(e.target.value)}/>
                        <img 
                            src="checkMark.webp" 
                            alt="change username" 
                            className="w-12 h-12 border-3 rounded-full p-1 cursor-pointer absolute right-12" 
                            onClick={setUsername} hidden={user.username===newUsername}/>
                    </div>
                </div>
                <label className="color-black border border-black font-hand text-4xl bg-black text-white rounded-2xl cursor-pointer p-2 w-fit">
                    Choose File
                    <input
                        type="file"
                        accept="image/*"
                        ref={fileInputRef}
                        onChange={handleFileChange}
                        className="hidden"
                    />
                </label>
                {imgSrc && 
                    <ReactCrop
                        crop={crop}
                        keepSelection
                        circularCrop
                        aspect={ASPECT}
                        onChange={(_,percent)=>setCrop(percent)}
                        minWidth={MINWIDTH}
                        className="max-h-100 mx-auto"
                    >
                         <img 
                            src={imgSrc} 
                            alt={"Upload"}
                            onLoad={e=>onImageLoad(e)}
                            ref={imgRef}
                        />
                    </ReactCrop>

                }
                {imgRef.current && 
                    <Button label="Confirm" className="w-30" onClick={
                    ()=>{
                        if(imgRef.current)
                            ConvertToCanvas(
                                imgRef.current,
                                canvas,
                                convertToPixelCrop(crop,imgRef.current.width,imgRef.current.height)
                            )
                        handleFileUpload();
                    }}/>
                }
            </div>
        } onClose={onClose} display={!hidden} className="h-200 w-175"/>
    );
}

export default EditUser;