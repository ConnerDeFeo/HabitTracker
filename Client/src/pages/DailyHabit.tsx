import Habit from "../types/Habit";

const DailyHabit = (props: {habit: Habit, inEditMode: boolean})=>{
    const {habit, inEditMode} = props;

    let habitType;
    switch(habit.type){
        case 2:
            habitType = "Time: "+habit.value+" minutes";
            break;
        case 3:
            habitType = "Numeric: "+habit.value+" "+habit.valueUnitType;
            break;
        default:
            habitType="Boolean"
            break;
    }

    return inEditMode ? 
        <div className="text-5xl text-left">
            <p >Name: {habit.name}</p>
            <p >{habitType}</p>
        </div>:
        <div className="w-80 break-words mx-auto" key={habit.id}>
            <p className="text-5xl">{habit.name}</p> 
        </div>;
}

export default DailyHabit;