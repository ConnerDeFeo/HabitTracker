import { useNavigate } from "react-router-dom";
import UserService from "../services/UserService";
import UserDto from "../types/UserDto";
import { useEffect, useState } from "react";
import AddProfilePic from "../components/Profile/AddProfilePic";
import ProfileHabit from "../types/ProfileHabit";
import DateService from "../services/DateService";
import DateData from "../data/DateData";
import RenderCurrentMonth from "../components/Profile/RenderCurrentMonth";

//Profile page the user sees
const Profile =(props:{user: UserDto, setUser: (user?:UserDto)=>void})=>{
    const {user,setUser} = props;
    const navigate = useNavigate();

    const [imageUrl, setImageUrl] = useState<string>("");
    const [modalOpen, setModalOpen] = useState<boolean>(false);
    const [currentHabits, setCurrentHabits] = useState<ProfileHabit[]>([]);
    const [currentMonthHabitsCompleted, setCurrentMonthHabitsCompleted] = useState<Record<string,boolean>>({});

    //On load, fetch profile photo, then set canvascrop
    useEffect(()=>{
        if(user.profilePhotoKey)
            setImageUrl(`https://habit-tracker-photos.s3.amazonaws.com/${user.profilePhotoKey}`);
        
    },[user.profilePhotoKey]);

    useEffect(()=>{
        const fetchProfileHabits = async ()=>{
            const resp = await UserService.GetProfile();
            if(resp.status==200){
                const profileHabits = await resp.json();
                setCurrentHabits(profileHabits.currentHabits);
                setCurrentMonthHabitsCompleted(profileHabits.currentMonthHabitsCompleted);
            }
        }
        fetchProfileHabits();
    },[])


    //Logout button pressed
    const logout = async ()=>{
        await UserService.Logout();
        setUser(undefined);
        navigate('/');
    }

    return(
        <div className="my-15 lg:w-[85%] mx-auto">
            <div className="grid md:grid-cols-2 w-[85%] mx-auto">
                {/**Profile picture*/}
                <div className="grid justify-center md:flex md:justify-between items-center">
                    <div 
                        className="h-40 w-40 border-3 flex justify-center items-center rounded-full"
                    >
                        {imageUrl ? 
                            <div className="relative">
                                <img src={imageUrl} alt="Profile pic" className="rounded-full"/>
                                <div 
                                    className="absolute h-10 w-10 border-2 bg-white bottom-0 right-0 flex justify-center items-center rounded-full cursor-pointer"
                                    onClick={()=>setModalOpen(true)}
                                >
                                    <img src={"/EditHabits.svg"} alt="uplaod image" className="h-5 w-5 "/>
                                </div>
                            </div>
                            :
                            <img src={"/UploadImage.png"} alt="uplaod image" className="h-10 w-10 cursor-pointer" onClick={()=>setModalOpen(true)}/>
                        }
                    </div>
                    <p className={`${user.username.length > 12 ? "text-2xl" : "text-4xl"} lg:text-4xl text-center my-5`}>{user.username}</p>
                </div>

                {/**Current Habits*/}
                <div className="row-span-2">
                    <div className="grid w-75 mx-auto mb-10">
                        <h1 className="text-6xl text-center border-b-3 mb-5">Current Habits</h1>
                        <div className="grid gap-y-5 overflow-y-auto max-h-100">
                            {currentHabits.map( habit => (
                                <div key={habit.name} className="habitBorder p-2">
                                    <p className="text-4xl text-center">{habit.name}</p>
                                    <p className="text-2xl ml-5">Created: {habit.dateCreated}</p>
                                    <p className="text-2xl ml-5">Current Streak: {habit.currentStreak}</p>
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
                
                {/**Calender displayed */}
                <RenderCurrentMonth currentMonthHabitsCompleted={currentMonthHabitsCompleted}/>
            </div>

            <div className="md:flex w-[85%] mx-auto items-center my-10">
                <p className="text-4xl text-center my-5">Member Sense: {user.dateCreated}</p>
                <div className="mr-[15%] md:mr-0 flex text-4xl crossOut w-fit ml-auto cursor-pointer" onClick={logout}>
                    <p>Logout</p>
                    <img src="/logout.webp" alt="logout" className="h-10 w-10"/>
                </div>
            </div>
            {/**Modal for profile pic*/}
            <AddProfilePic
                onClose={()=>setModalOpen(false)}
                setImageUrl={setImageUrl}
                hidden={!modalOpen}
            />
        </div>
    );
}

export default Profile;