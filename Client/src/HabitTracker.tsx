import {
    BrowserRouter as Router,
    Routes,
    Route
} from 'react-router-dom';
import HomePage from './Home';
import CreateAccount from './CreateAccount';
import Login from './Login';
import Navbar from './components/Navbar';
import Footer from './components/Footer';
import Habits from './Habits';

const HabitTracker = ()=>{

    return(
        <Router>
            <Navbar/>
            <Routes>
                <Route path='/' element={localStorage.getItem("sessionKey")=="" ? <HomePage/> : <Habits/>}/>
                <Route path='CreateAccount' element={<CreateAccount/>}/>
                <Route path='Login' element={<Login/>}/>
            </Routes>
            <Footer/>
        </Router>
    );
}

export default HabitTracker;