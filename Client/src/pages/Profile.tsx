import { useNavigate } from "react-router-dom";
import Button from "../components/Button";
import Container from "../components/Container";
import UserService from "../services/UserService";
import UserDto from "../types/UserDto";

//Profile page the user sees
const Profile =(props:{user: UserDto; setUser: (user:UserDto)=>void})=>{
    const navigate = useNavigate();
    const {user,setUser} = props;

    const logout = async ()=>{
        await UserService.Logout();
        sessionStorage.setItem("loggedIn","false");
        setUser({username:"", dateCreated: "" });
        navigate('/');
    }

    return(
        <Container content={
            <div className="grid mx-auto text-4xl md:text-6xl gap-5">
                <div className="h-40 w-40 rounded-2xl bg-gray-200 mx-auto"/>
                <p>Username: {user.username}</p>
                <p>Date Joined: {user.dateCreated}</p>
                <Button label="Logout" onClick={logout} className="w-30 ml-auto"/>
            </div>
        }/>
    );
}

export default Profile;