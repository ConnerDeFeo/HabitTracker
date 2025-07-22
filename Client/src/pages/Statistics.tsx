import { useEffect, useState } from "react";
import Habit from "../types/Habit";
import HabitService from "../services/HabitService";
import HistoricalData from "../types/HistoricalData";
import HabitStatisticService from "../services/HabitStatisticService";
import AllHabits from "../components/Statistics/AllHabits";
import HabitData from "../components/Statistics/HabitData";
import HabitDataByMonth from "../components/Statistics/HabitDataByMonth";
import DateInfo from "../types/DateInfo";
import DateService from "../services/DateService";

//Statistics page, obviously
const Statistics = (props:{smallScreen:boolean})=>{
    const {smallScreen} = props;

    const [activeHabits, setActiveHabits] = useState<Habit[]>([]);
    const [nonActiveHabits, setNonActiveHabits] = useState<Habit[]>([]);
    const [historicalData, setHistoricalData] = useState<HistoricalData>();
    const [totalValuesByMonth, setTotalValuesByMonth] = useState<Record<string,number>>({});

    const [date, setDate] = useState<DateInfo>(() => {
        const now = DateService.getUtcToday();
        return{
            year: now.getUTCFullYear(),
            month: now.getUTCMonth(), 
            day: now.getUTCDate()
        };
    });
    
    //Historical data in relation to streaks and total value completed
    const fetchHistoricalData = async (habitId:string)=>{
        const resp = await HabitStatisticService.getHistoricalData(habitId);
        if(resp.status==200){
            const data: HistoricalData = await resp.json();
            setHistoricalData(data);
        }
    }

    //This is the total value completed per month for the current year in relation to the habit
    const fetchTotalValueByMonth = async (habitId:string, year:number)=>{
        const resp = await HabitStatisticService.getTotalValueByMonth(habitId,year);
        if(resp.status==200){
            const totalValueByMonth: Record<string,number> = await resp.json();
            // fetchAiRecommendation(JSON.stringify(totalValueByMonth));
            setTotalValuesByMonth(totalValueByMonth);
        }
    }

    //Grabs habits for the current year along with their respective data
    useEffect(()=>{
        const fetchHabits = async()=>{
            const resp = await HabitService.getExistingHabits();

            if(resp.status==200){
                const existingHabits = await resp.json();
                const active = existingHabits["ActiveHabits"];
                const nonActive = existingHabits["NonActiveHabits"];

                setActiveHabits(active);
                setNonActiveHabits(nonActive);

                if(active.length>0){
                    const activeId = active[0].id!;
                    await fetchHistoricalData(activeId);
                    await fetchTotalValueByMonth(activeId,date.year);
                }
                else if(nonActive.length>0){
                    const nonActiveId = nonActive[0].id!;
                    await fetchHistoricalData(nonActiveId);
                    await fetchTotalValueByMonth(nonActiveId,date.year);
                }
            }
        }
        fetchHabits();
    },[]);

    //anytime the date is updated so should the respective dates
    useEffect(()=>{
        if(historicalData)
            fetchTotalValueByMonth(historicalData?.habit.id!,date.year);
        
    },[date.year]);

    //On selection of one of the habits in the AllHabits component
    const handleHabitSelection = async (habitId:string)=>{
        await fetchHistoricalData(habitId);
        await fetchTotalValueByMonth(habitId,date.year);
    }

    return(
        <div className="grid lg:flex w-[80%] xl:w-[70%] 2xl:w-[60%] mx-auto mt-10 justify-center lg:justify-between">
            <AllHabits 
                activeHabits={activeHabits} 
                nonActiveHabits={nonActiveHabits} 
                handleHabitSelection={handleHabitSelection} 
                selectedHabitId={historicalData?.habit.id!}
                smallScreen={smallScreen}
            />
            <div className="w-[80vw] min-w-35 md:w-150">
                <HabitData historicalData={historicalData}/>
                <HabitDataByMonth 
                    totalValuesByMonth={totalValuesByMonth} 
                    historicalData={historicalData} 
                    date={date} 
                    setDate={setDate}
                    smallScreen = {smallScreen}
                />
            </div>
        </div>
    );
}

export default Statistics;