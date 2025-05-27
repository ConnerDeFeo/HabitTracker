import { useState } from "react";
import Container from "../components/Container";
import UserService from "../services/UserService";
import Waiting from "../components/Waiting";
import { useNavigate } from "react-router-dom";
import Button from "../components/Button";
import Input from "../components/Input";

const CreateAccount = (props:{setSessionUsername: (sessionUsername:string)=>void})=>{
    const {setSessionUsername} = props;

    const navigate = useNavigate();

    //Determines if the loading screen shows
    const [waiting, setWaiting] = useState<boolean>(false);

    //Invisible character holding the place of where all messages are displayed
    const [message,setMessage] = useState<string>("\u00A0");

    const [username,setUsername] = useState<string>("");
    const [password,setPassword] = useState<string>("");
    const [confirmPassword,setConfirmPassword] = useState<string>("");

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
                const data = await response.json();
                //store session token so that user does not have to log in
                document.cookie = "sessionKey="+data.sessionKey;
                setSessionUsername(username);
                navigate('/');
            }
        }
    }

    return(
        <Container
            content={
                <div className={"flex flex-col justify-center w-[40rem] mx-auto gap-8 mt-18"}>
                    <div className="text-center text-red-600 text-2xl">{message}</div>
                    <Input title={"Username"} value={username} updateValue={setUsername} />
                    <Input title={"Password"} value={password} updateValue={setPassword} type="password"/>
                    <Input title={"Confirm Password"} value={confirmPassword} updateValue={setConfirmPassword} type="password"/>
                    <Button label="Create" onClick={onSubmit}/>
                    {waiting && <Waiting/>}
                </div>
            }
        />
    );
}

export default CreateAccount;