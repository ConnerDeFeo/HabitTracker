import {useState } from "react";
import AddFriend from "../components/Friends/AddFriends";
import UserDto from "../types/UserDto";
import FriendRequests from "../components/Friends/FriendRequests";

const Friends = (props:{user:UserDto | undefined, fetchUser: ()=>void})=>{
    const {user,fetchUser} = props;
    //flag for showing the addfriends component
    const [addFriends,setAddFriends] = useState<boolean>(false);
    //flag for showing the friend requests component
    const [displayFriendRequests,setSisplayFriendRequests] = useState<boolean>(false);
    const [displayedUsers,setDisplayedUsers] = useState<Record<string,string>>({});
    const pfpSizing = "h-15 w-15 md:h-20 md:w-20 border-2 rounded-full";
    const totalFriendRequests:number = user ? Object.keys(user.friendRequests).length : 0;
    
    return addFriends ? //add friends clicked
            <AddFriend 
                setDisplayedUsers={setDisplayedUsers} 
                displayedUsers={displayedUsers} 
                setAddFriends={setAddFriends} 
                fetchUser={fetchUser} 
                friendRequestsSent={user ? user.friendRequestsSent : []}
            />
            :
            displayFriendRequests ? //friend request button clicked
            <FriendRequests friendRequests={user ? user.friendRequests : {}} fetchUser={fetchUser} setFriendRequests={()=>setSisplayFriendRequests(false)}/>
            : //default
            <div className="md:w-[80%] mx-auto">
                <div className="flex justify-between w-[85%] mx-auto text-3xl md:text-4xl my-5">
                    <p className="cursor-pointer crossOut" onClick={()=>setSisplayFriendRequests(true)}>Friend Requests ({totalFriendRequests})</p>
                    <div className="flex crossOut items-center gap-x-1 cursor-pointer" onClick={()=>setAddFriends(true)}>
                        <p>AddFriends</p>
                        <img src="Add.svg" alt="add" className="border-2 rounded-full h-5 w-5 p-[0.1rem]"/>
                    </div>
                </div>
                {
                    Object.entries(user?.friends||{}).map(([key, value]) =>
                        <div key={key} className="cursor-pointer habitBorder w-[85%] max-w-125 mx-auto flex justify-between items-center mx-auto px-5 h-25 md:h-30 md:px-10">
                            {value ? 
                                <img src={`https://habit-tracker-photos.s3.amazonaws.com/${value}`} alt={`${key} pfp`} className={pfpSizing}/>
                                :
                                <img src="UserIcon.png" alt="missing pfp" className={pfpSizing}/>
                            }
                            <p className={key.length < 15 ? "text-4xl" : "text-2xl md:text:4xl"}>{key}</p>
                        </div>
                    )
                }
            </div>;
}

export default Friends;