import { useEffect, useState } from "react";
import HabitService from "../service/HabitService";
import Habit from "../types/Habit";
import ImageButton from "../components/ImageButton";
import DailyHabit from "./DailyHabit";


//not technically needed as a function but makes code more readable
const CreateHabit = (props: {setHabit: React.Dispatch<React.SetStateAction<Habit>> })=>{
    const {setHabit} = props;
    const [valueSelection, setValueSelection] = useState<React.ReactNode>(<></>)

    const handleBooleanHabit = ()=>{
        setValueSelection(<></>);
    }
    const handleTimeHabit = ()=>{
        setValueSelection(
            <div className="grid grid-cols-2 grid-rows-2">
                <label htmlFor="value" className="font-hand text-4xl text-left col-span-2 border-b border-black">Value: </label>
                <input id="timeValue" type="number" name="type" className="text-2xl text-center border-2 shadow-xl rounded-2xl"/>
                <div className="font-hand text-4xl border-2 shadow-xl rounded-2xl">Minutes</div>
            </div>
        );
    }
    const handleNumericHabit = ()=>{
        setValueSelection(
            <div className="grid grid-cols-2 grid-rows-2 gap-x-1">
                <label htmlFor="numericValue" className="font-hand text-4xl text-left col-span-2 border-b border-black">Value: </label>
                <input id="numericValue" type="number" name="type" className="text-2xl text-center border-2 shadow-xl rounded-2xl"/>
                <input id="valueUnitType" className="text-2xl text-center border-2 shadow-xl rounded-2xl"/>
            </div>
        );
    }

    //NO SWITCH STATEMENTS RAHHHHHHH (that's a code smell - Bobby <-- If you're an employer thats my prof)
    const typeChangeEvents: Record<string, ()=>void> = {
        boolean: handleBooleanHabit, 
        time: handleTimeHabit, 
        numeric: handleNumericHabit
    };


    const handleTypeChange = (event:string)=>{
        const handler = typeChangeEvents[event];
        handler();
    }


    return(
        <div className="grid">
            <label htmlFor="name" className="font-hand text-4xl text-left">{"Name: "}</label>
            <input 
                id="name"
                name="name" 
                className="resize-none border-2 shadow-xl rounded-2xl text-xl h-8 pl-3 " 
                onChange={(e) => {
                    setHabit((prevHabit: Habit) => ({
                    ...prevHabit,
                    name: e.target.value,
                    }));
                }}
            />
            <div className="grid">
                <label htmlFor="type" className="font-hand text-4xl text-left">Type: </label>
                <select 
                    id="type" 
                    name="type" 
                    className="border-2 shadow-xl rounded-2xl text-xl h-8 pl-3" 
                    onChange={(e)=>handleTypeChange(e.target.value)}
                >
                    <option value="boolean">Boolean</option>
                    <option value="time">Time</option>
                    <option value="numeric">Numeric</option>
                </select>
                {valueSelection}
            </div>
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

    //all used for creating new habit UI
    const [addHabit, setAddHabit] = useState<React.ReactNode>(<></>);
    const [inEditMode,setInEditMode] = useState<boolean>(false);
    const [newHabit, setNewHabit] = useState<Habit>(defaultHabit);

    const addHabitButton = <ImageButton className="mx-auto" onClick={()=>setAddHabit(<CreateHabit setHabit={()=>setNewHabit}/>)}
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
            <div className="flex flex-col  mx-auto">
                <div className="grid grid-cols-2 text-center gap-x-2 w-[60%] mx-auto mt-10 gap-y-10">
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