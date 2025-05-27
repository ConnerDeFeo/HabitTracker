import { useEffect, useState } from "react";
import Habit from "../types/Habit";
import CreateHabit from "./CreateHabit";
import HabitService from "../service/HabitService";
import Button from "../components/Button";

const DailyHabit = (props: {habit: Habit, inEditMode: boolean, setHabits: React.Dispatch<React.SetStateAction<Habit[]>>})=>{
    const {habit, inEditMode, setHabits} = props;
    const [inEditHabitMode, setInEditHabitMode] = useState<boolean>(false);
    const [inDeletionMode, setInDeletionMode] = useState<boolean>(false);

    const fontStyling = "text-5xl pb-2 col-span-2";

    //when edit mode is changed, editing a habit should be reset
    useEffect(()=>{
        setInEditHabitMode(false);
        setInDeletionMode(false);
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

    const handleDeleteHabitClick = async ()=>{
        const resp = await HabitService.deleteHabit(habit.id!);
        if(resp.status==200){
            setHabits((prevHabits) =>
                prevHabits.filter(h =>h.id !== habit.id)
            );
        }
    }

    const handleHabitEditCompletion = async(habit:Habit)=>{
        const resp = await HabitService.editHabit(habit);
        
        if(resp.status==200){
            const newHabit = await resp.json();
            setHabits((prevHabits) =>
                prevHabits.map((h) =>
                    h.id === newHabit.id ? newHabit : h
                )
            );
            setInEditHabitMode(false);
        }
    }

    const handleHabitCompletionChange = async()=>{
        const resp = await HabitService.completeHabit(habit.id!);

        if(resp.status==200){
            habit.completed=true;
            setHabits((prevHabits) =>
                prevHabits.map((h) =>
                    h.id === habit.id ? habit : h
                )
            );
            setHabits
        }
    }

    const handleCancelation = ()=>{
        setInEditHabitMode(false);
    }

    return inEditMode ? 
                inEditHabitMode ? 
                    <CreateHabit 
                        handleCancelation={handleCancelation}
                        handleHabitCompletion={handleHabitEditCompletion}
                        initialHabit={{...habit}}
                    />
                :
                inDeletionMode ? 
                    <div className="w-80 break-words mx-auto grid grid-cols-2">
                        <p className={fontStyling+" col-span-2"}>Are you sure you want to delete this habit?</p>
                        <Button label="yes" className="mx-auto" onClick={handleDeleteHabitClick}/>
                        <Button label="no" className="mx-auto" onClick={()=>setInDeletionMode(false)}/>
                    </div>
                :
                <div className={"w-80 break-words mx-auto grid grid-cols-2 habitBorder"} key={habit.id}>
                    <img src="./EditHabits.svg" alt="editIcon" className="h-8 w-10 pl-3 mt-2 cursor-pointer" onClick={()=>setInEditHabitMode(true)}/>
                    <img src="./Minus.png" className="h-7 w-11 ml-auto pr-3 mt-2 cursor-pointer" onClick={()=>setInDeletionMode(true)} />
                    <p className={fontStyling}>{habit.name}</p>
                </div>
            :
                <div className={"w-80 break-words mx-auto cursor-pointer"} key={habit.id} onClick={handleHabitCompletionChange}>
                    <p className={fontStyling + (habit.completed ? "border border-black":"")}>{habit.name}</p>
                </div>
}

export default DailyHabit;