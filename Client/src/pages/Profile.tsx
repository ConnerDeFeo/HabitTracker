import { useNavigate } from "react-router-dom";
import UserService from "../services/UserService";
import UserDto from "../types/UserDto";
import { useEffect, useState } from "react";
import AddProfilePic from "../components/Profile/AddProfilePic";
import ProfileHabit from "../types/ProfileHabit";
import RenderCurrentMonth from "../components/Profile/RenderCurrentMonth";
import CurrentHabits from "../components/Profile/CurrentHabits";
import ProfilePicture from "../components/Profile/ProfilePicture";

//Profile page the user sees
const Profile =(props:{user: UserDto, setUser: (user?:UserDto)=>void})=>{
    const {user,setUser} = props;
    const navigate = useNavigate();

    const [imageUrl, setImageUrl] = useState<string>(`https://habit-tracker-photos.s3.amazonaws.com/profilePhotos/${user.id}`);
    const [modalOpen, setModalOpen] = useState<boolean>(false);
    const [currentHabits, setCurrentHabits] = useState<ProfileHabit[]>([]);
    const [currentMonthHabitsCompleted, setCurrentMonthHabitsCompleted] = useState<Record<string,boolean>>({});


    useEffect(()=>{
        const fetchprofile = async ()=>{
            const resp = await UserService.GetProfile();
            if(resp.status==200){
                const profile = await resp.json();
                setCurrentHabits(profile.currentHabits);
                setCurrentMonthHabitsCompleted(profile.currentMonthHabitsCompleted);
            }
        }
        fetchprofile();
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
                    <ProfilePicture imageUrl={imageUrl} editAction={()=>setModalOpen(true)}/>
                    <p className={`${user.username.length > 12 ? "text-2xl" : "text-4xl"} lg:text-4xl text-center my-5`}>{user.username}</p>
                </div>
                {/**Current Habits*/}
                <CurrentHabits currentHabits={currentHabits}/>
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