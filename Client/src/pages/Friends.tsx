import {useState } from "react";
import AddFriend from "../components/Friends/AddFriends";
import UserDto from "../types/UserDto";

const Friends = (props:{user:UserDto | undefined, fetchUser: ()=>void})=>{
    const {user,fetchUser} = props;
    //flag for showing the addfriends page
    const [addFriends,setAddFriends] = useState<boolean>(false);
    //flag for showing the friend requests page
    const [friendRequests,setFriendRequests] = useState<boolean>(false);
    const [displayedUsers,setDisplayedUsers] = useState<Record<string,string>>({});
    

    return addFriends ?
            <AddFriend setDisplayedUsers={setDisplayedUsers} displayedUsers={displayedUsers} setAddFriends={setAddFriends} fetchUser={fetchUser} user={user}/>
            :
            <>
                <div className="flex justify-between w-[85%] mx-auto text-2xl my-5">
                    <p className="cursor-pointer">Friend Requests(1)</p>
                    <div className="flex crossOut items-center gap-x-1 cursor-pointer" onClick={()=>setAddFriends(true)}>
                        <p>AddFriends</p>
                        <img src="Add.svg" alt="add" className="border-2 rounded-full h-5 w-5"/>
                    </div>
                </div>
                <div>
                    all friends
                </div>
            </>;
}

export default Friends;