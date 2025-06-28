import { useNavigate } from "react-router-dom";
import Button from "../components/Button";
import Container from "../components/Container";
import UserService from "../services/UserService";
import UserDto from "../types/UserDto";
import { useEffect, useRef, useState } from "react";
import PhotoService from "../services/PhotoService";

//Profile page the user sees
const Profile =(props:{user: UserDto, setUser: (user:UserDto)=>void})=>{
    const {user,setUser} = props;
    const navigate = useNavigate();

    const fileInputRef = useRef<HTMLInputElement | null>(null);
    const [imageUrl, setImageUrl] = useState<string | undefined>();

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

    const handleButtonClick = () => {
        if (fileInputRef.current) {
            fileInputRef.current.click(); // safe if current is not null
        }
    };

    const handleFileChange = () => {
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
                <div className="h-40 w-40 habitBorder mx-auto flex justify-center items-center">
                    {imageUrl ? 
                        <img src={imageUrl} alt="profile picture"/>
                        :
                        <>
                            <input type="file" accept="image/*" className="hidden" ref={fileInputRef}/>
                            <img src="/UploadImage.png" alt="uplaod image" className="h-10 w-10 cursor-pointer"/>
                        </>
                    }
                </div>
                <p>Username: {user.username}</p>
                <p>Date Joined: {user.dateCreated}</p>
                <Button label="Logout" onClick={logout} className="w-30 ml-auto"/>
            </div>
        }/>
    );
}

export default Profile;