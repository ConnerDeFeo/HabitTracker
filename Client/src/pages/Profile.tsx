import { useNavigate } from "react-router-dom";
import Button from "../components/Button";
import Container from "../components/Container";
import UserService from "../services/UserService";

const Profile =(props:{sessionUsername: string; setSessionUsername: (username:string)=>void})=>{
    const navigate = useNavigate();
    const {sessionUsername,setSessionUsername} = props;

    const logout = async ()=>{
        await UserService.Logout();
        localStorage.setItem("loggedIn","false");
        setSessionUsername("");
        navigate('/');
    }

    return(
        <Container content={
            <div className="grid mx-auto text-6xl gap-5">
                <p>Username: {sessionUsername}</p>
                <Button label="Logout" onClick={logout}/>
            </div>
        }/>
    );
}

export default Profile;