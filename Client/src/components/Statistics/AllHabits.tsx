import Habit from "../../types/Habit";

const AllHabits = (
    props:{
        activeHabits:Habit[], 
        nonActiveHabits:Habit[], 
        handleHabitSelection:(habitId:string)=>void,
        selectedHabitId:string
    }
)=>{
    const {activeHabits, nonActiveHabits,handleHabitSelection,selectedHabitId} = props;
    const habitNameStyling = "text-4xl text-center my-7 cursor-pointer ";
    return(
        <div className="habitBorder w-75 h-110 overflow-y-auto">
                {activeHabits.map((habit)=>
                    <p 
                        key={habit.name} 
                        className={habitNameStyling+(selectedHabitId===habit.id! && "line-through")}
                        onClick={()=>handleHabitSelection(habit.id!)}
                    >{habit.name}</p>
                )}
                {nonActiveHabits.map((habit)=>
                    <p 
                        key={habit.name} 
                        className={habitNameStyling+(selectedHabitId===habit.id! && "line-through")}
                        onClick={()=>handleHabitSelection(habit.id!)}
                    >{habit.name}</p>
                )}
            </div>
    );
}

export default AllHabits;