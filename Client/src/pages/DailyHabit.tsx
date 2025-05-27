import { useEffect, useState } from "react";
import Habit from "../types/Habit";
import CreateHabit from "./CreateHabit";
import HabitService from "../service/HabitService";

const DailyHabit = (props: {habit: Habit, inEditMode: boolean, setHabits: React.Dispatch<React.SetStateAction<Habit[]>>})=>{
    const {habit, inEditMode, setHabits} = props;
    const [inEditHabitMode, setInEditHabitMode] = useState<boolean>(false);

    //when edit mode is changed, editing a habit should be reset
    useEffect(()=>{
        setInEditHabitMode(false);
    },[inEditMode]);

    let habitType;
    switch(habit.type){
        case 2:
            habitType = "Time: "+habit.value+" minutes";
            break;
        case 3:
            habitType = "Numeric: "+habit.value+" "+habit.valueUnitType;
            break;
        default:
            habitType="Boolean"
            break;
    }

    const handleClick = ()=>{
        if(inEditMode)
            setInEditHabitMode(true);
    }

    const handleHabitEditCompletion = async(habit:Habit)=>{
        const resp = await HabitService.EditHabit(habit);
        
        if(resp.status==200){
            const newHabit = await resp.json();
            console.log(newHabit);
            setHabits((prevHabits) =>
                prevHabits.map((h) =>
                    h.id === newHabit.id ? newHabit : h
                )
            );
            setInEditHabitMode(false);
        }
    }

    const handleCancelation = ()=>{
        setInEditHabitMode(false);
    }

    return inEditMode && inEditHabitMode ? 
        <CreateHabit 
            handleCancelation={handleCancelation}
            handleHabitCompletion={handleHabitEditCompletion}
            initialHabit={{...habit}}
        />
        :
        <div className={"w-80 break-words mx-auto cursor-pointer "+ (inEditMode?"border-2 shadow-xl rounded-2xl":"")} key={habit.id} 
            onClick={handleClick}
        >
            {inEditMode ? <img src="./EditHabits.svg" alt="editIcon" className="h-10 w-10 pl-2 mt-2"/>:<></>}
            <p className="text-5xl pb-2">{habit.name}</p> 
        </div>
}

export default DailyHabit;