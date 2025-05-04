import { useEffect, useState } from "react";
import Container from "../components/Container";
import HabitService from "../service/HabitService";
import Habit from "../types/Habit";

const Habits = ()=>{

    const [habits,setHabits] = useState<Habit[]>([]);

    useEffect(()=>{
        const fetchHabits = async ()=>{
            const response = await HabitService.GetHabits().then((resp)=>resp.json());
            setHabits(response.Habits || []);
        }
        fetchHabits();
    },[])

    return(
        <Container content={
            <>
                <div className="grid grid-cols-2 grid-rows-2 text-center gap-4">
                    {habits.map((habit)=><div>{habit.Name}</div>)}
                </div>
                <button>Testing</button>
            </>
        }/>
            
    );
}

export default Habits;