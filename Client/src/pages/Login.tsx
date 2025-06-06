import { useState } from "react";
import Button from "../components/Button";
import Container from "../components/Container";
import Input from "../components/Input";
import Waiting from "../components/Waiting";
import UserService from "../services/UserService";
import { useNavigate } from "react-router-dom";
import UserDto from "../types/UserDto";

const Login = (props: {setUser: (user:UserDto)=>void})=>{

    const navigate = useNavigate();

    const {setUser} = props;

    //Determines if loading screen pops up
    const [waiting,setWaiting] = useState(false);

    //Invisible character holding the place of where all messages are displayed
    const [message,setMessage] = useState("\u00A0");

    const [username,setUsername] = useState("");
    const [password,setPassword] = useState("");

    const onSubmit = async ()=>{

            setWaiting(true);
            const response = await UserService.Login(username,password);
            setWaiting(false);
            if(response.status!=200){
                setMessage("Invalid Username or Password");
            }else{
                const loginResult = await response.json();
                document.cookie = "sessionKey="+loginResult.sessionKey;
                sessionStorage.setItem("loggedIn","true");
                setUser(loginResult.User);
                navigate('/');
            }
    }

    return(
        <Container content={
            <div className={"flex flex-col justify-center w-[40rem] mx-auto gap-8 mt-2"}>
                    <div className="text-center text-red-600 text-2xl">{message}</div>
                    <Input title={"Username"} value={username} updateValue={setUsername} />
                    <Input title={"Password"} value={password} updateValue={setPassword} type="password"/>
                    <Button label="Login" onClick={onSubmit}/>
                    {waiting && <Waiting/>}
            </div>
        }/>
    );
}

export default Login;