import React, { useState } from 'react';
import {
  BrowserRouter as Router,
  Routes,
  Route
} from 'react-router-dom';


function App() {
  const [username,setUsername] = useState();

  return (
    <Router>
      <Routes>
        <Route path={"/Home"} element={
          <>
            <textarea onChange={(e)=>setUsername(e.target.value)}/>
            <button onClick={()=>alert(username)}>Click me!</button>
          </>}/>
      </Routes>
    </Router>
  );  
}

export default App
