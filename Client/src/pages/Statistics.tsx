import { useEffect, useState } from "react";
import Habit from "../types/Habit";
import HabitService from "../services/HabitService";
import HistoricalData from "../types/HistoricalData";
import HabitStatisticService from "../services/HabitStatisticService";
import HistoricalDate from "../types/HistoricalDate";
import HabitHistoryService from "../services/HabitHistoryService";
import DateInfo from "../types/DateInfo";

const Statistics = ()=>{
    const [activeHabits, setActiveHabits] = useState<Habit[]>([]);
    const [nonActiveHabits, setNonActiveHabits] = useState<Habit[]>([]);
    const [historicalData, setHistoricalData] = useState<HistoricalData>();
    const habitNameStyling = "text-4xl text-center my-7 cursor-pointer"

    useEffect(()=>{
        const fetchHabits = async()=>{
            const habitResp = await HabitService.getExistingHabits();

            if(habitResp.status==200){
                const existingHabits = await habitResp.json();
                const active = existingHabits["ActiveHabits"];
                const nonActive = existingHabits["NonActiveHabits"];

                setActiveHabits(existingHabits["ActiveHabits"]);
                setNonActiveHabits(existingHabits["NonActiveHabits"]);

                if(active.length>0){
                    const dataResp = await HabitStatisticService.getHistoricalData(active[0].id!);
                    const data: HistoricalData = await dataResp.json();
                    setHistoricalData(data);
                }
                else if(nonActive.length>0){
                    const dataResp = await HabitStatisticService.getHistoricalData(nonActive[0].id!);
                    const data: HistoricalData = await dataResp.json();
                    setHistoricalData(data);
                }
            }
        }
        fetchHabits();
    },[]);

    return(
        <div className="flex w-[75%] mx-auto my-10 justify-between">
            <div className="habitBorder w-75 h-100 overflow-y-auto">
                {activeHabits.map((habit)=><p key={habit.name} className={habitNameStyling}>{habit.name}</p>)}
                {nonActiveHabits.map((habit)=><p key={habit.name} className={habitNameStyling}>{habit.name}</p>)}
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
                                    ${historicalData.habit.valueUnitType} 
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
                    <div className="flex justify-between text-2xl w-[80%] mx-auto">
                        <p>arrow</p>
                        <p>arrow</p>
                        <p>arrow</p>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Statistics;