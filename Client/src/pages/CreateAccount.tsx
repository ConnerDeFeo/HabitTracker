import { useState } from "react";
import Container from "../components/Container";
import UserService from "../services/UserService";
import Waiting from "../components/Waiting";
import { useNavigate } from "react-router-dom";
import Button from "../components/Button";
import Input from "../components/Input";
import UserDto from "../types/UserDto";

//Create account page
const CreateAccount = (props:{setUser: (user:UserDto)=>void})=>{
    const {setUser} = props;

    const navigate = useNavigate();

    //Determines if the loading screen shows
    const [waiting, setWaiting] = useState<boolean>(false);

    //Invisible character holding the place of where all messages are displayed
    const [message,setMessage] = useState<string>("\u00A0");

    const [username,setUsername] = useState<string>("");
    const [password,setPassword] = useState<string>("");
    const [confirmPassword,setConfirmPassword] = useState<string>("");

    //On submision of a new user being made
    const onSubmit = async ()=>{
        if(password!=confirmPassword){
            setMessage("Passwords do not match");
        }
        else if(username.trim()==""){
            setMessage("Enter Username");
        }
        else if(password.length<8){
            setMessage("Password: 8 Character long");
        }else{
            setWaiting(true);
            const response = await UserService.PostUser(username,password);
            setWaiting(false);
            if(response.status!=200){
                setMessage("Username Taken");
            }else{
                const loginResult = await response.json();
                sessionStorage.setItem("loggedIn","true");
                setUser(loginResult.user);
                navigate('/');
            }
        }
    }

    return(
        <Container
            content={
                <div className={"flex flex-col justify-center w-[60vw] max-w-115 mx-auto gap-8"}>
                    <div className="text-center text-red-600 text-2xl">{message}</div>
                    <Input title={"Username"} value={username} updateValue={setUsername} />
                    <Input title={"Password"} value={password} updateValue={setPassword} type="password"/>
                    <Input title={"Confirm Password"} value={confirmPassword} updateValue={setConfirmPassword} type="password"/>
                    <Button label="Create" onClick={onSubmit} className="ml-auto w-15 md:w-15"/>
                    {waiting && <Waiting/>}
                </div>
            }
        />
    );
}

export default CreateAccount;