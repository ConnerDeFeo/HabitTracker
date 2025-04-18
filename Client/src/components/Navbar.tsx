
const Navbar = ()=>{
    return(
        <div className="flex place-content-between border-b-4 border-black h-18 w-[85%] mx-auto pt-3">
            <div>
                <h1 className="font-hand text-2xl">Habit Tracker</h1>
            </div>
            <img src="./UserIcon.png" className="h-10 w-10 border border-black"/>
        </div>
    );
} 

export default Navbar;