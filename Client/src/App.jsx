import React from 'react';
import {
  BrowserRouter as Router,
  Routes,
  Route
} from 'react-router-dom';
import CreateAccount from './CreateAccount';


function App() {

  return (
    <Router>
      <Routes>
        <Route path={"/"} element={<CreateAccount/>}/>
        <Route path={"/Create"} element={<CreateAccount/>}/>
      </Routes>
    </Router>
  );  
}

export default App
