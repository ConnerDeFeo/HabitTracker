import { useNavigate } from "react-router-dom";
import Navbar from "./Navbar";
import ProfilePicture from "../General/ProfilePicture";

//Navigation bar for the site
const Navigation = (
    props:{
        displayMenu:()=>void, 
        loggedIn:boolean, 
        useHamburger:boolean, 
        imageUrl:string,

    }
)=>{
    const {displayMenu,loggedIn, useHamburger,imageUrl} = props;
    const navigate = useNavigate();

    return (
        <div className="flex place-content-between border-b-4 border-black h-24 w-[85%] mx-auto pt-3 items-center">
            {useHamburger ? 
                <img src="/hamburger.png" className="w-8 h-8 cursor-pointer" onClick={displayMenu}/>
                : 
                <div className="flex gap-8">
                    <Navbar loggedIn={loggedIn}/>
                </div>
            }
            <ProfilePicture 
                imageUrl={imageUrl} 
                className=" h-12 w-12 md:h-16 md:w-16 cursor-pointer"
                onClick={()=>navigate("/Profile")}
            />
        </div>
    );
} 

export default Navigation