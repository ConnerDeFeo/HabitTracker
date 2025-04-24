import Button from "../components/Button";
import Container from "../components/Container";
import UserService from "../service/UserService";

const Profile =(props:{sessionUsername: string})=>{
    const {sessionUsername} = props;
    return(
        <Container content={
            <div className="grid mx-auto text-6xl border border-black gap-5">
                <p>Username: {sessionUsername}</p>
                <Button label="Logout" onClick={()=>UserService.Logout()}/>
            </div>
        }/>
    );
}

export default Profile;