import { useState } from "react";
import TextArea from "./components/TextArea";
import Container from "./components/Container";
import UserService from "./service/UserService";
import Waiting from "./components/Waiting";

const CreateAccount = ()=>{

    const [waiting, setWaiting] = useState(false);

    const [username,setUsername] = useState("");
    const [password,setPassword] = useState("");
    const [confirmPassword,setConfirmPassword] = useState("");

    const onSubmit = async ()=>{
        if(password==confirmPassword){
            setWaiting(true);
            await UserService.PostUser(username,password);
            setWaiting(false);
        }else{
            alert("Passwords do not match!")
        }
    }

    return(
        <Container
            content={
                <div className={"flex flex-col justify-center w-[40%] mx-auto gap-8 mt-25"}>
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