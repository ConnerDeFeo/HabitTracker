import React, {useState} from 'react';
import UserService from './service/UserService';

const CreateAccount = ()=>{
    const [username,setUsername] = useState();
    const [password, setPassword] = useState();

    const handleSubmit = async () => {
        
        const response = await UserService.PostUser(username,password);
        if(response.status==409){
            alert("Username Taken");
        }
        
    };
    
    return(
        <>
            <p>username</p>
            <textarea onChange={(e)=>setUsername(e.target.value)}/>
            <p>password</p>
            <textarea onChange={(e)=>setPassword(e.target.value)}/>
            <button onClick={handleSubmit}>Click me!</button>
        </>
      );
}

export default CreateAccount;