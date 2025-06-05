import { useState } from "react";
import Habit from "../types/Habit";
import HabitService from "../services/HabitService";
import CreateHabit from "./CreateHabit";
import ImageButton from "./ImageButton";

const RenderHabit = (props:{habit:Habit, active:boolean, setHabits:React.Dispatch<React.SetStateAction<Habit[]>>})=>{
    const {habit,active,setHabits} = props;
    const typeConverstion: Record<number,string> = {1:"Binary",2:"Time",3:"Numeric"};
    const [inChangeMode,setInChangeMode] = useState<boolean>(false);
    const [inRemovalMode,setInRemovalMode] = useState<boolean>(false);

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

    const handleMinusClick = ()=>{
        if(active){
            return;
        }
        return;
    }

    const handleHabitEditCompletion = async (habit:Habit)=>{
        const resp = await HabitService.editHabit(habit);
        const newHabit = await resp.json();
    
        setHabits((prevHabits) =>
            prevHabits.map((h) => (h.id === newHabit.id ? newHabit : h))
        );
        setInChangeMode(false);
    }

    return(
        inChangeMode ? 
            active ? 
                <CreateHabit handleCancelation={()=>setInChangeMode(false)} handleHabitCompletion={handleHabitEditCompletion} initialHabit={habit}/>
                :
                <div className="habitBorder p-3 grid gap-y-4">
                    <div className="flex justify-between">
                        {active ? 
                            <img src="EditHabits.svg" alt="edit" className="h-6 w-6 cursor-pointer" onClick={()=>setInChangeMode(true)}/>
                            :
                            <ImageButton/>
                        }
                        <p className="text-2xl">{getDaysActiveTitle(habit)}</p>
                        <img src="Minus.png" alt="removeHabit" className="h-6 w-6 cursor-pointer" onClick={handleMinusClick}/>
                    </div>
                    <p className="text-4xl text-center">{habit.name}</p>
                    <div className="flex justify-between">
                        <p className="text-2xl">Date created: {habit.dateCreated}</p>
                        <p className="text-2xl">Type: {typeConverstion[habit.type]}</p>
                    </div>
                </div>
            :
            <div className="habitBorder p-3 grid gap-y-4">
                <div className="flex justify-between">
                    {active ? 
                        <img src="EditHabits.svg" alt="edit" className="h-6 w-6 cursor-pointer" onClick={()=>setInChangeMode(true)}/>
                        :
                        <ImageButton/>
                    }
                    <p className="text-2xl">{getDaysActiveTitle(habit)}</p>
                    <img src="Minus.png" alt="removeHabit" className="h-6 w-6 cursor-pointer" onClick={handleMinusClick}/>
                </div>
                <p className="text-4xl text-center">{habit.name}</p>
                <div className="flex justify-between">
                    <p className="text-2xl">Date created: {habit.dateCreated}</p>
                    <p className="text-2xl">Type: {typeConverstion[habit.type]}</p>
                </div>
            </div>
        
    );
}

export default RenderHabit;