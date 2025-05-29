import {
    BrowserRouter as Router,
    Routes,
    Route
} from 'react-router-dom';
import HomePage from './pages/Home';
import CreateAccount from './pages/CreateAccount';
import Login from './pages/Login';
import Navbar from './components/Navbar';
import Footer from './components/Footer';
import HabitCheckList from './pages/HabitCheckList';
import Profile from './pages/Profile';
import { useEffect, useState } from 'react';
import UserService from './services/UserService';
import Schedule from './pages/Schedule';

const HabitTracker = ()=>{

    const [sessionUsername,setSessionUserName] = useState("");
    const year = new Date().getFullYear();
    const month = new Date().getMonth();
    const today = new Date().getDate();

    /**
     * Fetches user on load of application so that all relevant data can
     * immediately be updated
     */
    useEffect(()=>{
        const fetchUser = async()=>{
            const response = await UserService.GetUser();
            if(response.status==200){
                const user = await response.json();
                localStorage.setItem("loggedIn","true");
                setSessionUserName(user.username);
            }
            else
                localStorage.setItem("loggedIn","false");
        }
        fetchUser();
    },[])

    return(
        <Router >
            <Navbar/>
            <Routes>
                <Route path='' element={sessionUsername=="" ? <HomePage/> : <HabitCheckList/>}/>
                <Route path='CreateAccount' element={<CreateAccount setSessionUsername={setSessionUserName}/>}/>
                <Route path='Login' element={<Login setSessionUsername={setSessionUserName}/>}/>
                <Route path='Profile' element={sessionUsername==""? <HomePage/> : <Profile sessionUsername={sessionUsername} setSessionUsername={setSessionUserName}/>}/>
                <Route path='Schedule' element={<Schedule/>}/>
            </Routes>
            <Footer/>
        </Router>
    );
}

export default HabitTracker;