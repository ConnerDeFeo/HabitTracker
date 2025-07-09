import { useEffect, useState } from "react";
import PhotoService from "../../services/PhotoService";

//This is the box that displays username, profile pic, and some buttons on the right hand side of the component
const Friend = (
    props:{
        username:string,
        userId:string | undefined,
        buttons?:React.ReactNode,
        onClick?:()=>void
    }
)=>{
    const {username,userId,buttons, onClick} = props;
    const pfpSizing = "h-15 w-15 md:h-20 md:w-20 border-2 rounded-full";

    const [validUrl,setValidUrl] = useState<boolean>(false);

    useEffect(()=>{
        const validate = async()=>{
            if(await PhotoService.validateImageUrl(`https://habit-tracker-photos.s3.amazonaws.com/profilePhotos/${userId}`))
                setValidUrl(true);
        }
        validate();
    },[userId]);

    console.log(userId)
    return(
        <div className={`habitBorder w-[85%] max-w-125 mx-auto flex justify-between items-center mx-auto px-5 h-25 md:h-30 md:px-10 ${onClick&&'cursor-pointer'}`} onClick={onClick}>
            {validUrl ? 
                <img src={`https://habit-tracker-photos.s3.amazonaws.com/profilePhotos/${userId}`} alt={`${username} pfp`} className={pfpSizing}/>
                :
                <img src="UserIcon.png" alt="missing pfp" className={pfpSizing}/>
            }
            <p className={username.length < 15 ? "text-4xl" : "text-2xl"}>{username}</p>
            {buttons}
        </div>
    );
}

export default Friend;