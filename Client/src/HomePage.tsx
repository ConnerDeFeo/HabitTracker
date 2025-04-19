import { Link } from 'react-router-dom';

const HomePage = ()=>{
    const fontStyling = 'font-hand text-center text-5xl crossOut inline-block px-5 mx-auto';
    return(
        <div className="flex flex-col justify-end min-h-[50vh] mb-[30vh]">
            <div className="grid justify-center mt-10 mb-5 gap-10">
                <h1 className='text-9xl h'>Habit Tracker</h1>
                <Link to="/CreateAccount" className={fontStyling}>Create Account</Link>
                <Link to="/Login" className={fontStyling}>Login</Link>
            </div>
        </div>
    );
}

export default HomePage;