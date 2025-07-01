import { useNavigate } from "react-router-dom";
import Button from "../components/General/Button";
import Container from "../components/General/Container";
import UserService from "../services/UserService";
import UserDto from "../types/UserDto";
import { useEffect, useState } from "react";
import PhotoService from "../services/PhotoService";
import AddProfilePic from "../components/Profile/AddProfilePic";

//Profile page the user sees
const Profile =(props:{user: UserDto, setUser: (user:UserDto)=>void})=>{
    const {user,setUser} = props;
    const navigate = useNavigate();

    const [imageUrl, setImageUrl] = useState<string>("");
    const [modalOpen, setModalOpen] = useState<boolean>(false);

    //On load, fetch profile photo, then set canvascrop
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

    //Logout button pressed
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
                    className="h-40 w-40 mx-auto border-3 flex justify-center items-center rounded-full"
                >
                    {imageUrl ? 
                        <div className="relative">
                            <img src={imageUrl} alt="Profile pic" className="rounded-full"/>
                            <div 
                                className="absolute h-10 w-10 border-2 bg-white bottom-0 right-0 flex justify-center items-center rounded-full cursor-pointer"
                                onClick={()=>setModalOpen(true)}
                            >
                                <img src={"/EditHabits.svg"} alt="uplaod image" className="h-5 w-5 "/>
                            </div>
                        </div>
                        :
                        <img src={"/UploadImage.png"} alt="uplaod image" className="h-10 w-10 cursor-pointer" onClick={()=>setModalOpen(true)}/>
                    }
                </div>
                <p>Username: {user.username}</p>
                <p>Date Joined: {user.dateCreated}</p>
                <Button label="Logout" onClick={logout} className="w-30 ml-auto"/>
                {modalOpen && 
                    <AddProfilePic
                        onClose={()=>setModalOpen(false)}
                        setImageUrl={setImageUrl}
                    />
                }
            </div>
        }/>
    );
}

export default Profile;