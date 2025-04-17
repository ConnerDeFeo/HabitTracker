import { Link } from 'react-router-dom';

const HomePage = ()=>{
    return(
        <>
            <h1>HomePage</h1>
            <Link to={"/CreateAccount"}>Create Account</Link><br/>
            <Link to={"/Login"}>Login</Link>
        </>
    );
}

export default HomePage;