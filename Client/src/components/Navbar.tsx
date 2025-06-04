import { useNavigate } from "react-router-dom";

const Navbar = ()=>{
    const navigate = useNavigate();

    const navbarItem = (name:string, imagePath:string, alt:string, navigateTo:string)=>{
        return (
            <div className="cursor-pointer mt-auto flex gap-2 crossOut" onClick={()=>navigate(navigateTo)}>
                <p className="text-4xl">{name}</p>
                <img src={imagePath} alt={alt} className="w-8 h-8 my-auto"/>
            </div>
        );
    }

    return(
        <div className="flex place-content-between border-b-4 border-black h-24 w-[85%] mx-auto pt-3 items-center">
            <div className="flex gap-8">
                <h1 className="font-hand text-5xl cursor-pointer crossOut" onClick={()=>navigate("/")}>Habit Tracker</h1>
                {localStorage.getItem("loggedIn") ==="true" && 
                    <>
                        {navbarItem("MyHabits","Arrows.png","MyHabits","/MyHabits")}
                        {navbarItem("Schedule","Calender.png","calender","/Schedule")}
                    </>
                }
            </div>
            <img src="./UserIcon.png" className="h-18 w-18 cursor-pointer" onClick={()=>navigate("/Profile")}/>
        </div>
    );
} 

export default Navbar;