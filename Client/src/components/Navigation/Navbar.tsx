import { useNavigate } from "react-router-dom";
import NavigationItem from "./NavigationItem";

//Navigation bar for the site
const Navbar = (props:{displayMenu:()=>void, useHamburger:boolean})=>{
    const {displayMenu, useHamburger} = props;
    const navigate = useNavigate();
    const loggedIn:boolean = localStorage.getItem("loggedIn") ==="true";

    return (
        <div className="flex place-content-between border-b-4 border-black h-24 w-[85%] mx-auto pt-3 items-center">
            {useHamburger ? 
                <img src="/hamburger.png" className="w-8 h-8 cursor-pointer" onClick={displayMenu}/>
                : 
                <div className="flex gap-8">
                    <h1 className="font-hand text-5xl cursor-pointer crossOut" onClick={()=>navigate("/")}>Habit Tracker</h1>
                    {loggedIn && 
                        <>
                            <NavigationItem name="MyHabits" imagePath="/Arrows.png" alt="MyHabits" navigateTo="/MyHabits"/>
                            <NavigationItem name="Schedule" imagePath="/Calender.png" alt="calender" navigateTo="/Schedule"/>
                            <NavigationItem name="Statistics" imagePath="/Statistics.png" alt="Statistics" navigateTo="/Statistics"/>
                            <NavigationItem name="About" navigateTo="/About"/>
                            <NavigationItem name="Contact" navigateTo="/Contact"/>
                        </>
                    }
                </div>
            }
            <img src="/UserIcon.png" className="h-12 w-12 md:h-16 md:w-16 cursor-pointer" onClick={()=>navigate("/Profile")}/>
        </div>
    );
} 

export default Navbar;