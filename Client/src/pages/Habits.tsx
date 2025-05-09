import { useEffect, useState } from "react";
import Container from "../components/Container";
import HabitService from "../service/HabitService";
import Habit from "../types/Habit";
import Button from "../components/Button";

const Habits = ()=>{

    const [habits,setHabits] = useState<Habit[]>([]);
    console.log(habits);

    useEffect(()=>{
        const fetchHabits = async ()=>{
            const response : Habit[] = await HabitService.GetHabits().then((resp)=>resp.json());
            setHabits(response);
        }
        fetchHabits();
    },[])

    const createHabit = async(habitName:string)=>{
        const response = await HabitService.CreateHabit(habitName);
        const habits = await response.json();
        setHabits(habits);
        console.log(habits);
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

export default Habits;