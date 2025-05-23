import { useEffect, useState } from "react";
import HabitService from "../service/HabitService";
import Habit from "../types/Habit";
import ImageButton from "../components/ImageButton";
import DailyHabit from "./DailyHabit";
import Input from "../components/Input";

const CreateHabit = (props: {habit: Habit, setHabit: ()=>void})=>{
    // const {habit, setHabit} = props;
    return(
        <div>
            {/* <Input title="Name: " value={habit.name} updateValue={u}/> */}
        </div>
    );
}

const HabitCheckList = ()=>{
    const defaultHabit: Habit = {
        id: "",
        name: "",
        type: "Boolean",
        completed: false
    }

    const [habits,setHabits] = useState<Habit[]>([]);
    const [addHabit, setAddHabit] = useState<React.ReactNode>(<></>);
    const [inEditMode,setInEditMode] = useState<boolean>(false);
    const [newHabit, setNewHabit] = useState<Habit>(defaultHabit);
    
    const createHabit = ()=>{
        setAddHabit(<CreateHabit habit={newHabit} setHabit={()=>setNewHabit}/>);
    }

    const addHabitButton = <ImageButton className="mx-auto" onClick={()=>createHabit()}
    image={<img src="./Add.svg" alt="editIcon" className="h-7 w-7 ml-[0.45rem]"/>}/>;

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
        setInEditMode(!inEditMode);
    }

    return(
            <div className="flex flex-col border border-black mx-auto">
                <div className="grid grid-cols-2 text-center gap-x-2 w-[60%] mx-auto mt-10 gap-y-10 border border-black ">
                    {habits.map((habit)=>
                        <DailyHabit habit={habit} key={habit.id}/>
                    )}
                    {addHabit}
                </div>
                <ImageButton onClick={toggleEdit} className="ml-[80%] mt-5 drop-shadow-lg" 
                    image={<img src="./EditHabits.svg" alt="editIcon" className="h-7 w-7 ml-[0.45rem]"/>}/>
            </div>      
    );
}

export default HabitCheckList;