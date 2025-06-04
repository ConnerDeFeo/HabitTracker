import { useEffect, useState } from "react";
import HabitService from "../services/HabitService";
import Habit from "../types/Habit";
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


    useEffect(()=>{
        fetchMonth();
    },[habits])

    //On render grab the users habits
    useEffect(()=>{
        const fetchHabits = async ()=>{
            //month+1 becasue month is 0 indexed based
            const resp = await HabitService.getHabits(`${date.year}-${DateService.padZero(date.month+1)}-${DateService.padZero(date.day)}`);
            if(resp.status===200){
                const habits = await resp.json();
                setHabits(habits);
            }
            else{
                setHabits([]);
            }
        }
        fetchHabits();
    },[date])

    return(
        <div className="flex flex-col mx-auto mb-[50vh]">
            <div className="flex justify-between items-center w-[75%] mx-auto mt-8">
                <p className="text-6xl">{`${DateData.months[date.month]} ${date.day}${postFix}, ${date.year}`}</p>
                <Button label="Go to Today" onClick={()=>{setDate({day:today.getDate(), month: today.getMonth(), year: today.getFullYear()})}}/>
            </div>
            <Arrow onClick={()=>setDate(DateService.decreaseDay(date))} className="mt-10"/>
            <div className="grid grid-cols-2 text-center gap-x-2 w-[60%] mx-auto mt-10 gap-y-10" >
                {habits.map((habit)=><HabitComponent key={habit.id} habit={habit} setHabits={setHabits} date={date}/>)}
            </div>
            <Arrow onClick={()=>{setDate(DateService.increaseDay(date))}} inverse={true} className="mt-10" show={!dateIsToday}/>      
        </div>      
    );
}

export default HabitCheckList;