import { useState } from "react";
import Habit from "../../types/Habit";
import HabitService from "../../services/HabitService";
import Button from "../General/Button";
import DefaultHabitRender from "./DefaultHabitRender";

//Represents the non active habits shown in the myhabits page
const RenderNonActiveHabits = (props:
    {habit:Habit, 
        setActiveHabits:React.Dispatch<React.SetStateAction<Habit[]>>,
        setNonActiveHabits:React.Dispatch<React.SetStateAction<Habit[]>>
    }
)=>{
    const {habit,setActiveHabits,setNonActiveHabits} = props;
    const [inReactivateMode,setInReactivateMode] = useState<boolean>(false); //Flag for plus symbol being clicked
    const [inDeletionMode,setInDeletionMode] = useState<boolean>(false); //Flag for minus symbol being clicked
    const [currentDeletionValue,setCurrentDeletionValue] = useState<string>(""); //Holds current typed out deletion name from user
    const canDelete = currentDeletionValue==habit.name;

    // Handles confirmation of a habit reactivation
    const handleHabitReactivation = async ()=>{
        const resp = await HabitService.reactivateHabit(habit.id!);

        if(resp.status===200){
            setNonActiveHabits((prevHabits)=>prevHabits.filter(h=>h.id!==habit.id));
            setActiveHabits((prevHabits)=>[...prevHabits,habit]);
        }
    }

    //Handles confirmation of a habit deactivation
    const handleHabitDeletion = async()=>{
        const resp = await HabitService.deleteHabit(habit.id!);

        if(resp.status===200)
            setNonActiveHabits((prevHabits)=>prevHabits.filter(h=>h.id!==habit.id));
        
    }

    
    return inReactivateMode ? //User clicks plus symbol
        <div className="habitBorder p-3 grid gap-y-4 w-65 lg:w-80 mx-auto">
            <p className="text-4xl text-center">{habit.name}</p>
            <p className="text-4xl text-center">Reactivate?</p>
            <div className="flex justify-between w-[90%] mx-auto">
                <Button label="Yes" className="w-20" onClick={handleHabitReactivation}/>
                <Button label="No" className="w-20" onClick={()=>setInReactivateMode(false)}/>
            </div>
        </div>
        :
        inDeletionMode ? //User clicks minus symbol
        <div className="habitBorder p-3 grid gap-y-4 w-65 lg:w-80 mx-auto">
            <p className="text-4xl text-center wrap">Type "{habit.name}" to delete</p>
            <input value={currentDeletionValue} onChange={(e)=>setCurrentDeletionValue(e.target.value)} className="habitBorder w-[90%] pl-3 mx-auto text-lg h-9 md:h-12"/>
            <div className="flex justify-between w-[90%] mx-auto">
                <Button label="Delete" className="w-20" onClick={canDelete ? handleHabitDeletion : ()=>setCurrentDeletionValue("Names did not match")}/>
                <Button label="Cancel" className="w-20" onClick={()=>setInDeletionMode(false)}/>
            </div>
        </div>
        :
        //Default
        <DefaultHabitRender 
            habit={habit}
            topLeftButton = {<img src="/Add.svg" alt="reactivateHabit" className="h-6 w-6 cursor-pointer" onClick={()=>setInReactivateMode(true)}/>}
            topRightButton = {<img src="/Minus.png" alt="removeHabit" className="h-6 w-6 cursor-pointer" onClick={()=>setInDeletionMode(true)}/>}
        />
    ;
}

export default RenderNonActiveHabits;