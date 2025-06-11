import Habit from "../../types/Habit";

//Panel showing all habits in the statistics
const AllHabits = (
    props:{
        activeHabits:Habit[], 
        nonActiveHabits:Habit[], 
        handleHabitSelection:(habitId:string)=>void,
        selectedHabitId:string
    }
)=>{
    const {activeHabits, nonActiveHabits,handleHabitSelection,selectedHabitId} = props;
    const habitNameStyling = "my-7 cursor-pointer "; //Styling for each of the habit names
    return(
        <div className="habitBorder w-75 h-110 overflow-y-auto text-4xl text-center">
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