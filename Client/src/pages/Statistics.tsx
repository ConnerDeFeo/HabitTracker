import { useEffect, useState } from "react";
import Habit from "../types/Habit";
import HabitService from "../services/HabitService";
import HistoricalData from "../types/HistoricalData";
import HabitStatisticService from "../services/HabitStatisticService";
import Waiting from "../components/Waiting";
import HistoricalDate from "../types/HistoricalDate";
import DateService from "../services/DateService";
import HabitHistoryService from "../services/HabitHistoryService";
import DateData from "../data/DateData";
import DateInfo from "../types/DateInfo";
import AllHabits from "../components/Statistics/AllHabits";
const Statistics = ()=>{
    const [activeHabits, setActiveHabits] = useState<Habit[]>([]);
    const [nonActiveHabits, setNonActiveHabits] = useState<Habit[]>([]);
    const [historicalData, setHistoricalData] = useState<HistoricalData>();
    const [totalValuesByMonth, setTotalValuesByMonth] = useState<Record<string,number>>({});
    const [date, setDate] = useState<DateInfo>(() => {
        const now = new Date();
        return{
            year: now.getFullYear(),
            month: now.getMonth(), 
            day: now.getDate()
        };
    });
    const [monthlyHabits, setMonthlyHabits] = useState<Record<string,HistoricalDate>>();

    const parsedHabitCreatedDate = new Date(historicalData?.habit.dateCreated+"T00:00:01");
     const firtDayOfMonth = new Date(date.year, date.month, 1).getDay();
    
    const fetchHistoricalData = async (habitId:string)=>{
        const resp = await HabitStatisticService.getHistoricalData(habitId);
        if(resp.status==200){
            const data: HistoricalData = await resp.json();
            setHistoricalData(data);
        }
    }

    const fetchTotalValueByMonth = async (habitId:string, yearsBack:number)=>{
        const resp = await HabitStatisticService.getTotalValueByMonth(habitId,yearsBack);
        if(resp.status==200){
            const totalValueByMonth: Record<string,number> = await resp.json();
            setTotalValuesByMonth(totalValueByMonth);
        }
    }

    useEffect(()=>{
        const fetchHabits = async()=>{
            const resp = await HabitService.getExistingHabits();

            if(resp.status==200){
                const existingHabits = await resp.json();
                const active = existingHabits["ActiveHabits"];
                const nonActive = existingHabits["NonActiveHabits"];

                setActiveHabits(existingHabits["ActiveHabits"]);
                setNonActiveHabits(existingHabits["NonActiveHabits"]);

                if(active.length>0){
                    await fetchHistoricalData(active[0].id!);
                    await fetchTotalValueByMonth(active[0].id!,0);
                }
                else if(nonActive.length>0){
                    await fetchHistoricalData(nonActive[0].id!);
                    await fetchTotalValueByMonth(nonActive[0].id!,0);
                }
            }
        }
        fetchHabits();
    },[]);

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
        return (
            <>
                {Array.from({ length: daysInMonth }, (_, i) => i+1).map((day) => (
                    <div 
                        key={day} 
                        className="border-2 border-black rounded-sm border-black mb-5 relative h-5 w-5 shadow-md shadow-black"
                    >
                    </div>
                ))}
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

    const handleHabitSelection = async (habitId:string)=>{
        await fetchHistoricalData(habitId);
        await fetchTotalValueByMonth(habitId,0);
    }

    return(
        <div className="flex w-[75%] mx-auto my-10 justify-between">
            <AllHabits activeHabits={activeHabits} nonActiveHabits={nonActiveHabits} handleHabitSelection={handleHabitSelection}/>
            <div className="w-150">
                <div className="h-25 mb-5">
                    {historicalData === undefined ? 
                        <p className="text-6xl text-center">No habits to view!</p>
                        :
                        <div className="flex justify-between w-[90%] mx-auto">
                            <div>
                                <p className="text-4xl mb-5">{historicalData.habit.name}</p>
                                <p className="text-4xl">
                                    {`${historicalData.totalValueCompleted} 
                                    ${historicalData.habit.valueUnitType || "days completed"} 
                                    sense ${historicalData.habit.dateCreated}`}
                                </p>
                            </div>
                            <div>
                                <p className="text-4xl mb-5">Longest Streak: {historicalData.longestStreak}</p>
                                <p className="text-4xl">Current Streak: {historicalData.currentStreak}</p>
                            </div>
                        </div>
                    }
                </div>
                <div className="habitBorder h-80">
                    {monthlyHabits ? 
                        <div>
                            <div className="flex justify-between text-2xl w-[80%] mx-auto relative mt-4 text-4xl">
                                <img src="./BasicArrow.png" className="rotate-180 h-7 w-7 cursor-pointer" />
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
            </div>
            {historicalData===undefined && <Waiting/>}
        </div>
    );
}

export default Statistics;