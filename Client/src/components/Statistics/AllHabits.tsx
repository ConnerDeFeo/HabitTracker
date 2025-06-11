import Habit from "../../types/Habit";

const AllHabits = (props:{activeHabits:Habit[], nonActiveHabits:Habit[], handleHabitSelection:(habitId:string)=>void})=>{
    const {activeHabits, nonActiveHabits,handleHabitSelection} = props;
    const habitNameStyling = "text-4xl text-center my-7 cursor-pointer"
    return(
        <div className="habitBorder w-75 h-110 overflow-y-auto">
                {activeHabits.map((habit)=>
                    <p 
                        key={habit.name} 
                        className={habitNameStyling}
                        onClick={()=>handleHabitSelection(habit.id!)}
                    >{habit.name}</p>
                )}
                {nonActiveHabits.map((habit)=>
                    <p 
                        key={habit.name} 
                        className={habitNameStyling}
                        onClick={()=>handleHabitSelection(habit.id!)}
                    >{habit.name}</p>
                )}
            </div>
    );
}

export default AllHabits;