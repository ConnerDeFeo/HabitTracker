import { useState } from "react";
import FriendService from "../../services/FriendService";

const AddFriend = (
    props:{
        displayedUsers:Record<string,string>,
        setAddFriends:React.Dispatch<React.SetStateAction<boolean>>, 
        setDisplayedUsers:React.Dispatch<React.SetStateAction<Record<string,string>>>
    }
)=>{
    const {displayedUsers,setAddFriends,setDisplayedUsers} = props;
    const pfpSizing = "h-15 w-15 border-2 rounded-full";
    const [searchPhrase,setSearchPhrase] = useState<string>("");

    //Fetches the entered phrase
    const fetchUsers = async ()=>{
        const resp = await FriendService.findUsers(searchPhrase);
        if(resp.status==200){
            const users:Record<string,string> = await resp.json();
            setDisplayedUsers(users); 
        } 
    }

    const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
        if (event.key === 'Enter') 
            fetchUsers();
    };

    return(
        <>
            <div className="w-70 sm:w-100 md:w-125 mx-auto">
                <img src="BasicArrow.png" alt="basic arrow" className="rotate-180 h-8 w-8 my-5 cursor-pointer" onClick={()=>setAddFriends(false)}/>
                <div className="relative text-xl h-10 habitBorder mx-auto flex">
                    <input value={searchPhrase} onChange={(e)=>setSearchPhrase(e.target.value)} className="focus:outline-none pl-3" onKeyDown={handleKeyDown}/>
                    <img className="absolute h-6 w-6 right-3 top-[0.3rem] cursor-pointer" src="Search.png" alt="search" onClick={fetchUsers}/>
                </div>
            </div>
            <div className="grid gap-y-10 py-10">
                { Object.entries(displayedUsers).map(([key, value]) =>
                    <div key={key} className="habitBorder w-[85%] max-w-125 mx-auto flex justify-between items-center mx-auto px-[5%] h-30">
                        {value ? 
                            <img src={`https://habit-tracker-photos.s3.amazonaws.com/${value}`} alt={`${key} pfp`} className={pfpSizing}/>
                            :
                            <img src="UserIcon.png" alt="missing pfp" className={pfpSizing}/>
                        }
                        <p className={key.length < 15 ? "text-4xl" : "text-2xl"}>{key}</p>
                        <img src="Add.svg" className="h-6 w-6 my-auto cursor-pointer"/>
                    </div>
                )}
            </div>
        </>
    );
}

export default AddFriend;