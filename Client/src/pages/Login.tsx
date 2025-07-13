import { useState } from "react";
import Button from "../components/General/Button";
import Container from "../components/General/Container";
import Input from "../components/General/Input";
import Waiting from "../components/General/Waiting";
import UserService from "../services/UserService";
import { useNavigate } from "react-router-dom";
import UserDto from "../types/UserDto";
import { CredentialResponse, GoogleLogin } from "@react-oauth/google";
import { jwtDecode } from "jwt-decode";
import AuthUtils from "../services/AuthUtils";
import GoogleAuthService from "../services/GoogleAuthService";

//Login page
const Login = (props: {setUser: (user:UserDto)=>void})=>{

    const navigate = useNavigate();

    const {setUser} = props;

    //Determines if loading screen pops up
    const [waiting,setWaiting] = useState(false);

    //Invisible character holding the place of where all messages are displayed
    const [message,setMessage] = useState("\u00A0");

    const [username,setUsername] = useState("");
    const [password,setPassword] = useState("");

    //When user attempt login
    const onSubmit = async ()=>{
            setWaiting(true);
            const deviceId = AuthUtils.getDeviceId();
            const response = await UserService.Login(username,password,deviceId);
            setWaiting(false);
            if(response.status!=200){
                setMessage("Invalid Username or Password");
            }else{
                const user:UserDto = await response.json();
                setUser(user);
                navigate('/');
            }
    }

    const onGoogleSuccess = async (credential:CredentialResponse)=>{
        const deviceId = AuthUtils.getDeviceId();
        const resp = await GoogleAuthService.login(credential.credential!,deviceId);
        if(resp.ok){
            const user:UserDto = await resp.json();
            setUser(user);
            navigate('/');
        }

    }

    return(
        <Container content={
            <div className={"flex flex-col justify-center w-[85%] max-w-115 mx-auto gap-8 mt-2"}>
                <div className="text-center text-red-600 text-2xl">{message}</div>
                <div className="mx-auto">
                    <GoogleLogin onSuccess={(credendtials)=>onGoogleSuccess(credendtials)} onError={()=>setMessage("Unable to sign in")}/>
                </div>
                <Input title={"Username"} value={username} updateValue={setUsername} />
                <Input title={"Password"} value={password} updateValue={setPassword} type="password"/>
                <Button label="Login" onClick={onSubmit} className="ml-auto w-30"/>
                {waiting && <Waiting/>}
            </div>
        }/>
    );
}

export default Login;