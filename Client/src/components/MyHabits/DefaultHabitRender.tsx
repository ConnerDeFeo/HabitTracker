import HabitConverstions from "../../data/HabitConversions";
import Habit from "../../types/Habit";

//This is the default habit displayed in the myhabits page
const DefaultHabitRender = (props:{habit:Habit, topLeftButton:React.ReactNode, topRightButton:React.ReactNode})=>{
    const {habit,topLeftButton,topRightButton} = props;

    const getDaysActiveTitle = (habit:Habit):string =>{
        const daysActive: string[] = habit.daysActive;
        if(daysActive.length==7)
            return "Daily";
        const includesSaturday = daysActive.includes("Saturday");
        const includesSunday = daysActive.includes("Sunday");
        if(daysActive.length==5 && !includesSaturday && !includesSunday)
            return "Weekdays";
        if(daysActive.length==2 && includesSaturday && includesSunday)
            return "Weekends";
        const order: string[] = ["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"];

        //filter by days that are actually there, then convert to 3 letter abreviations
        return order.filter(day => daysActive.includes(day)).map((day)=>day.slice(0,3)).join(", ");
    }

    return(
        <div className="drop-shadow-xl p-3 grid gap-y-4 habitBorder w-65 lg:w-80 mx-auto">
            <div className="flex justify-between">
                {topLeftButton}
                <p className="text-lg lg:text-2xl">{getDaysActiveTitle(habit)}</p>
                {topRightButton}
            </div>
            <p className="text-4xl text-center mx-auto">{habit.name}</p>
            <div className="flex justify-center md:justify-between">
                <p className="text-lg lg:text-2xl">Date created: {habit.dateCreated}</p>
                <p className="text-lg lg:text-2xl hidden md:block">Type: {HabitConverstions.typeConverstion[habit.type]}</p>
            </div>
        </div>
    );
}

export default DefaultHabitRender;