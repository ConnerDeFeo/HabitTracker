import NavigationItem from "./NavigationItem";

//after clicking hamburger, this apears
const Menu = (props:{exitMenu:()=>void})=>{
    const {exitMenu} = props;

    return (
        <div className="w-[80%] mx-auto">
            <img src="./x.webp" className="h-10 w-10 cursor-pointer my-5" onClick={exitMenu}/>
            <div className="grid gap-y-10 w-[80%] mx-auto">
                <NavigationItem name="MyHabits" imagePath="Arrows.png" alt="MyHabits" navigateTo="/MyHabits"/>
                <NavigationItem name="Schedule" imagePath="Calender.png" alt="calender" navigateTo="/Schedule"/>
                <NavigationItem name="Statistics" imagePath="Statistics.png" alt="Statistics" navigateTo="/Statistics"/>
            </div>
        </div>
    );
}

export default Menu;