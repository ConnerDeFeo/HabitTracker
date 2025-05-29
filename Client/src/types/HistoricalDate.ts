import Habit from "./Habit";

type HistoricalDate = {
    habits: Record<number,Habit>,
    allHabitsCompleted: boolean
}

export default HistoricalDate;