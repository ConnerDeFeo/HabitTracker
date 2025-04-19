import { useNavigate } from "react-router-dom";

const Navbar = ()=>{
    const navigate = useNavigate();

    return(
        <div className="flex place-content-between border-b-4 border-black h-24 w-[85%] mx-auto pt-3 items-center">
            <div onClick={()=>{navigate("/")}} className="cursor-pointer">
                <h1 className="font-hand text-5xl">Habit Tracker</h1>
            </div>
            <img src="./UserIcon.png" className="h-15 w-15"/>
        </div>
    );
} 

export default Navbar;