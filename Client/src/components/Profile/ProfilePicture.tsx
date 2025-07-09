import { useEffect, useState } from "react";
import PhotoService from "../../services/PhotoService";

const ProfilePicture = (props:{imageUrl:string, editAction?:()=>void})=>{
    const {imageUrl, editAction} = props;

    const [validImageUrl, setValidImageUrl] = useState<boolean>(false);

    //Checks to see if the url is valid
    useEffect(() => {
        const checkImage = async () => {
            if(await PhotoService.validateImageUrl(imageUrl))
                setValidImageUrl(true);
            else
                setValidImageUrl(false);
        };
        checkImage();
    }, [imageUrl]);

    return(
        <div className="grid justify-center md:flex md:justify-between items-center">
            <div 
                className="h-40 w-40 border-3 flex justify-center items-center rounded-full"
            >
                {validImageUrl ? 
                    <div className="relative">
                        <img 
                            src={imageUrl} 
                            alt="Profile pic" 
                            className="rounded-full"
                        />
                        {/**If can edit, show the edit pencil icon */}
                        {editAction && 
                            <div 
                                className="absolute h-10 w-10 border-2 bg-white bottom-0 right-0 flex justify-center items-center rounded-full cursor-pointer"
                                onClick={editAction}
                            >
                                <img src={"/EditHabits.svg"} alt="uplaod image" className="h-5 w-5 "/>
                            </div>
                        }
                    </div>   
                    :
                    //if can edit, then show the uplaod image icon
                    editAction ? 
                        <img src={"/UploadImage.png"} alt="uplaod image" className="h-10 w-10 cursor-pointer" onClick={editAction}/>
                        :
                        <img src="UserIcon.png" alt="missing pfp" className={"h-15 w-15 md:h-20 md:w-20 border-2 rounded-full"}/>
                }
            </div>
        </div>
    );
}

export default ProfilePicture;