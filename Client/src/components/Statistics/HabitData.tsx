import HistoricalData from "../../types/HistoricalData";

//Habit data shown regarding streaks and days completed in the statistics page
const HabitData = (props:{historicalData?: HistoricalData})=>{
    const {historicalData} = props;
    const textSizing:string = "text-3xl xl:text-4xl";
    return (
        <div className="mb-5">
            {historicalData === undefined ? 
                <p className="text-6xl text-center">No habits to view!</p>
                :
                <div className="grid md:grid-cols-3 justify-between md:w-[90%] md:mx-auto border">
                    <p className={`${textSizing} md:mb-5 md:row-start-1 md:col-start-1`}>{historicalData.habit.name}</p>
                    <p className={`${textSizing} my-5 md:my-0 md:row-start-1 md:col-start-3 md:text-right`}>Longest Streak: {historicalData.longestStreak}</p>
                    <p className={`${textSizing} md:row-start-2 md:col-start-3 md:text-right`}>Current Streak: {historicalData.currentStreak}</p>
                    <p className={`${textSizing} my-5 md:my-0 md:row-start-2 md:col-start-1 md:col-span-2`}>
                        {`${historicalData.totalValueCompleted} 
                        ${historicalData.habit.valueUnitType || "days completed"} 
                        sense ${historicalData.habit.dateCreated}`}
                    </p>
                </div>
            }
        </div>
    );
}

export default HabitData;