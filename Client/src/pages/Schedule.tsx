import { useEffect, useState } from "react";
import HabitService from "../services/HabitService";
import HistoricalDate from "../types/HistoricalDate";

const Schedule = ()=>{
    const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    const months = ["January","February","March","April","May","June","July","August","September","October","November","December"];

    const [monthlyHabits, setMonthlyHabits] = useState<Record<string,HistoricalDate>>();
    const [year, setYear] = useState<number>(new Date().getFullYear());
    const [month, setMonth] = useState<number>(new Date().getMonth());
    const firtDayOfMonth = new Date(year, month, 1).getDay();
    const daysInMonth = new Date(year, month, 0).getDate();

    //This will fetch the monthl habits
    useEffect(()=>{
        const fetchMonth = async ()=>{
            setMonthlyHabits({});
            const yyyyMM: string = `${year}-${String(month+1).padStart(2,'0')}`;

            const resp = await HabitService.getMonth(yyyyMM);
            if(resp.status==200){
                const habits = await resp.json();
                setMonthlyHabits(habits);
            }
        }

        fetchMonth();
    },[month])

    //Depending on weather all habits were completed for a give day, return respective image
    const renderDay = (number:number):React.ReactNode=>{
        const day = monthlyHabits?.[number];
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
        let newMonth = month-1;

        if(newMonth<0){
            setYear(year-1);
            newMonth=11;
        }
        setMonth(newMonth);
    }

    const handleTimeIncrease = ()=>{
        const newMonth = month+1;
        const remainder = newMonth/12;

        if(remainder>=1){
            setYear(year+1);
        }
        setMonth(newMonth%12);
    }

    return(
        <div className="relative">
            <p className="text-9xl absolute left-35 top-50 cursor-pointer" onClick={handleTimeDecrease}>{"<"}</p>
            <p className="text-6xl w-[68%] mx-auto text-left mt-8 mb-2">{`${months[month]} ${year}`}</p>
            <div className="grid grid-cols-7 grid-rows-7 max-w-[75%] mx-auto justify-items-center">   
                {/*Row span down one for all days prior to the first day to give that calender look */}
                {days.map((day,i)=><p className={"text-4xl "+(i < firtDayOfMonth && "row-span-2")} key={day}>{day.substring(0,3)}</p>)}

                {Array.from({ length: daysInMonth }, (_, i) => i+1).map((number) => (
                    <div key={number} className="border-2 border-black rounded-sm border-black mb-5 cursor-pointer relative h-15 w-15">
                        <p className="text-3xl text-center">{number}</p>
                        <p>{renderDay(number)}</p>
                    </div>
                ))}
            </div>
            <p className="text-9xl absolute right-35 top-50 cursor-pointer" onClick={handleTimeIncrease}>{">"}</p>
        </div>
    );
}

export default Schedule;