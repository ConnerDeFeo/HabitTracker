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
            <div className="grid mx-auto text-6xl gap-5">
                <p>Username: {user.username}</p>
                <p>Date Joined: {user.dateCreated}</p>
                <Button label="Logout" onClick={logout} className="w-30 ml-auto"/>
            </div>
        }/>
    );
}

export default Profile;