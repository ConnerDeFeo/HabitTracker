import { Link } from 'react-router-dom';
import Container from '../components/General/Container';

//First page you seen when not loggged in
const Home = ()=>{
    const fontStyling = 'font-hand text-center text-4xl md:text-5xl crossOut inline-block px-5 mx-auto ';
    return(
        <Container content={
            <div className="grid justify-center mt-10 mb-5 gap-10">
                <h1 className='text-7xl md:text-9xl'>Kaizen Habits</h1>
                <Link to="/CreateAccount" className={fontStyling}>Create Account</Link>
                <Link to="/Login" className={fontStyling}>Login</Link>
            </div>
        }/>
    );
}

export default Home;