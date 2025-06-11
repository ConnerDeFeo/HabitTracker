import Habit from "../../types/Habit";

//Basically because both render classes were sharing this code
const RenderHabitUtils ={
    typeConverstion: {1:"Binary",2:"Time",3:"Numeric"} as Record<number, string>,
    getDaysActiveTitle: (habit:Habit):string =>{
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
}

export default RenderHabitUtils;