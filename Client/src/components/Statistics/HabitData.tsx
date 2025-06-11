import HistoricalData from "../../types/HistoricalData";

//Habit data shown regarding streaks and days completed in the statistics page
const HabitData = (props:{historicalData?: HistoricalData})=>{
    const {historicalData} = props;
    return (
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
    );
}

export default HabitData;