import { useEffect, useState } from "react";
import Habit from "../types/Habit";
import HabitService from "../services/HabitService";
import ImageButton from "../components/ImageButton";
import CreateHabit from "../components/CreateHabit";
import RenderHabit from "../components/RenderHabit";

const MyHabits = ()=>{

    const [activeHabits, setActiveHabits] = useState<Habit[]>([]);
    const [nonActiveHabits, setNonActiveHabits] = useState<Habit[]>([]);
    const [addHabit, setAddHabit] = useState<boolean>(false);

    useEffect(()=>{
        const fetchHabits = async()=>{
            const resp = await HabitService.getExistingHabits();
            if(resp.status==200){
                const existingHabits = await resp.json();
                setActiveHabits(existingHabits["ActiveHabits"]);
                setNonActiveHabits(existingHabits["NonActiveHabits"]);
            }
        }
        fetchHabits();
    },[]);

    //When the user completes a new habit
    const handleNewHabitCompletion = async (habit:Habit)=>{
        const resp = await HabitService.createHabit(habit);
        if(resp.status==200){
            const newHabit = await resp.json();
            setActiveHabits((prevHabits)=>(
                [
                    ...prevHabits,
                    newHabit
                ]
            ));
            setAddHabit(false);
        }
    }

    return(
        <div className="flex w-[60%] mx-auto justify-between mt-7 mb-[20vh]">
            <div className="grid">
                <h1 className="border-b-6 text-7xl text-center w-85 text-center mb-5">Active Habits</h1>
                {activeHabits.map((habit)=><RenderHabit key={habit.name} habit={habit} active={true} setHabits={setActiveHabits}/>)}
                {addHabit ?
                    <CreateHabit
                        handleCancelation={()=>setAddHabit(false)}
                        handleHabitCompletion={handleNewHabitCompletion}
                    />
                    :
                    <ImageButton
                        className="mx-auto mt-10" 
                        onClick={()=>setAddHabit(true)}
                        image={<img src="./Add.svg" alt="editIcon" className="h-7 w-7 ml-[0.45rem]"/>}
                    />
                }
            </div>
            <div>
                <h1 className="border-b-6 text-7xl w-85 text-center mb-5">NonActive Habits</h1>
            </div>
        </div>
    );
}

export default MyHabits;