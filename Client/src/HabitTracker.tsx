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
import UserService from './service/UserService';

const HabitTracker = ()=>{

    const [sessionUsername,setSessionUserName] = useState("");

    /**
     * Fetches user on load of application so that all relevant data can
     * immediately be updated
     */
    useEffect(()=>{
        const fetchUser = async()=>{
            const response = await UserService.GetUser();
            if(response.status==200){
                const user = await response.json();
                setSessionUserName(user.username);
            }
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
                <Route path='Profile' element={<Profile sessionUsername={sessionUsername} setSessionUsername={setSessionUserName}/>}/>
            </Routes>
            <Footer/>
        </Router>
    );
}

export default HabitTracker;