import React, { useState } from 'react';


function App() {
  const [username,setUsername] = useState();

  return (
    <>
      <textarea onChange={(e)=>setUsername(e.target.value)}/>
      <button onClick={()=>alert(username)}>Click me!</button>
    </>
  );  
}

export default App
