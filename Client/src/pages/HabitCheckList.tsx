import { useEffect, useState } from "react";
import HabitService from "../services/HabitService";
import Habit from "../types/Habit";
import DateInfo from "../types/DateInfo";
import DateService from "../services/DateService";
import DateData from "../data/DateData";
import Arrow from "../components/Arrow";
import Button from "../components/Button";
import HabitHistoryService from "../services/HabitHistoryService";

//Base page that the Habit Tracker clickable leads to in the top left
const HabitCheckList = (props:{date:DateInfo, fetchMonth: ()=>void, setDate: React.Dispatch<React.SetStateAction<DateInfo>>})=>{
    const {date, fetchMonth, setDate} = props;

    const [habits,setHabits] = useState<Habit[]>([]);

    const today = new Date();
    //Post fixes for the various days, so 14th, or 1rst for instance
    const postFixes: Record<number,string> = {1:"rst", 2:"nd", 3:"rd", 4:"rth"};
    /*post fix has to bge checked like this due to 11-14 ending with "th" 
    and other numbers ending with 1-4 containing the above post fixes*/
    const postFix = ()=>{
        if(date.day>10 && date.day<15 )
            return "th"
        const fix = postFixes[date.day%10];
        if(fix)
            return fix;
        return "th"
    }
    //flag for if the upper right button "go to today" will be shown
    const dateIsToday = today.getFullYear()===date.year && today.getMonth() === date.month && today.getDate()===date.day;
    //Used for habit completion, string of date in yyyy-MM-dd format to interact with the backend
    const dayInStringFormat = `${date.year}-${DateService.padZero(date.month+1)}-${DateService.padZero(date.day)}`;

    //Anytime a habit is updated, fetch month as it means that habit completion has changed
    useEffect(()=>{
        fetchMonth();
    },[habits]);

    //On render grab the users habits
    useEffect(()=>{
        const fetchHabits = async ()=>{
            //month+1 becasue month is 0 indexed based
            const resp = await HabitService.getHabits(dayInStringFormat);
            if(resp.status===200){
                const habits = await resp.json();
                setHabits(habits);
            }
            else{
                setHabits([]);
            }
        }
        fetchHabits();
    },[date]);// On date change grab users habits as well

    //When habit is completed for the given date
    const handleHabitCompletion = async (habit:Habit, completed: boolean)=>{
        const resp = await HabitHistoryService.completeHabit(habit.id!,dayInStringFormat,completed);

        if(resp.status===200){
            const newHabit: Habit = habit;
            newHabit.completed=completed;
            setHabits(prevHabits=>prevHabits.map((h) => (h.id === newHabit.id ? newHabit : h)));
        }
    }

    return(
        <div className="flex flex-col mx-auto mb-[50vh]">
            <div className="flex justify-between items-center w-[75%] mx-auto mt-8 relative">
                <p className="text-6xl">{`${DateData.months[date.month]} ${date.day}${postFix()}, ${date.year}`}</p>
                <p className="text-6xl absolute left-1/2 -translate-x-1/2">{DateData.days[new Date(date.year,date.month,date.day).getDay()]}</p>
                {
                    !dateIsToday &&
                    <Button 
                        label="Go to Today" 
                        onClick={()=>{setDate({day:today.getDate(), month: today.getMonth(), year: today.getFullYear()})}}
                        className="p-2"
                    />
                }
            </div>
            <Arrow onClick={()=>setDate(DateService.decreaseDay(date))} className="mt-10"/>
            <div className="grid grid-cols-2 text-center gap-x-2 w-[60%] mx-auto mt-10 gap-y-10" >
                {habits.map((habit)=>
                    <p 
                        key={habit.name} 
                        className={"text-4xl cursor-pointer "+(habit.completed && "line-through")} 
                        onClick={()=>handleHabitCompletion(habit,!habit.completed)}>{habit.name}
                    </p>
                )}
            </div>
            <Arrow onClick={()=>{setDate(DateService.increaseDay(date))}} inverse={true} className="mt-10" show={!dateIsToday}/>      
        </div>      
    );
}

export default HabitCheckList;