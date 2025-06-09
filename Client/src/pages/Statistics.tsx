import { useEffect, useState } from "react";
import Habit from "../types/Habit";
import HabitService from "../services/HabitService";
import HistoricalData from "../types/HistoricalData";

const Statistics = ()=>{
    const [activeHabits, setActiveHabits] = useState<Habit[]>([]);
    const [nonActiveHabits, setNonActiveHabits] = useState<Habit[]>([]);
    const [historicalData, setHistoricalData] = useState<HistoricalData>();
    const habitNameStyling = "text-4xl text-center my-7 cursor-pointer"

    useEffect(()=>{
        const fetchHabits = async()=>{
            const resp = await HabitService.getExistingHabits();
            if(resp.status==200){
                const existingHabits = await resp.json();
                setActiveHabits(existingHabits["ActiveHabits"]);
                setNonActiveHabits(existingHabits["NonActiveHabits"]);
            }
            if(activeHabits.length>0){
            }
            else if(activeHabits.length>0){
                
            }
        }
        fetchHabits();
    },[]);

    return(
        <div className="flex w-[75%] mx-auto my-10 justify-between">
            <div className="habitBorder w-75 h-100 overflow-y-auto">
                {activeHabits.map((habit)=><p className={habitNameStyling}>{habit.name}</p>)}
                {nonActiveHabits.map((habit)=><p className={habitNameStyling}>{habit.name}</p>)}
            </div>
            <div>
                <div className=" h-25 mb-5">
                    {historicalData === undefined ? 
                        <p className="text-6xl text-center">No habits to view!</p>
                        :
                        <></>
                    }
                </div>
                <div className="habitBorder w-150 h-70">
                    {historicalData !== undefined &&
                        <></>
                    }
                </div>
            </div>
        </div>
    );
}

export default Statistics;