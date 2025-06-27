import Habit from "../../types/Habit";

//Panel showing all habits in the statistics
const AllHabits = (
    props:{
        activeHabits:Habit[], 
        nonActiveHabits:Habit[], 
        handleHabitSelection:(habitId:string)=>void,
        selectedHabitId:string,
        smallScreen:boolean
    }
)=>{
    const {activeHabits, nonActiveHabits,handleHabitSelection,selectedHabitId,smallScreen} = props;
    const habitNameStyling = "my-7 cursor-pointer"; //Styling for each of the habit names
    return smallScreen ? 
        <>
            <select className="habitBorder mb-10 w-60 text-center text-xl pl-2 mx-auto"
                onChange={(e)=>handleHabitSelection(e.target.value)}
            >
                {activeHabits.map(habit=>
                    <option
                        key={habit.id}
                        value={habit.id}
                    >{habit.name}</option>
                )}
                {nonActiveHabits.map(habit=>
                    <option
                        key={habit.id}
                        value={habit.id}
                    >{habit.name}</option>
                )}
            </select>
        </>
        :
        <div className="habitBorder w-75 overflow-y-auto text-4xl text-center h-100">
            {activeHabits.map(habit=>
                <p 
                    key={habit.name} 
                    className={`${habitNameStyling} ${(selectedHabitId===habit.id! && "line-through")}`}
                    onClick={()=>handleHabitSelection(habit.id!)}
                >{habit.name}</p>
            )}
            {nonActiveHabits.map(habit=>
                <p 
                    key={habit.name} 
                    className={`${habitNameStyling} ${(selectedHabitId===habit.id! && "line-through")}`}
                    onClick={()=>handleHabitSelection(habit.id!)}
                >{habit.name}</p>
            )}
        </div>
    ;
}

export default AllHabits;