import { useState } from "react";
import Habit from "../types/Habit";
import HabitService from "../services/HabitService";
import Button from "../components/Button";

const RenderNonActiveHabits = (props:
    {habit:Habit, 
        setActiveHabits:React.Dispatch<React.SetStateAction<Habit[]>>,
        setNonActiveHabits:React.Dispatch<React.SetStateAction<Habit[]>>
    }
)=>{
    const {habit,setActiveHabits,setNonActiveHabits} = props;
    const [inReactivateMode,setInReactivateMode] = useState<boolean>(false);
    const [inDeletionMode,setInDeletionMode] = useState<boolean>(false);
    const [currentDeletionValue,setCurrentDeletionValue] = useState<string>("");
    const canDelete = currentDeletionValue==habit.name;

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

    const handleHabitReactivation = async ()=>{
        const resp = await HabitService.reactivateHabit(habit.id!);

        if(resp.status===200){
            setNonActiveHabits((prevHabits)=>prevHabits.filter(h=>h.id!==habit.id));
            setActiveHabits((prevHabits)=>[...prevHabits,habit]);
        }
    }

    const handleHabitDeletion = async()=>{
        const resp = await HabitService.deleteHabit(habit.id!);

        if(resp.status===200)
            setNonActiveHabits((prevHabits)=>prevHabits.filter(h=>h.id!==habit.id));
        
    }

    return inReactivateMode ? 
        <div className="habitBorder p-3 grid gap-y-4">
            <p className="text-4xl text-center">{habit.name}</p>
            <p className="text-4xl text-center">Reactivate?</p>
            <div className="flex justify-between w-[70%] mx-auto">
                <Button label="Yes" className="w-15" onClick={handleHabitReactivation}/>
                <Button label="No" className="w-15" onClick={()=>setInReactivateMode(false)}/>
            </div>
        </div>
        :
        inDeletionMode ? 
        <div className="habitBorder p-3 grid gap-y-4">
            <p className="text-4xl text-center wrap">Type "{habit.name}" to delete</p>
            <input value={currentDeletionValue} onChange={(e)=>setCurrentDeletionValue(e.target.value)} className="habitBorder w-[80%] pl-3 mx-auto"/>
            <div className="flex justify-between w-[70%] mx-auto">
                <Button label="Delete" className="w-15" onClick={canDelete ? handleHabitDeletion : ()=>setCurrentDeletionValue("Names did not match")}/>
                <Button label="Cancel" className="w-15" onClick={()=>setInDeletionMode(false)}/>
            </div>
        </div>
        :
        <div className="habitBorder p-3 grid gap-y-4">
            <div className="flex justify-between">
                <img src="Add.svg" alt="reactivateHabit" className="h-6 w-6 cursor-pointer" onClick={()=>setInReactivateMode(true)}/>
                <p className="text-2xl">{getDaysActiveTitle(habit)}</p>
                <img src="Minus.png" alt="removeHabit" className="h-6 w-6 cursor-pointer" onClick={()=>setInDeletionMode(true)}/>
            </div>
            <p className="text-4xl text-center">{habit.name}</p>
            <div className="flex justify-between">
                <p className="text-2xl">Date created: {habit.dateCreated}</p>
                <p className="text-2xl">Type: {typeConverstion[habit.type]}</p>
            </div>
        </div>
    ;
}

export default RenderNonActiveHabits;