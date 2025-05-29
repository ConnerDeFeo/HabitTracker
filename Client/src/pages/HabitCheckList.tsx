import { useEffect, useState } from "react";
import HabitService from "../services/HabitService";
import Habit from "../types/Habit";
import ImageButton from "../components/ImageButton";
import CreateHabit from "../components/CreateHabit";
import HabitComponent from "./HabitComponet";

const HabitCheckList = ()=>{

    const [habits,setHabits] = useState<Habit[]>([]);

    //All used for creating new habit UI
    const [addHabit, setAddHabit] = useState<React.ReactNode>(<></>);
    const [inEditMode,setInEditMode] = useState<boolean>(false);

    //On render grab the users habits
    useEffect(()=>{
        const fetchHabits = async ()=>{
            const resp = await HabitService.getHabits();
            const data = await resp.json();
            setHabits(data);
            console.log(data);
        }
        fetchHabits();
    },[])

    //When the user completes a new habit
    const handleNewHabitCompletion = async (habit:Habit)=>{
        const resp = await HabitService.createHabit(habit);

        if(resp.status==200){
            const newHabit = await resp.json();
            setHabits((prevHabits)=>([
                    ...prevHabits,
                    newHabit
                ]
            ));
            setAddHabit(addHabitButton);
        }
    }

    //The plus icon the user looks at when making a new habit
    const addHabitButton = 
        <ImageButton 
            className="mx-auto" 
            onClick={
                ()=>setAddHabit(
                    <CreateHabit 
                        handleCancelation={()=>setAddHabit(addHabitButton)}
                        handleHabitCompletion={handleNewHabitCompletion}
                    />
                )}
            image={<img src="./Add.svg" alt="editIcon" className="h-7 w-7 ml-[0.45rem]"/>}
        />;

    //Switches between differnet edit modes
    const toggleEdit = ()=>{
        if(inEditMode)
            setAddHabit(<></>);
        else
            setAddHabit(addHabitButton);
        setInEditMode(!inEditMode);
    }
    return(
        <div className="flex flex-col  mx-auto mb-[50vh]">
            <div className="grid grid-cols-2 text-center gap-x-2 w-[60%] mx-auto mt-10 gap-y-10">
                {habits.map((habit)=><HabitComponent habit={habit} inEditMode={inEditMode} key={habit.id} setHabits={setHabits}/>)}
                {/*This will only show if user is in edit mode */}
                {addHabit}
            </div>
            <ImageButton onClick={toggleEdit} className="ml-[80%] mt-5 drop-shadow-lg drop-shadow-lg" 
                image={<img src="./EditHabits.svg" alt="editIcon" className="h-7 w-7 ml-[0.45rem]"/>}/>
        </div>      
    );
}

export default HabitCheckList;