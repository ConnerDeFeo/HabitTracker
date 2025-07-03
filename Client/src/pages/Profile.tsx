import { useNavigate } from "react-router-dom";
import UserService from "../services/UserService";
import UserDto from "../types/UserDto";
import { useEffect, useState } from "react";
import AddProfilePic from "../components/Profile/AddProfilePic";
import ProfileHabit from "../types/ProfileHabit";
import DateService from "../services/DateService";
import DateData from "../data/DateData";

//Profile page the user sees
const Profile =(props:{user: UserDto, setUser: (user?:UserDto)=>void})=>{
    const {user,setUser} = props;
    const navigate = useNavigate();

    const now = new Date();
    const daysInCurrentMonth: number = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();
    const firtDayOfMonth = new Date(now.getFullYear(), now.getMonth(), 1).getDay();


    const [imageUrl, setImageUrl] = useState<string>("");
    const [modalOpen, setModalOpen] = useState<boolean>(false);
    const [currentHabits, setCurrentHabits] = useState<ProfileHabit[]>([]);
    const [currentMonthHabitsCompleted, setCurrentMonthHabitsCompleted] = useState<Record<string,boolean>>({});

    //On load, fetch profile photo, then set canvascrop
    useEffect(()=>{
        if(user.profilePhotoKey)
            setImageUrl(`https://habit-tracker-photos.s3.amazonaws.com/${user.profilePhotoKey}`);
        
    },[user]);

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

    //Calender displayed at bottom of profile page
    const renderMonth = ()=>{
        return(
            <>
                {Array.from({ length: daysInCurrentMonth }, (_, i) => i + 1).map((day) =>{
                    const habitsCompleted = currentMonthHabitsCompleted?.[DateService.padZero(day)];

                    let bgClass = "";

                    if (habitsCompleted !==undefined) {
                        bgClass = habitsCompleted? "bg-green-500" : "bg-red-500";
                    } 
                    else{
                        bgClass = "bg-gray-500";
                    }
                    return (
                        <div
                            key={day}
                            className={
                                `border-2 border-black rounded-sm mb-5 relative h-5 w-5 md:h-10 md:w-10 shadow-md shadow-black ${bgClass}`  
                            }
                        ></div>
                    );
                })}
            </>
        );
    }

    //Logout button pressed
    const logout = async ()=>{
        await UserService.Logout();
        setUser(undefined);
        navigate('/');
    }

    return(
        <div className="grid my-10">
            {/**Profile picture*/}
            <div 
                className="h-40 w-40 mx-auto border-3 flex justify-center items-center rounded-full"
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

            <p className="text-6xl text-center my-5">{user.username}</p>

            {/**Current Habits*/}
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
            
            {/**Calender displayed */}
            <h2 className="text-3xl md:text-5xl md:mt-5 font-hand text-center">{DateData.months[new Date().getMonth()]}</h2>
            <div className="grid grid-cols-7 justify-items-center w-[80%] mx-auto mt-2">
                {DateData.days.map((day,i)=>
                    <p className={"text-2xl md:text-4xl "+(i < firtDayOfMonth && "row-span-2")} 
                        key={day}
                    >
                        {day.substring(0,1)}
                    </p>
                )}
                {renderMonth()}
            </div>
            
            {/**Modal*/}
            {modalOpen && 
                <AddProfilePic
                    onClose={()=>setModalOpen(false)}
                    setImageUrl={setImageUrl}
                />
            }
            <p className="text-4xl text-center my-5">Member Sense: {user.dateCreated}</p>
            <div className="flex text-4xl crossOut w-fit ml-auto mr-[15%]" onClick={logout}>
                <p>Logout</p>
                <img src="/logout.webp" alt="logout" className="h-10 w-10"/>
            </div>
        </div>
    );
}

export default Profile;