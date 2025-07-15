import ProfilePicture from "../components/General/ProfilePicture";
import { useEffect, useState } from "react";
import CurrentHabits from "../components/Profile/CurrentHabits";
import RenderCurrentMonth from "../components/Profile/RenderCurrentMonth";
import Profile from "../types/Profile";
import Waiting from "../components/General/Waiting";
import { useParams } from "react-router-dom";
import SocialDataService from "../services/SocialDataService";
import PhotoService from "../services/PhotoService";

//Friend profile page the user sees
const FriendProfile =()=>{
    const {username} = useParams(); //from url
    const [profile,setProfile] = useState<Profile | undefined>();
    const imgUrl = PhotoService.getPhotoUrl(profile?.id);

    //fetch profile
    useEffect(()=>{
        const fetchProfile = async ()=>{
            if(username){
                const resp = await SocialDataService.getProfile(username);
                if(resp.ok){
                    const profile:Profile = await resp.json();
                    setProfile(profile);
                }
            }
        }
        fetchProfile();
    },[username]);

    if(!profile)
        return <Waiting/>

    return(
        <div className="mt-15 sm:w-[85%] mx-auto">
            <div className="grid md:grid-cols-2 mx-auto">
                {/**Profile picture*/}
                <div className="grid justify-center md:flex md:justify-between items-center">
                    <ProfilePicture imageUrl={imgUrl} height={40} width={40}/>
                    <p className={`${profile.username.length > 12 ? "text-2xl" : "text-4xl"} lg:text-4xl text-center my-5`}>{profile.username}</p>
                </div>
                {/**Current Habits*/}
                <CurrentHabits currentHabits={profile.currentHabits}/>
                {/**Calender displayed */}
                <RenderCurrentMonth currentMonthHabitsCompleted={profile.currentMonthHabitsCompleted}/>
            </div>

            <div className="md:flex w-[85%] mx-auto items-center my-10">
                <p className="text-4xl text-center my-5">Member Sense: {profile.dateCreated}</p>
            </div>
        </div>
    );
}

export default FriendProfile;