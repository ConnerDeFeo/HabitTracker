import { useEffect, useState } from "react";
import HabitService from "../service/HabitService";
import Habit from "../types/Habit";
import ImageButton from "../components/ImageButton";

const DailyHabits = ()=>{
    

    const addHabitButton = <ImageButton className="" image={<img src="./Add.svg" alt="editIcon" className="h-7 w-7 ml-[0.45rem]"/>}/>;

    const [habits,setHabits] = useState<Habit[]>([]);
    const [addHabit, setAddHabit] = useState<React.ReactNode>(<></>);
    const [inEditMode,setInEditMode] = useState<boolean>(false);

    useEffect(()=>{
        const fetchHabits = async ()=>{
            const resp = await HabitService.GetHabits();
            const data = await resp.json();
            setHabits(data);
        }
        fetchHabits();
    },[])

    const toggleEdit = ()=>{
        if(inEditMode)
            setAddHabit(<></>);
        else
            setAddHabit(addHabitButton);
    }
    const createHabit = async(habitName:string)=>{

        const response = await HabitService.CreateHabit(habitName);
        const habits = await response.json();
        setHabits(habits);
    }

    return(
            <div className="flex flex-col border border-black mx-auto">
                <div className="grid grid-cols-2 text-center gap-x-2 w-[60%] mx-auto mt-10 gap-y-10">
                    {habits.map((habit)=>{
                        return (
                            <div key={habit.id} className="border border-black w-80 break-words mx-auto">
                                <p className="text-5xl">{"Testing different word combinations "+habit.name}</p> 
                            </div>
                        );
                    })}
                    {addHabit}
                </div>
                <ImageButton onClick={toggleEdit} className="ml-[80%] mt-5" image={<img src="./EditHabits.svg" alt="editIcon" className="h-7 w-7 ml-[0.45rem]"/>}/>
            </div>      
    );
}

export default DailyHabits;