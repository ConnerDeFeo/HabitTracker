import { useEffect, useState } from "react";
import HabitService from "../services/HabitService";
import HistoricalDate from "../types/HistoricalDate";
import DateInfo from "../types/DateInfo";
import { useNavigate } from "react-router-dom";
import GeneralService from "../services/GeneralService";
import Arrow from "../components/Arrow";

const Schedule = (props:{setDate: React.Dispatch<React.SetStateAction<DateInfo>>, monthlyHabits?: Record<string,HistoricalDate>, date: DateInfo})=>{
    const {setDate,monthlyHabits, date} = props;
    const navigate = useNavigate();
    const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    const months = ["January","February","March","April","May","June","July","August","September","October","November","December"];

    const firtDayOfMonth = new Date(date.year, date.month, 1).getDay();
    const daysInMonth = new Date(date.year, date.month+1, 0).getDate();


    //Depending on weather all habits were completed for a give day, return respective image
    const renderDay = (number:number):React.ReactNode=>{
        const day = monthlyHabits?.[GeneralService.padZero(number)];
        if(day !== undefined){
            if(day?.allHabitsCompleted){
                return <img src="./checkMark.webp" alt="Monthly Habit CheckMark" className="h-7 w-7 absolute right-[0.9rem] bottom-1"/>;
            }
            else{
                return <img src="./RedX.png" alt="Monthly Habit CheckMark" className="h-6 w-6 absolute right-4 bottom-1"/>;
            }
        }
        return <></>;
    }

    const handleTimeDecrease = ()=>{
        let newMonth = date.month-1;
        let newYear = date.year; 

        if(newMonth<0){
            newYear = date.year-1;
            newMonth=11;
        }
        setDate(({
            day: date.day,
            month: newMonth,
            year: newYear
        }));
    }

    const handleTimeIncrease = ()=>{
        const newMonth = date.month+1;
        const remainder = newMonth/12;
        let newYear = date.year;

        if(remainder>=1)
            newYear = date.year+1;
        
        setDate(({
            day: date.day,
            month: newMonth%12,
            year: newYear
        }));
    }

    const handleDateSelection = (day: number)=>{
        setDate(({
            day: day,
            month: date.month,
            year: date.year
        }));
        navigate("/");
    }

    return(
        <div className="relative">
            <Arrow onClick={handleTimeDecrease}/>
            <p className="text-6xl w-[68%] mx-auto text-left mt-8 mb-2">{`${months[date.month]} ${date.year}`}</p>
            <div className="grid grid-cols-7 grid-rows-7 max-w-[75%] mx-auto justify-items-center">   
                {/*Row span down one for all days prior to the first day to give that calender look */}
                {days.map((day,i)=><p className={"text-4xl "+(i < firtDayOfMonth && "row-span-2")} key={day}>{day.substring(0,3)}</p>)}

                {Array.from({ length: daysInMonth }, (_, i) => i+1).map((day) => (
                    <div key={day} className="border-2 border-black rounded-sm border-black mb-5 cursor-pointer relative h-15 w-15 dropShadow"
                        onClick={()=>handleDateSelection(day)}
                    >
                        <p className="text-3xl text-center">{day}</p>
                        <p>{renderDay(day)}</p>
                    </div>
                ))}
            </div>
            <Arrow onClick={handleTimeIncrease} inverse={true}/>
        </div>
    );
}

export default Schedule;