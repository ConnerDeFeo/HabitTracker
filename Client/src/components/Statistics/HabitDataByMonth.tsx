import { useState } from "react";
import DateData from "../../data/DateData";
import DateService from "../../services/DateService";
import HabitHistoryService from "../../services/HabitHistoryService";
import DateInfo from "../../types/DateInfo";
import HistoricalDate from "../../types/HistoricalDate";
import HistoricalData from "../../types/HistoricalData";
import Habit from "../../types/Habit";

const HabitDataByMonth = (
    props:{
        totalValuesByMonth: Record<string,number>,
        historicalData?: HistoricalData
    }
)=>{
    const {totalValuesByMonth,historicalData} = props;

    const [monthlyHabits, setMonthlyHabits] = useState<Record<string,HistoricalDate>>();
    console.log(monthlyHabits);
    const [date, setDate] = useState<DateInfo>(() => {
        const now = new Date();
        return{
            year: now.getFullYear(),
            month: now.getMonth(), 
            day: now.getDate()
        };
    });

    const parsedHabitCreatedDate = new Date(historicalData?.habit.dateCreated+"T00:00:01");
    const firtDayOfMonth = new Date(date.year, date.month, 1).getDay();
    
    /*Compares the current month being rendered to the 
    creation date of the habit and today to see if the given month is valid for
    a date that the current habit could have existed in*/
    const compareMonth = (index:number):boolean =>{
        const dateBeingChecked:number = new Date(date.year,index,1).getMonth();
        const today:number = new Date().getMonth();
        return dateBeingChecked >=parsedHabitCreatedDate.getMonth() && dateBeingChecked<=today;
    }

    const renderMonth = ()=>{
        const daysInMonth:number = new Date(date.year,date.month,0).getDate();
        const habitId:string = historicalData?.habit.id! || "";
        return (
            <>
                {Array.from({ length: daysInMonth }, (_, i) => i + 1).map((day) => {
                    const containsHabit = monthlyHabits?.[DateService.padZero(day)]?.habits?.[habitId];
                    return (
                        <div
                            key={day}
                            className={
                                "border-2 border-black rounded-sm mb-5 relative h-5 w-5 shadow-md shadow-black " +
                                (containsHabit && (containsHabit.completed ? "bg-green-500" : "bg-red-500"))
                            }
                        ></div>
                    );
                })}
            </>
        );
    }

    async function handleMonthSelection(index:number):Promise<void>{
        const month:string = DateService.padZero(index+1);
        const resp = await HabitHistoryService.getMonth(`${date.year}-${month}`);

        if(resp.status==200){
            const habits:Record<string,HistoricalDate> = await resp.json();
            setMonthlyHabits(habits);
            setDate((prevDate)=>({...prevDate, month:index }));
        }
    }

    return (
        <div className="habitBorder h-80">
            {monthlyHabits ? 
                <div>
                    <div className="flex justify-between text-2xl w-[80%] mx-auto relative mt-4 text-4xl">
                        <img src="./BasicArrow.png" className="rotate-180 h-7 w-7 cursor-pointer" onClick={()=>setMonthlyHabits(undefined)}/>
                        <p>{DateData.months[date.month]}</p>
                        <p>{date.year}</p>
                    </div>
                    <div className="grid grid-cols-7 justify-items-center w-[80%] mx-auto">
                        {DateData.days.map((day,i)=>
                            <p className={"text-2xl "+(i < firtDayOfMonth && "row-span-2")} 
                                key={day}
                            >
                                {day.substring(0,1)}
                            </p>
                        )}
                        {renderMonth()}
                    </div>
                </div>
                :
                <>
                    <div className="flex justify-between text-2xl w-[80%] mx-auto relative mt-4">
                        <img src="./BasicArrow.png" className="rotate-180 h-7 w-7 cursor-pointer" />
                        {date.year}
                        <img src="./BasicArrow.png" className="h-7 w-7 cursor-pointer"/>
                    </div>
                    <div className="overflow-y-auto h-50 grid grid-cols-3 gap-y-3 w-[80%] mx-auto text-center">
                        {DateData.months.map((month,index)=>
                            <div 
                                key={month} 
                                className="border-2 border-dashed rounded-md h-30 w-30 my-autogrid items-center cursor-pointer dropShadow"
                                onClick={()=>handleMonthSelection(index)}
                            >
                                <p className="text-4xl mt-5">{month}</p>
                                <p className="text-2xl">
                                    {compareMonth(index) &&
                                        `${totalValuesByMonth[month] || 0}\n
                                        ${historicalData?.habit.type ===1 ? "Days" 
                                            : 
                                        historicalData?.habit.valueUnitType}`
                                    }
                                </p>
                            </div>
                        )}
                    </div>
                </>
            }
        </div>
    );
}

export default HabitDataByMonth;