import DateData from "../../data/DateData";
import DateService from "../../services/DateService";

//month thats displayed for user profile and friends profile
const RenderCurrentMonth = (props:{currentMonthHabitsCompleted:Record<string,boolean>})=>{
    const {currentMonthHabitsCompleted}= props;
    const now = new Date();
    const daysInCurrentMonth: number = new Date(now.getFullYear(), now.getMonth()+1 , 0).getDate();
    const firtDayOfMonth = new Date(now.getFullYear(), now.getMonth(), 1).getDay();
    return(
            <div>
                <h2 className="text-3xl md:text-5xl md:mt-5 font-hand text-center">{DateData.months[new Date().getMonth()]}</h2>
                <div className="grid grid-cols-7 justify-items-center mx-auto mt-2">
                    {DateData.days.map((day,i)=>
                        <p className={"text-2xl md:text-4xl "+(i < firtDayOfMonth && "row-span-2")} 
                            key={day}
                        >
                            {day.substring(0,1)}
                        </p>
                    )}
                    {Array.from({ length: daysInCurrentMonth }, (_, i) => i + 1).map((day) =>{
                        const habitsCompleted = currentMonthHabitsCompleted?.[DateService.padZero(day)];
                        let bgClass = "";

                        if (habitsCompleted !==undefined) 
                            bgClass = habitsCompleted? "bg-green-500" : "bg-red-500";
                        else
                            bgClass = "bg-gray-500";
                        return (
                            <div
                                key={day}
                                className={
                                    `border-2 border-black rounded-sm mb-5 relative h-5 w-5 md:h-8 md:w-8 lg:w-10 lg:h-10 shadow-md shadow-black ${bgClass}`  
                                }
                            ></div>
                        );
                    })}
                </div>
            </div>
        );
}

export default RenderCurrentMonth;