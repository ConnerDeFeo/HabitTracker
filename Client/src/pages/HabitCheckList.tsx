import { useEffect, useState } from "react";
import HabitService from "../services/HabitService";
import Habit from "../types/Habit";
import DateInfo from "../types/DateInfo";
import DateService from "../services/DateService";
import DateData from "../data/DateData";
import Arrow from "../components/Arrow";
import Button from "../components/Button";
import HabitHistoryService from "../services/HabitHistoryService";

const HabitCheckList = (props:{date:DateInfo, fetchMonth: ()=>void, setDate: React.Dispatch<React.SetStateAction<DateInfo>>})=>{
    const {date, fetchMonth, setDate} = props;
    const today = new Date();
    const postFixes: Record<number,string> = {1:"rst", 2:"nd", 3:"rd", 4:"rth"};
    const postFix = postFixes[date.day%10]==undefined ? "th" : postFixes[date.day%10];
    const dateIsToday = today.getFullYear()===date.year && today.getMonth() === date.month && today.getDate()===date.day;
    const [habits,setHabits] = useState<Habit[]>([]);
    const dayInStringFormat = `${date.year}-${DateService.padZero(date.month+1)}-${DateService.padZero(date.day)}`;

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
    },[date]);

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
                <p className="text-6xl">{`${DateData.months[date.month]} ${date.day}${postFix}, ${date.year}`}</p>
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
                {habits.map((habit)=><p key={habit.name} className="text-4xl cursor-pointer">{habit.name}</p>)}
            </div>
            <Arrow onClick={()=>{setDate(DateService.increaseDay(date))}} inverse={true} className="mt-10" show={!dateIsToday}/>      
        </div>      
    );
}

export default HabitCheckList;