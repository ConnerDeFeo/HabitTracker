import { useState } from "react";
import Habit from "../../types/Habit";
import HabitService from "../../services/HabitService";
import CreateHabit from "./CreateHabit";
import Button from "../Button";
import DefaultHabitRender from "./DefaultHabitRender";

//The active a single active habit in the myhabits page
const RenderActiveHabit = (props:
    {habit:Habit, 
        setActiveHabits:React.Dispatch<React.SetStateAction<Habit[]>>,
        setNonActiveHabits:React.Dispatch<React.SetStateAction<Habit[]>>
    }
)=>{
    const {habit,setActiveHabits,setNonActiveHabits} = props;

    const [inEditMode,setInEditMode] = useState<boolean>(false); //Flag for the user clickcing the edit symbol
    const [inRemovalMode,setInRemovalMode] = useState<boolean>(false); //Flag for user clicking the minus symbol


    //On when the user clicks check for a new habit being created
    const handleHabitEditCompletion = async (habit:Habit)=>{
        const resp = await HabitService.editHabit(habit);
        const newHabit = await resp.json();
    
        setActiveHabits((prevHabits) =>
            prevHabits.map((h) => (h.id === newHabit.id ? newHabit : h))
        );
        setInEditMode(false);
    }

    //On confimation that the user would like to deactivate a habit
    const handleHabitDeactivation = async()=>{
        const resp = await HabitService.deactivateHabit(habit.id!);
        if(resp.status==200){
            setActiveHabits((prevHabits)=>prevHabits.filter(h=>h.id!==habit.id));
            setNonActiveHabits((prevHabits)=>[...prevHabits,habit]);
            setInRemovalMode(false);
        }  
    }

    return inEditMode ? 
        //User clicks edit icon
        <CreateHabit handleCancelation={()=>setInEditMode(false)} handleHabitCompletion={handleHabitEditCompletion} initialHabit={habit}/>
        :
        //User clicks minus icon
        inRemovalMode ? 
        <div className="border-b-3 p-3 grid gap-y-4">
            <p className="text-4xl text-center">{habit.name}</p>
            <p className="text-4xl text-center">Deactivate?</p>
            <div className="flex justify-between w-[70%] mx-auto">
                <Button label="Yes" className="w-10 sm:w-15" onClick={handleHabitDeactivation}/>
                <Button label="No" className="w-10 sm:w-15" onClick={()=>setInRemovalMode(false)}/>
            </div>
        </div>
        :
        //Default
        <DefaultHabitRender 
            habit={habit}
            topLeftButton = {<img src="/EditHabits.svg" alt="editHabit" className="h-6 w-6 cursor-pointer" onClick={()=>setInEditMode(true)}/>}
            topRightButton = {<img src="/Minus.png" alt="deactivateHabit" className="h-6 w-6 cursor-pointer" onClick={()=>setInRemovalMode(true)}/>}
        />
    ;
}

export default RenderActiveHabit;