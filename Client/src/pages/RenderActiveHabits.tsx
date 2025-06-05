import { useState } from "react";
import Habit from "../types/Habit";
import HabitService from "../services/HabitService";
import CreateHabit from "../components/CreateHabit";
import Button from "../components/Button";

const RenderActiveHabit = (props:
    {habit:Habit, 
        setActiveHabits:React.Dispatch<React.SetStateAction<Habit[]>>,
        setNonActiveHabits:React.Dispatch<React.SetStateAction<Habit[]>>
    }
)=>{
    const {habit,setActiveHabits,setNonActiveHabits} = props;
    const [inEditMode,setInEditMode] = useState<boolean>(false);
    const [inRemovalMode,setInRemovalMode] = useState<boolean>(false);

    const typeConverstion: Record<number,string> = {1:"Binary",2:"Time",3:"Numeric"};

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

    const handleHabitEditCompletion = async (habit:Habit)=>{
        const resp = await HabitService.editHabit(habit);
        const newHabit = await resp.json();
    
        setActiveHabits((prevHabits) =>
            prevHabits.map((h) => (h.id === newHabit.id ? newHabit : h))
        );
        setInEditMode(false);
    }

    const handleHabitDeactivation = async()=>{
        const resp = await HabitService.deactivateHabit(habit.id!);

        if(resp.status==200){
            setActiveHabits((prevHabits)=>prevHabits.filter(h=>h.id!==habit.id));
            setNonActiveHabits((prevHabits)=>[...prevHabits,habit]);
            setInRemovalMode(false);
        }  
    }

    return inEditMode ? 
        <CreateHabit handleCancelation={()=>setInEditMode(false)} handleHabitCompletion={handleHabitEditCompletion}/>
        :
        inRemovalMode ? 
        <div className="habitBorder p-3 grid gap-y-4">
            <p className="text-4xl text-center">{habit.name}</p>
            <p className="text-4xl text-center">Deactivate?</p>
            <div className="flex justify-between w-[70%] mx-auto">
                <Button label="Yes" className="w-15" onClick={handleHabitDeactivation}/>
                <Button label="No" className="w-15" onClick={()=>setInRemovalMode(false)}/>
            </div>
        </div>
        :
        <div className="habitBorder p-3 grid gap-y-4">
            <div className="flex justify-between">
                <img src="EditHabits.svg" alt="editHabit" className="h-6 w-6 cursor-pointer" onClick={()=>setInEditMode(true)}/>
                <p className="text-2xl">{getDaysActiveTitle(habit)}</p>
                <img src="Minus.png" alt="deactivateHabit" className="h-6 w-6 cursor-pointer" onClick={()=>setInRemovalMode(true)}/>
            </div>
            <p className="text-4xl text-center">{habit.name}</p>
            <div className="flex justify-between">
                <p className="text-2xl">Date created: {habit.dateCreated}</p>
                <p className="text-2xl">Type: {typeConverstion[habit.type]}</p>
            </div>
        </div>
    ;
}

export default RenderActiveHabit;