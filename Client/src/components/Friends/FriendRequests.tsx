import FriendService from "../../services/FriendService";

//Displayed after user clicks FriendRequests button on friends page
const FriendRequests = (
    props:{
        friendRequests:Record<string,string | undefined>,
        fetchUser:()=>void,
        setFriendRequests: ()=>void
    }
)=>{
    const {friendRequests, fetchUser, setFriendRequests} = props;
    const pfpSizing = "h-15 w-15 md:h-20 md:w-20 border-2 rounded-full";

    const acceptFriendRequest = async (username:string)=>{
        const resp = await FriendService.acceptFriendRequest(username);
        if(resp.status==200)
            fetchUser();
        
    }

    const rejectFriendRequest = async (username:string)=>{
        const resp = await FriendService.rejectFriendRequest(username);
        if(resp.status==200)
            fetchUser();
    }

    return(
        <div className="my-10">
            {/** arrow*/}
            <div className="w-70 sm:w-100 md:w-125 mx-auto">
                <img src="BasicArrow.png" alt="basic arrow" className="rotate-180 h-8 w-8 my-5 cursor-pointer" onClick={setFriendRequests}/>
            </div>
            {/** displayed users*/}
            {Object.keys(friendRequests).length > 0 ? 
                <>
                    { Object.entries(friendRequests).map(([key, value]) =>
                        <div key={key} className="habitBorder w-[85%] max-w-125 mx-auto flex justify-between items-center mx-auto px-5 h-25 md:h-30 md:px-10">
                            {value ? 
                                <img src={`https://habit-tracker-photos.s3.amazonaws.com/${value}`} alt={`${key} pfp`} className={pfpSizing}/>
                                :
                                <img src="UserIcon.png" alt="missing pfp" className={pfpSizing}/>
                            }
                            <p className={key.length < 15 ? "text-4xl" : "text-2xl"}>{key}</p>
                            <div className="">
                                <img src="checkMark.webp" alt="check" className="h-8 w-8 m-auto cursor-pointer" onClick={()=>acceptFriendRequest(key)}/>
                                <img src="x.webp" alt="x" className="h-10 w-10 m-auto cursor-pointer" onClick={()=>rejectFriendRequest(key)}/>
                            </div>
                        </div>
                    )}
                </>
                :
                <p className="text-4xl text-center">No friend requests! (no one likes you)</p>
            }
        </div>
    );
}

export default FriendRequests;