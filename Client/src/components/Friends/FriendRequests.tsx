import FriendService from "../../services/FriendService";
import Friend from "./Friend";

//Displayed after user clicks FriendRequests button on friends page
const FriendRequests = (
    props:{
        friendRequests:Record<string,string | undefined>,
        fetchUser:()=>void,
        setFriendRequests: ()=>void
    }
)=>{
    const {friendRequests, fetchUser, setFriendRequests} = props;

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
                        <Friend key={key} username={key} profilePic={value} buttons={
                            <div className="">
                                <img src="checkMark.webp" alt="check" className="h-8 w-8 m-auto cursor-pointer" onClick={()=>acceptFriendRequest(key)}/>
                                <img src="x.webp" alt="x" className="h-10 w-10 m-auto cursor-pointer" onClick={()=>rejectFriendRequest(key)}/>
                            </div>
                        }/>
                    )}
                </>
                :
                <p className="text-4xl text-center">No friend requests! (no one likes you)</p>
            }
        </div>
    );
}

export default FriendRequests;