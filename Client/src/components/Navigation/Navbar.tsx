import NavigationItem from "./NavigationItem";

const Navbar = (props:{loggedIn:boolean})=>{
    const {loggedIn} = props;
    return loggedIn? 
        <>
            <NavigationItem name="Tracker" imagePath="/checklist.webp" alt="checklist" navigateTo="/"/>
            <NavigationItem name="MyHabits" imagePath="/Arrows.png" alt="MyHabits" navigateTo="/MyHabits"/>
            <NavigationItem name="Schedule" imagePath="/Calender.png" alt="calender" navigateTo="/Schedule"/>
            <NavigationItem name="Statistics" imagePath="/Statistics.png" alt="Statistics" navigateTo="/Statistics"/>
            <NavigationItem name="Friends" imagePath="/Friends.png" alt="Friends" navigateTo="/Friends"/>
            <NavigationItem name="About" navigateTo="/About"/>
            <NavigationItem name="Contact" navigateTo="/Contact"/>
        </>
        :
        <>
            <NavigationItem name="Create Account" navigateTo="/CreateAccount"/>
            <NavigationItem name="Login" navigateTo="/Login"/>
        </>;
}

export default Navbar;