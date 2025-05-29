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
import DateInfo from './types/DateInfo';

const HabitTracker = ()=>{

    const [sessionUsername,setSessionUserName] = useState("");
    const [date, setDate] = useState<DateInfo>(() => {
        const now = new Date();
        return{
            year: now.getFullYear(),
            month: now.getMonth()+1, //0 indexed based
            day: now.getDate()
        };
    });

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
                <Route path='' element={sessionUsername=="" ? <HomePage/> : <HabitCheckList date={date}/>}/>
                <Route path='CreateAccount' element={<CreateAccount setSessionUsername={setSessionUserName}/>}/>
                <Route path='Login' element={<Login setSessionUsername={setSessionUserName}/>}/>
                <Route path='Profile' element={sessionUsername==""? <HomePage/> : <Profile sessionUsername={sessionUsername} setSessionUsername={setSessionUserName}/>}/>
                <Route path='Schedule' element={<Schedule setDate={setDate}/>}/>
            </Routes>
            <Footer/>
        </Router>
    );
}

export default HabitTracker;