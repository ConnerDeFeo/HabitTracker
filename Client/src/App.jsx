import React, { useState } from 'react';
import {
  BrowserRouter as Router,
  Routes,
  Route
} from 'react-router-dom';
import HabitService from './service/HabitService';


function App() {
  const [username,setUsername] = useState();

  const handleSubmit = async () => {
    
    const response = await HabitService.PostUser(username);
    console.log(response);
    
  };

  return (
    <Router>
      <Routes>
        <Route path={"/Home"} element={
          <>
            <textarea onChange={(e)=>setUsername(e.target.value)}/>
            <button onClick={handleSubmit}>Click me!</button>
          </>}/>
      </Routes>
    </Router>
  );  
}

export default App
