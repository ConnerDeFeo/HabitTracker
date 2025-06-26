import {
    BrowserRouter as Router,
    Routes,
    Route
} from 'react-router-dom';
import HomePage from './pages/Home';
import CreateAccount from './pages/CreateAccount';
import Login from './pages/Login';
import Navbar from './components/Navigation/Navbar';
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
import Statistics from './pages/Statistics';
import Menu from './components/Navigation/Menu';
import About from './pages/About';
import Contact from './pages/Contact';

//Overarching application
const HabitTracker = ()=>{
    //Current user being held
    const [user,setUser] = useState<UserDto>({username:"",dateCreated:""});
    //Current monthly habits being viewed
    const [monthlyHabits, setMonthlyHabits] = useState<Record<string,HistoricalDate>>();
    //Flag for showing the menu after clicking hamburger
    const [displayMenu, setDisplayMenu] = useState<boolean>(false);
    const [smallScreen,setSmallScreen] = useState<boolean>(window.innerWidth<768);
    const [date, setDate] = useState<DateInfo>(() => {
        const now = new Date();
        return{
            year: now.getFullYear(),
            month: now.getMonth(), 
            day: now.getDate()
        };
    });

    //Hamburger menu used instead if teh screen is smaller than medium
    useEffect(() => {
        //Any time the screen is changed in width, this is called
        const handleResize = () => {
            setSmallScreen(window.innerWidth<768);
        };

        window.addEventListener("resize", handleResize);
        return () => window.removeEventListener("resize", handleResize);
    }, []);

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

    //Anytime month changes, update the current monthly habits
    useEffect(()=>{
        fetchMonth();
    },[date.month]);

    return (
        <Router >
            {displayMenu ?
                //menu should be displayed regardless of path when necessary 
                <Menu exitMenu={()=>setDisplayMenu(false)}/> : 
                <>
                    <Navbar displayMenu={()=>setDisplayMenu(true)} useHamburger={smallScreen}/>
                    <Routes>
                        <Route path='' element={user.username==="" ? <HomePage/> : <HabitCheckList date={date} fetchMonth={fetchMonth} setDate={setDate}/>}/>
                        <Route path='CreateAccount' element={<CreateAccount setUser={setUser}/>}/>
                        <Route path='Login' element={<Login setUser={setUser}/>}/>
                        <Route path='Profile' element={user.username==""? <HomePage/> : <Profile user={user} setUser={setUser}/>}/>
                        <Route path='Schedule' element={<Schedule setDate={setDate} monthlyHabits={monthlyHabits} date={date}/>}/>
                        <Route path='MyHabits' element={<MyHabits fetchMonth={fetchMonth}/>}/>
                        <Route path='Statistics' element={<Statistics smallScreen={smallScreen}/>}/>
                        <Route path='About' element={<About/>}/>
                        <Route path='Contact' element={<Contact/>}/>
                    </Routes>
                    <Footer/>
                </>
            }
        </Router>
    );
}

export default HabitTracker;