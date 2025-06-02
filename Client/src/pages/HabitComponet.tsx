import { useEffect, useState } from "react";
import Habit from "../types/Habit";
import CreateHabit from "../components/CreateHabit";
import HabitService from "../services/HabitService";
import Button from "../components/Button";
import DateInfo from "../types/DateInfo";
import GeneralService from "../services/GeneralService";

const HabitComponet = (props: {habit: Habit, inEditMode: boolean, setHabits: React.Dispatch<React.SetStateAction<Habit[]>>, date:DateInfo})=>{
    const {habit, inEditMode, setHabits, date} = props;
    const [inEditHabitMode, setInEditHabitMode] = useState<boolean>(false);
    const [inDeletionMode, setInDeletionMode] = useState<boolean>(false);

    const fontStyling = "text-5xl pb-2 col-span-2";

    //when edit mode is changed, editing a habit should be reset
    useEffect(()=>{
        setInEditHabitMode(false);
        setInDeletionMode(false);
    },[inEditMode]);


    //When this habbit is to be delted
    const handleDeleteHabitClick = async ()=>{
        const resp = await HabitService.deleteHabit(habit.id!);
        if(resp.status==200){
            setHabits((prevHabits) =>
                prevHabits.filter(h =>h.id !== habit.id)
            );
        }
    }

    //When this habbit is to be edited
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

    //when a habit is completed
    const handleHabitCompletionChange = async()=>{
        const habitCompleted = !habit.completed;
        const resp = await HabitService.completeHabit(habit.id!, `${date.year}-${GeneralService.padZero(date.month)}-${GeneralService.padZero(date.day)}`, habitCompleted);

        if(resp.status==200){
            habit.completed = habitCompleted;
            setHabits((prevHabits) =>
                prevHabits.map((h) =>
                    h.id === habit.id ? habit : h
                )
            );
        }
    }

    //On canelation of either deletion or editation
    const handleCancelation = ()=>{
        setInEditHabitMode(false);
    }

    //User puts whole screen in edit mode
    return inEditMode ? 
                //Edits this habit specifically
                inEditHabitMode ? 
                    <CreateHabit 
                        handleCancelation={handleCancelation}
                        handleHabitCompletion={handleHabitEditCompletion}
                        initialHabit={{...habit}}
                    />
                :
                //Hits minus symbol for potential deletion of this habit
                inDeletionMode ? 
                    <div className="w-80 break-words mx-auto grid grid-cols-2">
                        <p className={fontStyling+" col-span-2"}>Are you sure you want to delete this habit?</p>
                        <Button label="yes" className="mx-auto" onClick={handleDeleteHabitClick}/>
                        <Button label="no" className="mx-auto" onClick={()=>setInDeletionMode(false)}/>
                    </div>
                :
                //Default case in edit mode, showing differet edit choises and delteion
                <div className={"w-80 break-words mx-auto grid grid-cols-2 habitBorder"} key={habit.id}>
                    <img src="./EditHabits.svg" alt="editIcon" className="h-8 w-10 pl-3 mt-2 cursor-pointer" onClick={()=>setInEditHabitMode(true)}/>
                    <img src="./Minus.png" className="h-7 w-11 ml-auto pr-3 mt-2 cursor-pointer" onClick={()=>setInDeletionMode(true)} />
                    <p className={fontStyling}>{habit.name}</p>
                </div>
        :
        //Just looking at the normal daily checklist
        <div className={"w-80 break-words mx-auto cursor-pointer "} key={habit.id} onClick={handleHabitCompletionChange}>
            <p className={fontStyling + (habit.completed && " line-through")}>{habit.name}</p>
        </div>
}

export default HabitComponet;