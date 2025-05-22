import { useEffect, useState } from "react";
import HabitService from "../service/HabitService";
import Habit from "../types/Habit";
import Button from "../components/Button";

const DailyHabits = ()=>{

    const [habits,setHabits] = useState<Habit[]>([]);

    useEffect(()=>{
        const fetchHabits = async ()=>{
            const resp = await HabitService.GetHabits();
            const data = await resp.json();
            setHabits(data);
        }
        fetchHabits();
    },[])

    const createHabit = async(habitName:string)=>{
        const response = await HabitService.CreateHabit(habitName);
        const habits = await response.json();
        setHabits(habits);
    }

    return(
            <>
                <div className="grid grid-cols-4 text-center gap-y-6 w-[60%] mx-auto mt-10">
                    {habits.map((habit)=>
                        <p key={habit.id} className="text-6xl">{"• "+habit.name}</p>)
                    }
                </div>
                <Button label="create" onClick={()=>createHabit("Please")}/>
            </>      
    );
}

export default DailyHabits;