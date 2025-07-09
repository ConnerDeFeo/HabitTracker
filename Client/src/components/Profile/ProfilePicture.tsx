const ProfilePicture = (props:{imageUrl?:string, editAction?:()=>void})=>{
    const {imageUrl, editAction} = props;
    return(
        <div 
            className="h-40 w-40 border-3 flex justify-center items-center rounded-full"
        >
            {imageUrl ? 
                <div className="relative">
                    <img src={imageUrl} alt="Profile pic" className="rounded-full"/>
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
    );
}

export default ProfilePicture;