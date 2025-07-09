import {useState } from "react";
import AddFriend from "../components/Friends/AddFriends";
import UserDto from "../types/UserDto";
import FriendRequests from "../components/Friends/FriendRequests";
import Friend from "../components/Friends/Friend";
import FriendService from "../services/FriendService";
import Modal from "../components/General/Modal";

const Friends = (props:{user:UserDto | undefined, fetchUser: ()=>void})=>{
    const {user,fetchUser} = props;

    const [addFriends,setAddFriends] = useState<boolean>(false); //flag for showing the addfriends component
    const [displayFriendRequests,setSisplayFriendRequests] = useState<boolean>(false); //flag for showing the friend requests component

    const [removeFriendModal, setRemoveFriendModal] = useState<boolean>(false);
    const [displayedUsers,setDisplayedUsers] = useState<Record<string,string|undefined>>({});
    const totalFriendRequests:number = user ? Object.keys(user.friendRequests).length : 0;

    const removeFriend = async (username:string)=>{
        const resp = await FriendService.removeFriend(username);
        if(resp.status===200)
            fetchUser();
    }
    
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
            {/**Header with friend requests and add friends */}
            <div className="flex justify-between w-[85%] mx-auto text-3xl md:text-4xl my-5">
                <p className="cursor-pointer crossOut" onClick={()=>setSisplayFriendRequests(true)}>Friend Requests ({totalFriendRequests})</p>
                <div className="flex crossOut items-center gap-x-1 cursor-pointer" onClick={()=>setAddFriends(true)}>
                    <p>AddFriends</p>
                    <img src="Add.svg" alt="add" className="border-2 rounded-full h-5 w-5 p-[0.1rem]"/>
                </div>
            </div>
            {/**all displayed friends */}
            <div className="grid gap-y-10">
                {Object.entries(user?.friends||{}).map(([key, value]) =>
                    <Friend 
                        key={key} 
                        username={key} 
                        profilePic={value} 
                        buttons={
                            <img 
                                src="Minus.png" 
                                alt="remove friend" 
                                className="h-8" 
                                onClick={(e)=>{
                                    e.stopPropagation(); //prevents outer button from being clicked
                                    removeFriend(key);
                                }}
                            />
                        }
                        onClick={()=>alert("testing")}
                    />
                )}
            </div>
            <Modal content={<>asdf</>} onClose={()=>setRemoveFriendModal(false)} display={removeFriendModal}/>
        </div>;
}

export default Friends;