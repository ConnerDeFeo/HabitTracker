import Habit from "./Habit";

type HistoricalData =  {
    habit: Habit,
    daysCompleted: number,
    totalValueCompleted: number,
    longestStreak: number,
    currentStreak: number
}

export default HistoricalData;
