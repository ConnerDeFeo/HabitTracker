import {useState } from "react";
import AddFriend from "../components/Friends/AddFriends";
import UserDto from "../types/UserDto";
import FriendRequests from "../components/Friends/FriendRequests";
import Friend from "../components/Friends/Friend";
import Modal from "../components/General/Modal";
import { useNavigate } from "react-router-dom";
import Button from "../components/General/Button";
import FriendModificationService from "../services/FriendModificationService";

const Friends = (props:{user:UserDto | undefined, fetchUser: ()=>void})=>{
    const {user,fetchUser} = props;
    const navigate = useNavigate();
    const [addFriends,setAddFriends] = useState<boolean>(false); //flag for showing the addfriends component
    const [displayFriendRequests,setSisplayFriendRequests] = useState<boolean>(false); //flag for showing the friend requests component
    const [removeFriendModal, setRemoveFriendModal] = useState<string>(""); //Flag for remove friend modal?
    const totalFriendRequests:number = user ? Object.keys(user.friendRequests).length : 0;

    const removeFriend = async (username:string)=>{
        const resp = await FriendModificationService.removeFriend(username);
        if(resp.status===200)
            fetchUser();
        setRemoveFriendModal("");
    }
    
    return addFriends ? //add friends clicked
        <AddFriend 
            setAddFriends={setAddFriends} 
            fetchUser={fetchUser} 
            friendRequestsSent={user ? user.friendRequestsSent : []}
            friends={user?.friends || {}}
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
                    <img src="/Add.svg" alt="add" className="border-2 rounded-full h-5 w-5 p-[0.1rem]"/>
                </div>
            </div>
            {/**all displayed friends */}
            <div className="grid gap-y-10">
                {Object.entries(user?.friends||{}).map(([key, value]) =>
                    <Friend 
                        key={key} 
                        username={key} 
                        userId={value} 
                        buttons={
                            <img 
                                src="/Minus.png" 
                                alt="remove friend" 
                                className="h-8" 
                                onClick={(e)=>{
                                    e.stopPropagation(); //prevents outer button from being clicked
                                    setRemoveFriendModal(key);
                                }}
                            />
                        }
                        onClick={()=>navigate(`/@/${key}`)}
                    />
                )}
            </div>
            {/**Modal displayed when minus icon is hit */}
            <Modal content={
                <div className="h-40 w-75">
                    <p className="text-4xl text-center mt-5 mb-15">Are you sure you want to remove {removeFriendModal}?</p>
                    <div className="flex justify-between">
                        <Button label="Yes" className="w-25" onClick={()=>removeFriend(removeFriendModal)}/>
                        <Button label="No" className="w-25" onClick={()=>setRemoveFriendModal("")}/>
                    </div>
                </div>
            } onClose={()=>setRemoveFriendModal("")} display={removeFriendModal!==""}/>
        </div>;
}

export default Friends;