import { useState } from "react";

//Profile pictures that are displayed
const ProfilePicture = (props:{imageUrl:string, editAction?:()=>void})=>{
    const {imageUrl, editAction} = props;
    const [imgError, setImgError] = useState(false);

    return(
        <div className="grid justify-center md:flex md:justify-between items-center">
            <div 
                className="h-40 w-40 border-3 flex justify-center items-center rounded-full"
            >
                <div className="relative">
                    <img
                        src={imgError ? "/UserIcon.png" : imageUrl}
                        onError={() => setImgError(true)}
                        alt={`user profile`}
                        className='rounded-full'
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
            </div>
        </div>
    );
}

export default ProfilePicture;