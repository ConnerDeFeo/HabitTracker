import React from 'react';
import {
  BrowserRouter as Router,
  Routes,
  Route
} from 'react-router-dom';
import CreateAccount from './CreateAccount';
import HomePage from './HomePage';
import Login from './Login';


function App() {

  return (
    <Router>
      <Routes>
        <Route path={"/"} element={<HomePage/>}/>
        <Route path={"/Create"} element={<CreateAccount/>}/>
        <Route path={"/Login"} element={<Login/>}/>
      </Routes>
    </Router>
  );  
}

export default App
