import { useEffect, useState } from "react";
import HabitService from "../service/HabitService";
import Habit from "../types/Habit";
import ImageButton from "../components/ImageButton";
import CreateHabit from "./CreateHabit";
import DailyHabit from "./DailyHabit";

const HabitCheckList = ()=>{

    const [habits,setHabits] = useState<Habit[]>([]);

    //all used for creating new habit UI
    const [addHabit, setAddHabit] = useState<React.ReactNode>(<></>);
    const [inEditMode,setInEditMode] = useState<boolean>(false);

    const addHabitButton = 
        <ImageButton 
            className="mx-auto" 
            onClick={()=>setAddHabit(<CreateHabit setHabits={setHabits} handleCancelation={()=>setAddHabit(addHabitButton)}/>)}
            image={<img src="./Add.svg" alt="editIcon" className="h-7 w-7 ml-[0.45rem]"/>}/>;

    useEffect(()=>{
        const fetchHabits = async ()=>{
            const resp = await HabitService.GetHabits();
            const data = await resp.json();
            setHabits(data);
        }
        fetchHabits();
    },[])

    const handleHabitEdits = ()=>{
        
    }

    const toggleEdit = ()=>{
        if(inEditMode)
            setAddHabit(<></>);
        else
            setAddHabit(addHabitButton);
        setInEditMode(!inEditMode);
    }

    return(
            <div className="flex flex-col  mx-auto">
                <div className="grid grid-cols-2 text-center gap-x-2 w-[60%] mx-auto mt-10 gap-y-10">
                    {habits.map((habit)=><DailyHabit habit={habit} inEditMode={inEditMode} key={habit.id}/>)}
                    {addHabit}
                </div>
                <ImageButton onClick={toggleEdit} className="ml-[80%] mt-5 drop-shadow-lg" 
                    image={<img src="./EditHabits.svg" alt="editIcon" className="h-7 w-7 ml-[0.45rem]"/>}/>
            </div>      
    );
}

export default HabitCheckList;