import { useEffect, useState } from "react";
import Habit from "../types/Habit";
import HabitService from "../services/HabitService";
import ImageButton from "../components/ImageButton";
import CreateHabit from "../components/CreateHabit";
import RenderActiveHabit from "./RenderActiveHabits";
import RenderNonActiveHabits from "./RenderNonActiveHabits";

const MyHabits = (props:{fetchMonth: ()=>void})=>{

    const {fetchMonth} = props;

    const [activeHabits, setActiveHabits] = useState<Habit[]>([]);
    const [nonActiveHabits, setNonActiveHabits] = useState<Habit[]>([]);
    const [addHabit, setAddHabit] = useState<boolean>(false);

    useEffect(()=>{
        fetchMonth();
    },[activeHabits,nonActiveHabits])

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
        <div className="flex w-[60%] mx-auto justify-between mt-7 mb-[10vh]">
            <div>
                <h1 className="border-b-6 text-7xl text-center w-85 text-center mb-5">Active Habits</h1>
                <div className="overflow-y-auto h-[70vh] p-2 flex flex-col gap-y-4">
                    {activeHabits.map((habit)=>
                        <RenderActiveHabit 
                            key={habit.name} 
                            habit={habit} 
                            setActiveHabits={setActiveHabits} 
                            setNonActiveHabits={setNonActiveHabits}
                        />
                    )}
                    {addHabit ?
                        <CreateHabit
                            handleCancelation={()=>setAddHabit(false)}
                            handleHabitCompletion={handleNewHabitCompletion}
                        />
                        :
                        <ImageButton
                            className="mx-auto mt-5"
                            onClick={()=>setAddHabit(true)}
                            image={<img src="./Add.svg" alt="editIcon" className="h-7 w-7 ml-[0.45rem] my-[0.45rem]"/>}
                        />
                    }
                </div>
            </div>
            <div>
                <h1 className="border-b-6 text-7xl w-85 text-center mb-5">NonActive Habits</h1>
                <div className="overflow-y-auto h-[70vh] flex flex-col gap-y-4">
                    {nonActiveHabits.map((habit)=>
                        <RenderNonActiveHabits 
                            key={habit.name} 
                            habit={habit}
                            setActiveHabits={setActiveHabits}
                            setNonActiveHabits={setNonActiveHabits}
                        />
                    )}
                </div>
            </div>
        </div>
    );
}

export default MyHabits;