import { useNavigate } from "react-router-dom";

const Navbar = ()=>{
    const navigate = useNavigate();

    return(
        <div className="flex place-content-between border-b-4 border-black h-24 w-[85%] mx-auto pt-3 items-center">
            <div className="flex border border-black gap-8">
                <h1 className="font-hand text-5xl cursor-pointer crossOut" onClick={()=>navigate("/")}>Habit Tracker</h1>
                <div className="cursor-pointer mt-auto flex gap-2 crossOut" onClick={()=>navigate("/Schedule")}>
                    <p className="text-4xl">Schedule</p>
                    <img src="Calender.png" alt="calender" className="w-8 h-8 my-auto"/>
                </div>
            </div>
            <img src="./UserIcon.png" className="h-18 w-18 cursor-pointer" onClick={()=>navigate("/Profile")}/>
        </div>
    );
} 

export default Navbar;