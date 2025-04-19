import { useState } from "react";
import TextArea from "./components/TextArea";

const CreateAccount = ()=>{

    const [username,setUsername] = useState("");
    const [password,setPassword] = useState("");
    const [confirmPassword,setConfirmPassword] = useState("");

    return(
        <div className="flex flex-col justify-center w-[40%] mx-auto">
            <TextArea title={username} value={username} updateValue={setUsername} />
            <TextArea title={password} value={password} updateValue={setPassword} />
            <TextArea title={confirmPassword} value={confirmPassword} updateValue={setConfirmPassword} />
        </div>
    );
}

export default CreateAccount;