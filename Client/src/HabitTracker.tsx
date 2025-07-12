import {
    BrowserRouter as Router,
    Routes,
    Route
} from 'react-router-dom';
import HomePage from './pages/Home';
import CreateAccount from './pages/CreateAccount';
import Login from './pages/Login';
import Navbar from './components/Navigation/Navigation';
import Footer from './components/General/Footer';
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
import Friends from './pages/Friends';
import FriendProfile from './pages/FriendProfile';
import PhotoService from './services/PhotoService';

//Overarching application
const HabitTracker = ()=>{
    //Current user being held
    const [user,setUser] = useState<UserDto | undefined >();
    //Current monthly habits being viewed
    const [monthlyHabits, setMonthlyHabits] = useState<Record<string,HistoricalDate>>();
    //Flag for showing the menu after clicking hamburger
    const [displayMenu, setDisplayMenu] = useState<boolean>(false);
    const [imageUrl,setImageUrl] = useState<string>(PhotoService.getPhotoUrl(user?.id));
    const [smallScreen,setSmallScreen] = useState<boolean>(window.innerWidth<1024);
    const [date, setDate] = useState<DateInfo>(() => {
        const now = new Date();
        return{
            year: now.getFullYear(),
            month: now.getMonth(), 
            day: now.getDate()
        };
    });


    useEffect(()=>{
        setImageUrl(PhotoService.getPhotoUrl(user?.id));
    },[user?.id])

    //Hamburger menu used instead if teh screen is smaller than medium
    useEffect(() => {
        //Any time the screen is changed in width, this is called
        const handleResize = () => {
            setSmallScreen(window.innerWidth<1024);
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

    const fetchUser = async()=>{
        const response = await UserService.GetUser();
        if(response.status==200){
            const user: UserDto = await response.json();
            setUser(user);
        }
    }

    /**
     * Fetches user on load of application so that all relevant data can
     * immediately be updated
     */
    useEffect(()=>{
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
                <Menu exitMenu={()=>setDisplayMenu(false)} loggedIn={user!==undefined} /> : 
                <>
                    <Navbar displayMenu={()=>setDisplayMenu(true)} useHamburger={smallScreen} loggedIn={user!==undefined} imageUrl={imageUrl}/>
                    <div className='min-h-[85vh] flex flex-col'>
                        <Routes>
                            <Route path='' element={user ? <HabitCheckList date={date} fetchMonth={fetchMonth} setDate={setDate}/> : <HomePage setUser={setUser}/>}/>
                            <Route path='CreateAccount' element={<CreateAccount setUser={setUser}/>}/>
                            <Route path='Login' element={<Login setUser={setUser}/>}/>
                            <Route path='Profile' element={user ? <Profile user={user} setUser={setUser} updateUrl={()=>setImageUrl(PhotoService.getPhotoUrl(user.id))}/> : <HomePage setUser={setUser}/> }/>
                            <Route path='@/:username' element={<FriendProfile/> }/>
                            <Route path='Schedule' element={<Schedule setDate={setDate} monthlyHabits={monthlyHabits} date={date}/>}/>
                            <Route path='MyHabits' element={<MyHabits fetchMonth={fetchMonth} username={user?.username || ""}/>}/>
                            <Route path='Statistics' element={<Statistics smallScreen={smallScreen}/>}/>
                            <Route path='Friends' element={<Friends user={user} fetchUser={fetchUser}/>}/>
                            <Route path='About' element={<About/>}/>
                            <Route path='Contact' element={<Contact/>}/>
                        </Routes>
                        <Footer/>
                    </div>
                </>
            }
        </Router>
    );
}

export default HabitTracker;