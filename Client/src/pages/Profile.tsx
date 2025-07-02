import { useNavigate } from "react-router-dom";
import UserService from "../services/UserService";
import UserDto from "../types/UserDto";
import { useEffect, useState } from "react";
import AddProfilePic from "../components/Profile/AddProfilePic";
import ProfileHabit from "../types/ProfileHabit";

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

    //Logout button pressed
    const logout = async ()=>{
        await UserService.Logout();
        setUser(undefined);
        navigate('/');
    }

    return(
        <div className="grid mt-10">
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
            <div className="grid w-75 mx-auto">
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
            <div>

            </div>
            
            {/**Modal*/}
            {modalOpen && 
                <AddProfilePic
                    onClose={()=>setModalOpen(false)}
                    setImageUrl={setImageUrl}
                />
            }
        </div>
    );
}

export default Profile;