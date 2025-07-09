import ProfileHabit from "../../types/ProfileHabit";

const CurrentHabits = (props:{currentHabits:ProfileHabit[]})=>{
    const {currentHabits} = props;
    return(
        <div className="row-span-2">
            <div className="grid w-75 mx-auto mb-10">
                <h1 className="text-6xl text-center border-b-3 mb-5">Current Habits</h1>
                <div className="grid gap-y-5 overflow-y-auto max-h-100">
                    {currentHabits.map( habit => (
                        <div key={habit.name} className="habitBorder p-2">
                            <p className="text-4xl text-center">{habit.name}</p>
                            <p className="text-2xl ml-5">Created: {habit.dateCreated}</p>
                            <p className="text-2xl ml-5">Current Streak: {habit.currentStreak}</p>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}

export default CurrentHabits;