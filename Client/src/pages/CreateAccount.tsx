import { useState } from "react";
import TextArea from "../components/TextArea";
import Container from "../components/Container";
import UserService from "../service/UserService";
import Waiting from "../components/Waiting";
import { useNavigate } from "react-router-dom";

const CreateAccount = ()=>{

    const navigate = useNavigate();

    const [waiting, setWaiting] = useState(false);

    const [message,setMessage] = useState("\u00A0");

    const [username,setUsername] = useState("");
    const [password,setPassword] = useState("");
    const [confirmPassword,setConfirmPassword] = useState("");

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
                //store session token so that user does not have to log in
                await response.json().then(data=> {localStorage.setItem("sessionKey", data.token);});
                navigate('/');
            }
        }
    }

    return(
        <Container
            content={
                <div className={"flex flex-col justify-center w-[40rem] mx-auto gap-8 mt-18"}>
                    <div className="text-center text-red-600 text-2xl">{message}</div>
                    <TextArea title={"Username"} value={username} updateValue={setUsername} />
                    <TextArea title={"Password"} value={password} updateValue={setPassword} />
                    <TextArea title={"Confirm Password"} value={confirmPassword} updateValue={setConfirmPassword} />
                    <button onClick={onSubmit}
                    className="color-black border border-black w-[20%] ml-auto font-hand text-4xl bg-black text-white rounded-2xl cursor-pointer"
                    >Create</button>
                    {waiting && <Waiting/>}
                </div>
            }
        />
    );
}

export default CreateAccount;