import {useEffect, useRef, useState } from "react";
import Habit from "../types/Habit";
import HabitService from "../services/HabitService";
import ImageButton from "../components/General/ImageButton";
import CreateHabit from "../components/MyHabits/CreateHabit";
import RenderActiveHabit from "../components/MyHabits/RenderActiveHabits";
import RenderNonActiveHabits from "../components/MyHabits/RenderNonActiveHabits";
import OpenAIService from "../services/OpenAIService";

//MyHabits link leads to this
const MyHabits = (props:{fetchMonth: ()=>void, username:string})=>{
    const {fetchMonth, username} = props;

    const [activeHabits, setActiveHabits] = useState<Habit[]>([]);
    const [nonActiveHabits, setNonActiveHabits] = useState<Habit[]>([]);
    //Flag for if a add button or habit creation form should be shown
    const [addHabit, setAddHabit] = useState<boolean>(false);
    
    const [aiReccomendation,setAiReccomendation] = useState<string>("");
    const hasFetchedRec = useRef(false);

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
            };
        }
        const fetchReccomendation = async ()=>{
            if(!hasFetchedRec.current){
                hasFetchedRec.current=true;
                const resp = await OpenAIService.getReccomendation();
                if(resp.status==200){
                    const reccomendation = await resp.text();
                    setAiReccomendation(reccomendation);
                };
            }
        }
        fetchHabits();
        fetchReccomendation();
    },[]);

    //When the user completes a new habit
    const handleNewHabitCompletion = async (habit:Habit)=>{
        if(habit.value==0)
            habit.value=1;
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
        <div className="grid md:grid-cols-2 w-[80%] mx-auto justify-center mt-7">
            {/** AI reccomendation*/}
            <p className="text-4xl text-center md:col-span-2">AI Reccomendation</p>
            <div className="font-sans md:col-span-2 mb-10 text-xl text-center habitBorder p-3 max-w-150 mx-auto min-w-65 overflow-y-auto max-h-40">
                {aiReccomendation}
            </div>
            {/**Active and non active habits */}
            <div className="w-75 md:w-90 mx-auto">
                <h1 className="border-b-4 lg:border-b-6 text-5xl lg:text-7xl text-center w-65 lg:w-85 mx-auto mb-5">Active Habits</h1>
                <div className="sm:overflow-y-auto sm:h-[70vh] p-2 flex flex-col gap-y-4">
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
                            image={<img src="/Add.svg" alt="editIcon" className="h-7 w-7 ml-[0.45rem] my-[0.45rem]"/>}
                        />
                    }
                </div>
            </div>
            <div className="w-75 md:w-90 mx-auto mt-15 md:mt-0">
                <h1 className="border-b-4 lg:border-b-6 text-5xl lg:text-7xl w-65 lg:w-85 text-center mb-5 mx-auto">NonActive Habits</h1>
                <div className="sm:overflow-y-auto sm:h-[70vh] flex flex-col gap-y-4">
                    {nonActiveHabits.map((habit)=>
                        <RenderNonActiveHabits 
                            key={habit.name} 
                            habit={habit}
                            setActiveHabits={setActiveHabits}
                            setNonActiveHabits={setNonActiveHabits}
                            username={username}
                        />
                    )}
                </div>
            </div>
        </div>
    );
}

export default MyHabits;