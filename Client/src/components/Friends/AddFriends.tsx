import { useEffect, useState } from "react";
import FriendService from "../../services/FriendService";

//Displayed after user clicks AddFriends button on friends page
const AddFriend = (
    props:{
        displayedUsers:Record<string,string>,
        setAddFriends:React.Dispatch<React.SetStateAction<boolean>>, 
        setDisplayedUsers:React.Dispatch<React.SetStateAction<Record<string,string>>>,
        fetchUser: ()=>void,
        friendRequestsSent: string[]
    }
)=>{
    const {displayedUsers,setAddFriends,setDisplayedUsers, fetchUser, friendRequestsSent} = props;
    const pfpSizing = "h-15 w-15 md:h-20 md:w-20 border-2 rounded-full";
    //Phrase being searched
    const [searchPhrase,setSearchPhrase] = useState<string>("");

    //onload fetch random users to display
    useEffect(()=>{
        const fetchRandomUsers = async ()=>{
            const resp = await FriendService.getRandomUsers();
            if(resp.status==200){
                const users = await resp.json();
                setDisplayedUsers(users);
            }
        }
        fetchRandomUsers();
    },[])

    //Fetches any users with the phrase in their name
    const fetchUsers = async ()=>{
        const resp = await FriendService.findUsers(searchPhrase);
        if(resp.status==200){
            const users:Record<string,string> = await resp.json();
            setDisplayedUsers(users); 
        } 
    }

    const sendFriendRequest= async(username:string)=>{
        const resp = await FriendService.sendFriendRequest(username);
        if(resp.status==200){
            fetchUser();
        } 
    }

    const unSendFriendRequest= async(username:string)=>{
        const resp = await FriendService.unSendFriendRequest(username);
        if(resp.status==200){
            fetchUser();
        } 
    }

    //Allows enter key to be pressed for a search
    const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
        if (event.key === 'Enter') 
            fetchUsers();
    };

    return(
        <div>
            {/**Seach bar */}
            <div className="w-70 sm:w-100 md:w-125 mx-auto">
                <img src="BasicArrow.png" alt="basic arrow" className="rotate-180 h-8 w-8 my-5 cursor-pointer" onClick={()=>setAddFriends(false)}/>
                <div className="relative text-xl h-10 habitBorder mx-auto flex">
                    <input value={searchPhrase} onChange={(e)=>setSearchPhrase(e.target.value)} className="focus:outline-none pl-3" onKeyDown={handleKeyDown}/>
                    <img className="absolute h-6 w-6 right-3 top-[0.3rem] cursor-pointer" src="Search.png" alt="search" onClick={fetchUsers}/>
                </div>
            </div>
            {/** displayed users*/}
            <div className="grid gap-y-10 py-10">
                { Object.entries(displayedUsers).map(([key, value]) =>
                    <div key={key} className="habitBorder w-[85%] max-w-125 mx-auto flex justify-between items-center mx-auto px-5 h-25 md:h-30 md:px-10">
                        {value ? 
                            <img src={`https://habit-tracker-photos.s3.amazonaws.com/${value}`} alt={`${key} pfp`} className={pfpSizing}/>
                            :
                            <img src="UserIcon.png" alt="missing pfp" className={pfpSizing}/>
                        }
                        <p className={key.length < 15 ? "text-4xl" : "text-2xl"}>{key}</p>
                        {friendRequestsSent.some(u=> u===key) ? 
                            <img src="checkMark.webp" className="h-6 w-6 my-auto cursor-pointer" onClick={()=>unSendFriendRequest(key)}/>
                            :
                            <img src="Add.svg" className="h-6 w-6 my-auto cursor-pointer" onClick={()=>sendFriendRequest(key)}/>
                        }
                    </div>
                )}
            </div>
        </div>
    );
}

export default AddFriend;