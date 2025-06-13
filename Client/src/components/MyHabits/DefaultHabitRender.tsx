import Habit from "../../types/Habit";
import RenderHabitUtils from "./RenderHabitUtils";

//This is the default habit displayed in the myhabits page
const DefaultHabitRender = (props:{habit:Habit, topLeftButton:React.ReactNode, topRightButton:React.ReactNode})=>{
    const {habit,topLeftButton,topRightButton} = props;
    return(
        <div className="drop-shadow-xl p-3 grid gap-y-4 habitBorder w-40 md:w-65 lg:w-80 mx-auto">
            <div className="flex justify-between">
                {topLeftButton}
                <p className="text-lg lg:text-2xl">{RenderHabitUtils.getDaysActiveTitle(habit)}</p>
                {topRightButton}
            </div>
            <p className="text-4xl text-center mx-auto">{habit.name}</p>
            <div className="flex justify-center md:justify-between">
                <p className="text-lg lg:text-2xl">Date created: {habit.dateCreated}</p>
                <p className="text-lg lg:text-2xl hidden md:block">Type: {RenderHabitUtils.typeConverstion[habit.type]}</p>
            </div>
        </div>
    );
}

export default DefaultHabitRender;