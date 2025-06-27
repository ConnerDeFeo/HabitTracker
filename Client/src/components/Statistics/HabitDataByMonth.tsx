import { useState } from "react";
import DateData from "../../data/DateData";
import DateService from "../../services/DateService";
import HabitHistoryService from "../../services/HabitHistoryService";
import DateInfo from "../../types/DateInfo";
import HistoricalDate from "../../types/HistoricalDate";
import HistoricalData from "../../types/HistoricalData";

//Interactive calender on the statistics page in the statistics page
const HabitDataByMonth = (
    props:{
        totalValuesByMonth: Record<string,number>,
        historicalData?: HistoricalData
        date:DateInfo,
        setDate: React.Dispatch<React.SetStateAction<DateInfo>>,
        smallScreen:boolean
    }
)=>{
    const {totalValuesByMonth,historicalData,date,setDate,smallScreen} = props;

    //Only defined when user clicks on one of the months, acts as its own flag for rendering in this way
    const [monthlyHabits, setMonthlyHabits] = useState<Record<string,HistoricalDate>>();

    //Date created in object form for the currently viewed habit
    const parsedHabitCreatedDate = new Date(historicalData?.habit.dateCreated+"T00:00:01");
    const firtDayOfMonth = new Date(date.year, date.month, 1).getDay();

    /*Compares the current month being rendered to the 
    creation date of the habit and today to see if the given month is valid for
    a date that the current habit could have existed in*/
    const compareMonth = (index: number): boolean => {
        const today = new Date();
        const target = new Date(date.year, index, 1);

        // Check if target month is before habit creation and after current month
        return (
            target >= new Date(parsedHabitCreatedDate.getFullYear(), parsedHabitCreatedDate.getMonth(), 1) &&
            target <= new Date(today.getFullYear(), today.getMonth(), 1)
        );
    };

    /*When user clicks on a month in the initial panel, this is dwhat displays the 
    individual days in the month with the respecitve colorings*/
    const renderMonth = ()=>{
        const daysInMonth:number = new Date(date.year,date.month,0).getDate();
        const habitId:string = historicalData?.habit.id! || "";
        return (
            <>
                {Array.from({ length: daysInMonth }, (_, i) => i + 1).map((day) => {
                    /*Basically just figuring out weather the date was completed, the 
                    hierarchy gos completed --> not completed --> non active date. Note 
                    that if a user did complete a habit on a date, then changed the active 
                    dates to no longer include that date, the previous dates will still 
                    be counted towards long term statistics */
                    const containsHabit = monthlyHabits?.[DateService.padZero(day)]?.habits?.[habitId];
                    const dayOfWeek = DateData.days[new Date(date.year,date.month,day).getDay()];
                    const containsDay = historicalData?.habit.daysActive.includes(dayOfWeek);

                    let bgClass = "";

                    if (containsHabit) {
                        bgClass = containsHabit.completed ? "bg-green-500" : "bg-red-500";
                    } else if (!containsDay) {
                        bgClass = "bg-gray-500";
                    }
                    return (
                        <div
                            key={day}
                            className={
                                `border-2 border-black rounded-sm mb-5 relative h-5 w-5 md:h-10 md:w-10 shadow-md shadow-black ${bgClass}`  
                            }
                        ></div>
                    );
                })}
            </>
        );
    }

    //When user clicks on a month
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
        <div className="habitBorder h-fit md:h-170 pb-10">
            {monthlyHabits ?
                //Month selected
                <div>
                    <div className="flex justify-between text-xl md:text-4xl w-[80%] mx-auto relative mt-4">
                        <img src="./BasicArrow.png" className="rotate-180 w-7 h-7 cursor-pointer md:mt-7" onClick={()=>setMonthlyHabits(undefined)}/>
                        <p className="text-3xl md:text-5xl md:mt-5">{DateData.months[date.month]}</p>
                        <p className="text-3xl md:text-5xl md:mt-5">{date.year}</p>
                    </div>
                    <div className="grid grid-cols-7 justify-items-center w-[80%] mx-auto mt-[10%]">
                        {DateData.days.map((day,i)=>
                            <p className={"text-2xl md:text-4xl "+(i < firtDayOfMonth && "row-span-2")} 
                                key={day}
                            >
                                {day.substring(0,1)}
                            </p>
                        )}
                        {renderMonth()}
                    </div>
                </div>
                :
                //Initial pane shown
                <>
                    {/*Header to the bottom right pane   */}
                    <div className="flex justify-between text-2xl w-[80%] mx-auto relative mt-4 items-center">
                        <img 
                            src="./BasicArrow.png" 
                            className="rotate-180 h-7 w-7 cursor-pointer" 
                            onClick={()=>setDate((prevDate)=>({...prevDate,year:date.year-1}))}
                        />
                        {date.year}
                        <img 
                            src="./BasicArrow.png" 
                            className="h-7 w-7 cursor-pointer" 
                            onClick={()=>setDate((prevDate)=>({...prevDate,year:date.year+1}))}
                        />
                    </div>
                    {/*All the different months */}
                    <div className="grid grid-cols-3 gap-y-3 w-[80%] mx-auto text-center mt-10">
                        {DateData.months.map((month,index)=>
                            <div 
                                key={month} 
                                className="shadow-md mx-auto border-2 border-dashed rounded-md h-15 md:h-30 w-15 md:w-30 my-autogrid items-center cursor-pointer dropShadow"
                                onClick={()=>handleMonthSelection(index)}
                            >
                                <p className="text-2xl md:text-4xl md:mt-5">{smallScreen ? month.substring(0,3) : month}</p>
                                <p className="text-lg md:text-2xl">
                                    {compareMonth(index) &&
                                        //determines what follows the number, the unit of the habit
                                        `${totalValuesByMonth[month] || 0}\n
                                        ${historicalData?.habit.type ===1 ? "Days" : ""}`
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