import { useEffect, useState } from "react";
import FriendService from "../services/FriendService";

const Friends = ()=>{
    //flag for showing the addfriends page
    const [addFriends,setAddFriends] = useState<boolean>(false);
    //flag for showing the friend requests page
    const [friendRequests,setFriendRequests] = useState<boolean>(false);
    const [searchPhrase,setSearchPhrase] = useState<string>("");
    const [displayedUsers,setDisplayedUsers] = useState<Record<string,string>>({});
    
    
    const fetchUsers = async ()=>{
        const resp = await FriendService.findUsers(searchPhrase);
        if(resp.status==200){
            const users:Record<string,string> = await resp.json();
            setDisplayedUsers(users); 
        }
    }

    return addFriends ?
            <>
                <img src="BasicArrow.png" alt="basic arrow" className="rotate-180 h-8 w-8 ml-[7.5%] my-5 cursor-pointer" onClick={()=>setAddFriends(false)}/>
                <div className="relative text-xl h-10 w-80 habitBorder mx-auto flex">
                    <input value={searchPhrase} onChange={(e)=>setSearchPhrase(e.target.value)} className="focus:outline-none pl-3"/>
                    <img className="absolute h-6 w-6 right-3 top-[0.3rem] cursor-pointer" src="Search.png" alt="search" onClick={fetchUsers}/>
                </div>
                { Object.entries(displayedUsers).map(([key, value]) =>
                    <div key={key} className="habitBorder w-[85%] mx-auto flex justify-between mx-auto px-[10%]">
                        <img src={value ? `https://habit-tracker-photos.s3.amazonaws.com/${value}`:"a"} alt={`${key} pfp`} className="h-10 w-10 rounded-full"/>
                        <p>{key}</p>

                    </div>
                )}
            </>
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