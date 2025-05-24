import Habit from "../types/Habit";

const DailyHabit = (props:{habit: Habit})=>{
    const {habit} = props;
    return(
        <div className="w-80 break-words mx-auto">
            <p className="text-5xl">{"Testing different word combinations "+habit.name}</p> 
        </div>
    );
}

export default DailyHabit;