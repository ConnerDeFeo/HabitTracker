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
import HabitHistoryService from './services/HabitHistoryService';
import MyHabits from './pages/MyHabits';
import UserDto from './types/UserDto';

const HabitTracker = ()=>{

    const [user,setUser] = useState<UserDto>({username:"",dateCreated:""});
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

        const resp = await HabitHistoryService.getMonth(yyyyMM);
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
                sessionStorage.setItem("loggedIn","true");
                setUser(user);
            }
            else
                sessionStorage.setItem("loggedIn","false");
        }
        fetchUser();
    },[]);

    useEffect(()=>{
        fetchMonth();
    },[date.month]);

    return(
        <Router >
            <Navbar/>
            <Routes>
                <Route path='' element={user.username==="" ? <HomePage/> : <HabitCheckList date={date} fetchMonth={fetchMonth} setDate={setDate}/>}/>
                <Route path='CreateAccount' element={<CreateAccount setUser={setUser}/>}/>
                <Route path='Login' element={<Login setUser={setUser}/>}/>
                <Route path='Profile' element={user.username==""? <HomePage/> : <Profile user={user} setUser={setUser}/>}/>
                <Route path='Schedule' element={<Schedule setDate={setDate} monthlyHabits={monthlyHabits} date={date}/>}/>
                <Route path='MyHabits' element={<MyHabits fetchMonth={fetchMonth}/>}/>
            </Routes>
            <Footer/>
        </Router>
    );
}

export default HabitTracker;