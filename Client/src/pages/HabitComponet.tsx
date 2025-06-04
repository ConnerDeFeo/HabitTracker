import Habit from "../types/Habit";
import HabitService from "../services/HabitService";
import DateInfo from "../types/DateInfo";
import DateService from "../services/DateService";

const HabitComponet = (props: {habit: Habit,  setHabits: React.Dispatch<React.SetStateAction<Habit[]>>, date:DateInfo})=>{
    const {habit, setHabits, date} = props;

    const fontStyling = "text-5xl pb-2 col-span-2";

    //when a habit is completed
    const handleHabitCompletionChange = async()=>{
        const habitCompleted = !habit.completed;
        const resp = await HabitService.completeHabit(habit.id!, `${date.year}-${DateService.padZero(date.month+1)}-${DateService.padZero(date.day)}`, habitCompleted);

        if(resp.status==200){
            habit.completed = habitCompleted;
            setHabits((prevHabits) =>
                prevHabits.map((h) =>
                    h.id === habit.id ? habit : h
                )
            );
        }
    }

    return(
        <div className={"w-80 break-words mx-auto cursor-pointer "} key={habit.id} onClick={handleHabitCompletionChange}>
            <p className={fontStyling + (habit.completed && " line-through")}>{habit.name}</p>
        </div>
    );
}

export default HabitComponet;