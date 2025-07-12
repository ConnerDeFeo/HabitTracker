import { Link, useNavigate } from 'react-router-dom';
import Container from '../components/General/Container';
import Waiting from '../components/General/Waiting';
import { useState } from 'react';
import UserService from '../services/UserService';
import UserDto from '../types/UserDto';

//First page you seen when not loggged in
const Home = (props:{setUser:React.Dispatch<React.SetStateAction<UserDto | undefined>>})=>{
    const {setUser} = props;
    const fontStyling = 'font-hand text-center text-4xl md:text-5xl crossOut inline-block px-5 mx-auto ';
    const [waiting,setWaiting] = useState<boolean>(false);
    const navigate = useNavigate();

    //For a guest feature
    const guestLogin = async ()=>{
            setWaiting(true);
            let deviceId = localStorage.getItem("deviceId");
            if (!deviceId) {
                deviceId = crypto.randomUUID();
                localStorage.setItem("deviceId", deviceId);
            }
            const response = await UserService.Login("Guest","KaizenHabits",deviceId);
            setWaiting(false);
            const loginResult = await response.json();
            setUser(loginResult.user);
            navigate('/');
            
    }

    return(
        <Container content={
            <div className="grid justify-center mt-10 mb-5 gap-10">
                <h1 className='text-7xl md:text-9xl'>Kaizen Habits</h1>
                <Link to="/CreateAccount" className={fontStyling}>Create Account</Link>
                <Link to="/Login" className={fontStyling}>Login</Link>
                <p className={`cursor-pointer ${fontStyling}`} onClick={guestLogin}>Demo Account</p>
                {waiting && <Waiting/>}
            </div>
        }/>
    );
}

export default Home;