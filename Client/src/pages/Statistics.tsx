import { useEffect, useState } from "react";
import Habit from "../types/Habit";
import HabitService from "../services/HabitService";
import HistoricalData from "../types/HistoricalData";
import HabitStatisticService from "../services/HabitStatisticService";
const Statistics = ()=>{
    const [activeHabits, setActiveHabits] = useState<Habit[]>([]);
    const [nonActiveHabits, setNonActiveHabits] = useState<Habit[]>([]);
    const [historicalData, setHistoricalData] = useState<HistoricalData>();
    const [totalValuesByMonth, setTotalValuesByMonth] = useState<Record<string,number>>({});
    const [currentDate, setCurrentDate] = useState<Date>(new Date());
    const habitNameStyling = "text-4xl text-center my-7 cursor-pointer"
    const months = ["January","February","March","April","May","June","July","August","September","October","November","December"];

    
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

    return(
        <div className="flex w-[75%] mx-auto my-10 justify-between">
            <div className="habitBorder w-75 h-100 overflow-y-auto">
                {activeHabits.map((habit)=>
                    <p 
                        key={habit.name} 
                        className={habitNameStyling}
                        onClick={async ()=>{
                            await fetchHistoricalData(habit.id!);
                            await fetchTotalValueByMonth(habit.id!,0);
                        }}
                    >{habit.name}</p>
                )}
                {nonActiveHabits.map((habit)=>
                    <p 
                        key={habit.name} 
                        className={habitNameStyling}
                        onClick={async ()=>{
                            await fetchHistoricalData(habit.id!);
                            await fetchTotalValueByMonth(habit.id!,0);
                        }}
                    >{habit.name}</p>
                )}
            </div>
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
                <div className="habitBorder h-70">
                    <div className="flex justify-between text-2xl w-[80%] mx-auto relative mt-4 ">
                        <img src="./BasicArrow.png" className="rotate-180 h-7 w-7 cursor-pointer" />
                        {currentDate.getFullYear()}
                        <img src="./BasicArrow.png" className="h-7 w-7 cursor-pointer"/>
                    </div>
                    <div className="overflow-y-auto h-50 grid grid-cols-3 gap-y-3 w-[80%] mx-auto text-center">
                        {months.map((month,index)=>
                            <div key={month} className="border-2 border-dashed rounded-md h-30 w-30 my-autogrid items-center cursor-pointer dropShadow">
                                <p className="text-4xl mt-5">{month}</p>
                                <p className="text-2xl">
                                    {totalValuesByMonth[month] || "0"}
                                    {
                                        historicalData?.habit.type ===1 ? " Days" 
                                        : 
                                        " "+historicalData?.habit.valueUnitType
                                    }
                                </p>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Statistics;