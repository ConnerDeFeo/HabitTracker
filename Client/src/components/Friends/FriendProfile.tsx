import UserDto from "../../types/UserDto";
import ProfileHabit from "../../types/ProfileHabit";
import ProfilePicture from "../Profile/ProfilePicture";
import { useEffect, useState } from "react";
import PhotoService from "../../services/PhotoService";
import CurrentHabits from "../Profile/CurrentHabits";
import RenderCurrentMonth from "../Profile/RenderCurrentMonth";

//Profile page the user sees
const FriendProfile =(props:{user: UserDto,currentHabits:ProfileHabit[],currentMonthHabitsCompleted:Record<string,boolean> })=>{
    const {user, currentHabits,currentMonthHabitsCompleted} = props;
    const [validImgUrl, setValidImgUrl] = useState<boolean>(false);
    const imgUrl = `https://habit-tracker-photos.s3.amazonaws.com/profilePhotos/${user.id}`;
    useEffect(()=>{
        const validate = async ()=>{
            if(await PhotoService.validateImageUrl(imgUrl))
                setValidImgUrl(true);
        }
        validate();
    },[]);

    return(
        <div className="my-15 lg:w-[85%] mx-auto">
            <div className="grid md:grid-cols-2 w-[85%] mx-auto">
                {/**Profile picture*/}
                <div className="grid justify-center md:flex md:justify-between items-center">
                    <ProfilePicture imageUrl={validImgUrl ? imgUrl : "UserIcon.png"} />
                    <p className={`${user.username.length > 12 ? "text-2xl" : "text-4xl"} lg:text-4xl text-center my-5`}>{user.username}</p>
                </div>
                {/**Current Habits*/}
                <CurrentHabits currentHabits={currentHabits}/>
                {/**Calender displayed */}
                <RenderCurrentMonth currentMonthHabitsCompleted={currentMonthHabitsCompleted}/>
            </div>

            <div className="md:flex w-[85%] mx-auto items-center my-10">
                <p className="text-4xl text-center my-5">Member Sense: {user.dateCreated}</p>
            </div>
        </div>
    );
}

export default FriendProfile;