import { useEffect, useState } from "react";

const ProfilePicture = (props:{imageUrl:string, editAction?:()=>void})=>{
    const {imageUrl, editAction} = props;

    const [validImageUrl, setValidImageUrl] = useState<boolean>(false);

    useEffect(() => {
        const checkImage = async () => {
            if(imageUrl){
                const res = await fetch(imageUrl, { method: "HEAD" }); // HEAD is faster, gets headers only
                if (res.ok) 
                    setValidImageUrl(true);
            }
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
                        <div 
                            className="absolute h-10 w-10 border-2 bg-white bottom-0 right-0 flex justify-center items-center rounded-full cursor-pointer"
                            onClick={editAction}
                        >
                            <img src={"/EditHabits.svg"} alt="uplaod image" className="h-5 w-5 "/>
                        </div>
                    </div>   
                    :
                    <img src={"/UploadImage.png"} alt="uplaod image" className="h-10 w-10 cursor-pointer" onClick={editAction}/>
                }
            </div>
        </div>
    );
}

export default ProfilePicture;