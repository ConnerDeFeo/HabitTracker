import React from 'react';
import { Link } from 'react-router-dom';

const HomePage = ()=>{
    return(
        <>
            <h1>Habit Tracker</h1>
            <Link to={"/Create"}>Create Account</Link><br/>
            <Link to={"/Login"}>Login</Link>
        </>
    );
}

export default HomePage;