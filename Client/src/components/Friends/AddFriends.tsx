import { useEffect, useState } from "react";
import Friend from "./Friend";
import SocialDataService from "../../services/SocialDataService";
import FriendModificationService from "../../services/FriendModificationService";

//Displayed after user clicks AddFriends button on friends page
const AddFriend = (
    props:{
        setAddFriends:React.Dispatch<React.SetStateAction<boolean>>, 
        fetchUser: ()=>void,
        friendRequestsSent: string[],
        friends: Record<string,string | undefined>
    }
)=>{
    
    const {setAddFriends, fetchUser, friendRequestsSent,friends} = props;
    //Phrase being searched
    const [searchPhrase,setSearchPhrase] = useState<string>("");
    
    const [displayedUsers,setDisplayedUsers] = useState<Record<string,string|undefined>>({});

    //onload fetch random users to display
    useEffect(()=>{
        const fetchRandomUsers = async ()=>{
            const resp = await SocialDataService.getRandomUsers();
            if(resp.status==200){
                const users = await resp.json();
                setDisplayedUsers(users);
            }
        }
        fetchRandomUsers();
    },[])

    //Fetches any users with the phrase in their name
    const fetchUsers = async ()=>{
        const resp = await SocialDataService.findUsers(searchPhrase);
        if(resp.status==200){
            const users:Record<string,string> = await resp.json();
            setDisplayedUsers(users); 
        } 
    }

    const sendFriendRequest= async(username:string)=>{
        const resp = await FriendModificationService.sendFriendRequest(username);
        if(resp.status==200){
            fetchUser();
        } 
    }

    const unSendFriendRequest= async(username:string)=>{
        const resp = await FriendModificationService.unSendFriendRequest(username);
        if(resp.status==200){
            fetchUser();
        } 
    }

    //Allows enter key to be pressed for a search
    const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
        if (event.key === 'Enter') 
            fetchUsers();
    };

    const removeFriend = async (username:string)=>{
        const resp = await FriendModificationService.removeFriend(username);
        if(resp.status===200)
            fetchUser();
    }

    return(
        <div>
            {/**Seach bar */}
            <div className="w-70 sm:w-100 md:w-125 mx-auto">
                <img src="/BasicArrow.png" alt="basic arrow" className="rotate-180 h-8 w-8 my-5 cursor-pointer" onClick={()=>setAddFriends(false)}/>
                <div className="relative text-xl h-10 habitBorder mx-auto flex">
                    <input value={searchPhrase} onChange={(e)=>setSearchPhrase(e.target.value)} className="focus:outline-none pl-3" onKeyDown={handleKeyDown}/>
                    <img className="absolute h-6 w-6 right-3 top-[0.3rem] cursor-pointer" src="/Search.png" alt="search" onClick={fetchUsers}/>
                </div>
            </div>
            {/** displayed users*/}
            <div className="grid gap-y-10 py-10">
                { Object.entries(displayedUsers).map(([key, value]) =>
                    <Friend key={key} username={key} userId={value} buttons={
                        //friend request sent
                        friendRequestsSent.some(u=> u===key) ? 
                        <img src="checkMark.webp" className="h-6 w-6 my-auto cursor-pointer" onClick={()=>unSendFriendRequest(key)}/>
                        :
                        //already friends
                        friends && key in friends ? 
                        <img 
                            src="/Minus.png" 
                            alt="remove friend" 
                            className="h-8 cursor-pointer" 
                            onClick={(e)=>{
                                e.stopPropagation(); //prevents outer button from being clicked
                                removeFriend(key);
                            }}
                        />
                        :
                        <img src="Add.svg" className="h-6 w-6 my-auto cursor-pointer" onClick={()=>sendFriendRequest(key)}/>
                        
                    }/>
                )}
            </div>
        </div>
    );
}

export default AddFriend;