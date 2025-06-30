import { useState } from "react";
import Button from "../components/General/Button";
import Container from "../components/General/Container";
import Input from "../components/General/Input";
import Waiting from "../components/General/Waiting";
import UserService from "../services/UserService";
import { useNavigate } from "react-router-dom";
import UserDto from "../types/UserDto";

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
            const response = await UserService.Login(username,password);
            setWaiting(false);
            if(response.status!=200){
                setMessage("Invalid Username or Password");
            }else{
                const loginResult = await response.json();
                sessionStorage.setItem("loggedIn","true");
                setUser(loginResult.user);
                navigate('/');
            }
    }

    return(
        <Container content={
            <div className={"flex flex-col justify-center w-[85%] max-w-115 mx-auto gap-8 mt-2"}>
                    <div className="text-center text-red-600 text-2xl">{message}</div>
                    <Input title={"Username"} value={username} updateValue={setUsername} />
                    <Input title={"Password"} value={password} updateValue={setPassword} type="password"/>
                    <Button label="Login" onClick={onSubmit} className="ml-auto w-30"/>
                    {waiting && <Waiting/>}
            </div>
        }/>
    );
}

export default Login;