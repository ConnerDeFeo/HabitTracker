
//This is the box that displays username, profile pic, and some buttons on the right hand side of the component
const Friend = (
    props:{
        username:string,
        profilePic:string | undefined,
        buttons?:React.ReactNode,
        onClick?:()=>void
    }
)=>{
    const {username,profilePic,buttons, onClick} = props;
    const pfpSizing = "h-15 w-15 md:h-20 md:w-20 border-2 rounded-full";

    return(
        <div className={`habitBorder w-[85%] max-w-125 mx-auto flex justify-between items-center mx-auto px-5 h-25 md:h-30 md:px-10 ${onClick&&'cursor-pointer'}`} onClick={onClick}>
            {profilePic ? 
                <img src={`https://habit-tracker-photos.s3.amazonaws.com/${profilePic}`} alt={`${username} pfp`} className={pfpSizing}/>
                :
                <img src="UserIcon.png" alt="missing pfp" className={pfpSizing}/>
            }
            <p className={username.length < 15 ? "text-4xl" : "text-2xl"}>{username}</p>
            {buttons}
        </div>
    );
}

export default Friend;