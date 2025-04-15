import React, {useState} from 'react';
import HabitService from './service/HabitService';

const CreateAccount = ()=>{
    const [username,setUsername] = useState();

    const handleSubmit = async () => {
        
        const response = await HabitService.PostUser(username);
        console.log(response);
        
    };
    
    return(
        <>
            <textarea onChange={(e)=>setUsername(e.target.value)}/>
            <button onClick={handleSubmit}>Click me!</button>
        </>
      );
}

export default CreateAccount;