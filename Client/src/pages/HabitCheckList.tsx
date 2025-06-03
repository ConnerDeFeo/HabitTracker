import { useEffect, useState } from "react";
import HabitService from "../services/HabitService";
import Habit from "../types/Habit";
import ImageButton from "../components/ImageButton";
import CreateHabit from "../components/CreateHabit";
import HabitComponent from "./HabitComponet";
import DateInfo from "../types/DateInfo";
import DateService from "../services/DateService";
import DateData from "../data/DateData";
import Arrow from "../components/Arrow";
import Button from "../components/Button";

const HabitCheckList = (props:{date:DateInfo, fetchMonth: ()=>void, setDate: React.Dispatch<React.SetStateAction<DateInfo>>})=>{
    const {date, fetchMonth, setDate} = props;
    const today = new Date();
    const postFixes: Record<number,string> = {1:"rst", 2:"nd", 3:"rd", 4:"rth"};
    const postFix = postFixes[date.day%10]==undefined ? "th" : postFixes[date.day%10];
    const dateIsToday = today.getFullYear()===date.year && today.getMonth() === date.month && today.getDate()===date.day;

    const [habits,setHabits] = useState<Habit[]>([]);

    //All used for creating new habit UI
    const [addHabit, setAddHabit] = useState<React.ReactNode>(<></>);
    const [inEditMode,setInEditMode] = useState<boolean>(false);

    useEffect(()=>{
        fetchMonth();
    },[habits])

    //On render grab the users habits
    useEffect(()=>{
        const fetchHabits = async ()=>{
            //month+1 becasue month is 0 indexed based
            const resp = await HabitService.getHabits(`${date.year}-${DateService.padZero(date.month+1)}-${DateService.padZero(date.day)}`);
            if(resp.status===200){
                const data = await resp.json();
                setHabits(data);
            }
            else{
                setHabits([]);
            }
        }
        fetchHabits();
    },[date])

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
        <div className="flex flex-col mx-auto mb-[50vh]">
            <div className="flex justify-between items-center w-[75%] mx-auto mt-8">
                <p className="text-6xl">{`${DateData.months[date.month]} ${date.day}${postFix}, ${date.year}`}</p>
                <Button label="Go to Today" onClick={()=>{setDate({day:today.getDate(), month: today.getMonth(), year: today.getFullYear()})}}/>
            </div>
            <Arrow onClick={()=>setDate(DateService.decreaseDay(date))} className="mt-10"/>
            <div className="grid grid-cols-2 text-center gap-x-2 w-[60%] mx-auto mt-10 gap-y-10" >
                {habits.map((habit)=><HabitComponent key={habit.id} habit={habit} inEditMode={inEditMode} setHabits={setHabits} date={date}/>)}
                {/*This will only show if user is in edit mode */}
                {addHabit}
            </div>
            {dateIsToday && 
                <ImageButton onClick={toggleEdit} className="ml-[80%] mt-5 drop-shadow-lg" 
                    image={<img src="./EditHabits.svg" alt="editIcon" className="h-7 w-7 ml-[0.45rem]"/>}/>}
            <Arrow onClick={()=>{setDate(DateService.increaseDay(date))}} inverse={true} className="mt-10" show={!dateIsToday}/>      
        </div>      
    );
}

export default HabitCheckList;