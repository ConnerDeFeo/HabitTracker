import { useEffect, useState } from "react";
import Habit from "../types/Habit";
import HabitService from "../services/HabitService";
import ImageButton from "../components/ImageButton";
import CreateHabit from "../components/MyHabits/CreateHabit";
import RenderActiveHabit from "../components/MyHabits/RenderActiveHabits";
import RenderNonActiveHabits from "../components/MyHabits/RenderNonActiveHabits";

//MyHabits link leads to this
const MyHabits = (props:{fetchMonth: ()=>void})=>{
    const {fetchMonth} = props;

    const [activeHabits, setActiveHabits] = useState<Habit[]>([]);
    const [nonActiveHabits, setNonActiveHabits] = useState<Habit[]>([]);
    //Flag for if a add button or habit creation form should be shown
    const [addHabit, setAddHabit] = useState<boolean>(false);

    //anytime either list changed, monthly habits needs to be updated
    useEffect(()=>{
        fetchMonth();
    },[activeHabits,nonActiveHabits])

    //All habits should be grabbed on load
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
        <div className="grid grid-cols-2 w-[80%] mx-auto justify-center mt-7 mb-[10vh]">
            <div className="w-90 mx-auto">
                <h1 className="border-b-4 lg:border-b-6 text-5xl lg:text-7xl text-center w-65 lg:w-85 text-center mb-5 mx-auto">Active Habits</h1>
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
            <div className="w-90 mx-auto">
                <h1 className="border-b-4 lg:border-b-6 text-5xl lg:text-7xl w-65 lg:w-85 text-center mb-5 mx-auto">NonActive Habits</h1>
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