import { useNavigate } from "react-router-dom";
import UserService from "../services/UserService";
import UserDto from "../types/UserDto";
import { useEffect, useState } from "react";
import EditUser from "../components/Profile/EditUser";
import ProfileHabit from "../types/ProfileHabit";
import RenderCurrentMonth from "../components/Profile/RenderCurrentMonth";
import CurrentHabits from "../components/Profile/CurrentHabits";
import ProfilePicture from "../components/General/ProfilePicture";
import PhotoService from "../services/PhotoService";

//Profile page the user sees
const Profile =(
    props:{
        user: UserDto, 
        setUser: React.Dispatch<React.SetStateAction<UserDto | undefined>>,
        updateUrl:()=>void
    }
)=>{
    const {user,setUser,updateUrl} = props;
    const navigate = useNavigate();

    const imageUrl = PhotoService.getPhotoUrl(user.id);
    const [modalOpen, setModalOpen] = useState<boolean>(false); //modal flag
    const [currentHabits, setCurrentHabits] = useState<ProfileHabit[]>([]);
    const [currentMonthHabitsCompleted, setCurrentMonthHabitsCompleted] = useState<Record<string,boolean>>({});

    //Fetches the users profile
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
        <div className="mt-15 sm:w-[85%] mx-auto">
            <div className="grid md:grid-cols-2 mx-auto">
                {/**Profile picture*/}
                <div className="grid justify-center md:flex md:justify-between items-center">
                    <ProfilePicture imageUrl={imageUrl} editAction={()=>setModalOpen(true)} height={40} width={40}/>
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
            <EditUser
                onClose={()=>setModalOpen(false)}
                hidden={!modalOpen}
                updateUrl={updateUrl}
                user = {user}
                setUser={setUser}

            />
        </div>
    );
}

export default Profile;