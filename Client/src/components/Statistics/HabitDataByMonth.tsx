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
    const compareMonth = (index:number):boolean =>{
        const today:Date = new Date();
        const dateBeingChecked:Date = new Date(date.year,index,today.getDate());
        return dateBeingChecked >=parsedHabitCreatedDate && dateBeingChecked<=today;
    }

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
                                `border-2 border-black rounded-sm mb-5 relative h-3 w-3 sm:h-5 sm:w-5 shadow-md shadow-black ${bgClass}`  
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
                        <img src="./BasicArrow.png" className="rotate-180 h-4 w-4 md:w-7 md:h-7 cursor-pointer" onClick={()=>setMonthlyHabits(undefined)}/>
                        <p>{DateData.months[date.month]}</p>
                        <p>{date.year}</p>
                    </div>
                    <div className="grid grid-cols-7 justify-items-center w-[80%] mx-auto mt-[25%]">
                        {DateData.days.map((day,i)=>
                            <p className={"text-lg md:text-2xl "+(i < firtDayOfMonth && "row-span-2")} 
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
                    <div className="flex justify-between text-md md:text-2xl w-[80%] mx-auto relative mt-4">
                        <img 
                            src="./BasicArrow.png" 
                            className="rotate-180 h-4 lg:h-7 w-4 lg:w-7 cursor-pointer" 
                            onClick={()=>setDate((prevDate)=>({...prevDate,year:date.year-1}))}
                        />
                        {date.year}
                        <img 
                            src="./BasicArrow.png" 
                            className="h-4 lg:h-7 w-4 lg:w-7 cursor-pointer" 
                            onClick={()=>setDate((prevDate)=>({...prevDate,year:date.year+1}))}
                        />
                    </div>
                    {/*All the different months */}
                    <div className="grid grid-cols-3 gap-y-3 w-[80%] mx-auto text-center mt-10">
                        {DateData.months.map((month,index)=>
                            <div 
                                key={month} 
                                className="shadow-md mx-auto md:border-2 border-dashed rounded-md h-10 md:h-30 w-8 md:w-30 my-autogrid items-center cursor-pointer dropShadow"
                                onClick={()=>handleMonthSelection(index)}
                            >
                                <p className="text-md md:text-4xl md:mt-5">{smallScreen ? month.substring(0,3) : month}</p>
                                <p className="text-xs md:text-2xl">
                                    {compareMonth(index) &&
                                        //determines what follows the number, the unit of the habit
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