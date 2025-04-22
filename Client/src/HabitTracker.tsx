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
import Habits from './pages/Habits';
import Profile from './pages/Profile';
import { useEffect } from 'react';
import UserService from './service/UserService';

const HabitTracker = ()=>{

    useEffect(()=>{
        const fetchUser = async()=>{
            
        }

        fetchUser();
    },[])

    return(
        <Router>
            <Navbar/>
            <Routes>
                <Route path='' element={localStorage.getItem("sessionKey")=="" ? <HomePage/> : <Habits/>}/>
                <Route path='CreateAccount' element={<CreateAccount/>}/>
                <Route path='Login' element={<Login/>}/>
                <Route path='Profile' element={<Profile/>}/>
            </Routes>
            <Footer/>
        </Router>
    );
}

export default HabitTracker;