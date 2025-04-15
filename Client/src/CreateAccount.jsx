import React, {useState} from 'react';
import HabitService from './service/HabitService';

const CreateAccount = ()=>{
    const [username,setUsername] = useState();
    const [password, setPassword] = useState();

    const handleSubmit = async () => {
        
        const response = await HabitService.PostUser(username,password);
        if(response.status!=200){
            alert("Invalid username");
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