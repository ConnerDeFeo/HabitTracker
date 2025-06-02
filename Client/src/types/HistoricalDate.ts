import Habit from "./Habit";

type HistoricalDate = {
    habits: Record<string,Habit>,
    allHabitsCompleted: boolean
}

export default HistoricalDate;