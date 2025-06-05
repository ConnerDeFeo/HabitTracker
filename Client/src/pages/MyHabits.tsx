import { useEffect, useState } from "react";
import Habit from "../types/Habit";
import HabitService from "../services/HabitService";
import ImageButton from "../components/ImageButton";
import CreateHabit from "../components/CreateHabit";

const RenderHabit = (props:{habit:Habit, active:boolean, setHabits:React.Dispatch<React.SetStateAction<Habit[]>>})=>{
    const {habit,active,setHabits} = props;
    const typeConverstion: Record<number,string> = {1:"Binary",2:"Time",3:"Numeric"};
    const [inChangeMode,setInChangeMode] = useState<boolean>(false);

    const getDaysActiveTitle = (habit:Habit):string =>{
        const daysActive: string[] = habit.daysActive;
        if(daysActive.length==7)
            return "Daily";
        const includesSaturday = daysActive.includes("Saturday");
        const includesSunday = daysActive.includes("Sunday");
        if(daysActive.length==5 && !includesSaturday && !includesSunday)
            return "Weekdays";
        if(daysActive.length==2 && includesSaturday && includesSunday)
            return "Weekends";
        const order: string[] = ["Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"];
        //filter by days that are actually there, then convert to 3 letter abreviations
        return order.filter(day => daysActive.includes(day)).map((day)=>day.slice(0,3)).join(", ");
    }

    const handleMinusClick = ()=>{
        if(active){
            return;
        }
        return;
    }

    const handleHabitEditCompletion = async (habit:Habit)=>{
        const resp = await HabitService.editHabit(habit);
        const newHabit = await resp.json();
    
        setHabits((prevHabits) =>
            prevHabits.map((h) => (h.id === newHabit.id ? newHabit : h))
        );
        setInChangeMode(false);
    }

    return(
        inChangeMode ? 
            active ? 
                <CreateHabit handleCancelation={()=>setInChangeMode(false)} handleHabitCompletion={handleHabitEditCompletion} initialHabit={habit}/>
                :
                <div className="habitBorder p-3 grid gap-y-4">
                    <div className="flex justify-between">
                        {active ? 
                            <img src="EditHabits.svg" alt="edit" className="h-6 w-6 cursor-pointer" onClick={()=>setInChangeMode(true)}/>
                            :
                            <ImageButton/>
                        }
                        <p className="text-2xl">{getDaysActiveTitle(habit)}</p>
                        <img src="Minus.png" alt="removeHabit" className="h-6 w-6 cursor-pointer" onClick={handleMinusClick}/>
                    </div>
                    <p className="text-4xl text-center">{habit.name}</p>
                    <div className="flex justify-between">
                        <p className="text-2xl">Date created: {habit.dateCreated}</p>
                        <p className="text-2xl">Type: {typeConverstion[habit.type]}</p>
                    </div>
                </div>
            :
            <div className="habitBorder p-3 grid gap-y-4">
                <div className="flex justify-between">
                    {active ? 
                        <img src="EditHabits.svg" alt="edit" className="h-6 w-6 cursor-pointer" onClick={()=>setInChangeMode(true)}/>
                        :
                        <ImageButton/>
                    }
                    <p className="text-2xl">{getDaysActiveTitle(habit)}</p>
                    <img src="Minus.png" alt="removeHabit" className="h-6 w-6 cursor-pointer" onClick={handleMinusClick}/>
                </div>
                <p className="text-4xl text-center">{habit.name}</p>
                <div className="flex justify-between">
                    <p className="text-2xl">Date created: {habit.dateCreated}</p>
                    <p className="text-2xl">Type: {typeConverstion[habit.type]}</p>
                </div>
            </div>
        
    );
}

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