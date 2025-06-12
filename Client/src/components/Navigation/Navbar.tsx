import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import NavigationItem from "./NavigationItem";

//Navigation bar for the site
const Navbar = (props:{displayMenu:()=>void})=>{
    const {displayMenu} = props;
    const navigate = useNavigate();
    const [useHamburger,setUseHamburger] = useState<boolean>(window.innerWidth<768);
    const loggedIn:boolean = sessionStorage.getItem("loggedIn") ==="true";

    //Hamburger menu used instead if teh screen is smaller than medium
    useEffect(() => {
        //Any time the screen is changed in width, this is called
        const handleResize = () => {
            setUseHamburger(window.innerWidth<768);
        };

        window.addEventListener("resize", handleResize);
        return () => window.removeEventListener("resize", handleResize);
    }, []);


    return (
        <div className="flex place-content-between border-b-4 border-black h-24 w-[85%] mx-auto pt-3 items-center">
            {useHamburger ? 
                <img src="./hamburger.png" className="w-8 h-8 cursor-pointer" onClick={displayMenu}/>
                : 
                <div className="flex gap-8">
                    <h1 className="font-hand text-5xl cursor-pointer crossOut" onClick={()=>navigate("/")}>Habit Tracker</h1>
                    {loggedIn && 
                        <>
                            <NavigationItem name="MyHabits" imagePath="Arrows.png" alt="MyHabits" navigateTo="/MyHabits"/>
                            <NavigationItem name="Schedule" imagePath="Calender.png" alt="calender" navigateTo="/Schedule"/>
                            <NavigationItem name="Statistics" imagePath="Statistics.png" alt="Statistics" navigateTo="/Statistics"/>
                        </>
                    }
                </div>
            }
            <img src="./UserIcon.png" className="h-12 w-12 md:h-16 md:w-16 cursor-pointer" onClick={()=>navigate("/Profile")}/>
        </div>
    );
} 

export default Navbar;