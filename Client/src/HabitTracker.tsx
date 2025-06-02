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
import HistoricalDate from './types/HistoricalDate';
import HabitService from './services/HabitService';

const HabitTracker = ()=>{

    const [sessionUsername,setSessionUserName] = useState("");
    const [monthlyHabits, setMonthlyHabits] = useState<Record<string,HistoricalDate>>();
    const [date, setDate] = useState<DateInfo>(() => {
        const now = new Date();
        return{
            year: now.getFullYear(),
            month: now.getMonth(), 
            day: now.getDate()
        };
    });


    //Re-renders the month
    const fetchMonth = async ()=>{
        setMonthlyHabits({});
        const yyyyMM: string = `${date.year}-${String(date.month+1).padStart(2,'0')}`;

        const resp = await HabitService.getMonth(yyyyMM);
        if(resp.status==200){
            const habits = await resp.json();
            setMonthlyHabits(habits);
        }
    }

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

    useEffect(()=>{
        fetchMonth();
    },[date.month]);

    return(
        <Router >
            <Navbar/>
            <Routes>
                <Route path='' element={sessionUsername=="" ? <HomePage/> : <HabitCheckList date={date} fetchMonth={fetchMonth}/>}/>
                <Route path='CreateAccount' element={<CreateAccount setSessionUsername={setSessionUserName}/>}/>
                <Route path='Login' element={<Login setSessionUsername={setSessionUserName}/>}/>
                <Route path='Profile' element={sessionUsername==""? <HomePage/> : <Profile sessionUsername={sessionUsername} setSessionUsername={setSessionUserName}/>}/>
                <Route path='Schedule' element={<Schedule setDate={setDate} monthlyHabits={monthlyHabits} date={date}/>}/>
            </Routes>
            <Footer/>
        </Router>
    );
}

export default HabitTracker;