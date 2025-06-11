import { useEffect, useState } from "react";
import Habit from "../types/Habit";
import HabitService from "../services/HabitService";
import HistoricalData from "../types/HistoricalData";
import HabitStatisticService from "../services/HabitStatisticService";
import Waiting from "../components/Waiting";
import AllHabits from "../components/Statistics/AllHabits";
import HabitData from "../components/Statistics/HabitData";
import HabitDataByMonth from "../components/Statistics/HabitDataByMonth";
const Statistics = ()=>{
    const [activeHabits, setActiveHabits] = useState<Habit[]>([]);
    const [nonActiveHabits, setNonActiveHabits] = useState<Habit[]>([]);
    const [historicalData, setHistoricalData] = useState<HistoricalData>();
    const [totalValuesByMonth, setTotalValuesByMonth] = useState<Record<string,number>>({});
    
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

    const handleHabitSelection = async (habitId:string)=>{
        await fetchHistoricalData(habitId);
        await fetchTotalValueByMonth(habitId,0);
    }

    return(
        <div className="flex w-[75%] mx-auto my-10 justify-between">
            <AllHabits activeHabits={activeHabits} nonActiveHabits={nonActiveHabits} handleHabitSelection={handleHabitSelection}/>
            <div className="w-150">
                <HabitData historicalData={historicalData}/>
                <HabitDataByMonth totalValuesByMonth={totalValuesByMonth} historicalData={historicalData}/>
            </div>
            {historicalData===undefined && <Waiting/>}
        </div>
    );
}

export default Statistics;