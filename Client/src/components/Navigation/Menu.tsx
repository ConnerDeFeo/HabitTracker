import NavigationItem from "./NavigationItem";

//after clicking hamburger, this apears
const Menu = (props:{exitMenu:()=>void, loggedIn:boolean})=>{
    const {exitMenu, loggedIn} = props;

    return (
        <div className="w-[80%] mx-auto">
            <img src="/x.webp" className="h-10 w-10 cursor-pointer my-5" onClick={exitMenu}/>
            <div className="grid gap-y-10 w-[80%] mx-auto">
                {loggedIn ?
                    <>
                        <NavigationItem name="Tracker" imagePath="/checklist.webp" alt="checklist" navigateTo="/" onClick={exitMenu}/>
                        <NavigationItem name="MyHabits" imagePath="/Arrows.png" alt="MyHabits" navigateTo="/MyHabits" onClick={exitMenu}/>
                        <NavigationItem name="Schedule" imagePath="/Calender.png" alt="calender" navigateTo="/Schedule" onClick={exitMenu}/>
                        <NavigationItem name="Statistics" imagePath="/Statistics.png" alt="Statistics" navigateTo="/Statistics" onClick={exitMenu}/>
                        <NavigationItem name="Friends" imagePath="/Friends.png" alt="Friends" navigateTo="/Friends" onClick={exitMenu}/>
                        <NavigationItem name="About" navigateTo="/About" onClick={exitMenu}/>
                        <NavigationItem name="Contact" navigateTo="/Contact" onClick={exitMenu}/>
                    </>
                    :
                    <>
                        <NavigationItem name="Login"  navigateTo="/Login" onClick={exitMenu}/>
                        <NavigationItem name="Create Account" navigateTo="/CreateAccount" onClick={exitMenu}/>
                    </>
                }
            </div>
        </div>
    );
}

export default Menu;