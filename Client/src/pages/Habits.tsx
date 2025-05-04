import { useEffect, useState } from "react";
import Container from "../components/Container";
import HabitService from "../service/HabitService";
import Habit from "../types/Habit";
import Button from "../components/Button";

const Habits = ()=>{

    const [habits,setHabits] = useState<Habit[]>([]);

    useEffect(()=>{
        const fetchHabits = async ()=>{
            const response : Habit[] = await HabitService.GetHabits().then((resp)=>resp.json());
            setHabits(response);
        }
        fetchHabits();
    },[])

    return(
        <Container content={
            <>
                <div className="grid grid-cols-2 text-center gap-4">
                    {habits.map((habit)=><div>{habit.Name}</div>)}
                </div>
                <Button label="create" onClick={()=>alert("Test")}/>
            </>
        }/>
            
    );
}

export default Habits;