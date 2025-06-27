import { useState } from "react";
import Habit from "../../types/Habit";
import DateData from "../../data/DateData";
import HabitConverstions from "../../data/HabitConversions";

/**
 * _____________________________________________________
 * IMPORTANT READ BEFORE LOOKING AT THE REST OF THE CODE
 * 
 * Types are held as numeric values on the front end for deserialization purposes:
 * 
 * 1: Boolean
 * 2: Time
 * 3: Numeric
 * 
 * This component is somewhat generic and can deal with the creation adn editation of habits.
 * 
 * Used for creation and editaion of habits in the myhabits page
 */
const CreateHabit = (props: {
    handleCancelation:()=>void,
    handleHabitCompletion: (habit: Habit)=>Promise<void>,
    initialHabit?: Habit,
})=>{
    const {handleCancelation, handleHabitCompletion, initialHabit,} = props;
    const labelStyling = "font-hand text-2xl md:text-4xl text-left";
    const inputStyling = "resize-none habitBorder text-xl sm:text-xl h-9 md:h-12 w-[100%] sm:w-[80%] pl-3";
    const inputLayoutStyling = "grid sm:flex sm:justify-between";

    const defaultHabit: Habit = initialHabit??{
        name: "",
        //Type is set default to "Binary"
        type: 1,
        completed:false,
        skipped:false,
        daysActive:[],
        valueUnitType:"",
        value: 1
    }
    const [habit, setHabit] = useState<Habit>(defaultHabit);
    const buttonStyling = "border-2 border-black w-12 h-12 rounded-xl cursor-pointer mx-auto dropShadow";

    //Handles value changes for the numeric and time habits.    
    const handleValueChange = (number: string)=>{
        const num = Number.parseInt(number);
        
        if(num<0 || Number.isNaN(num)){
            setHabit((prevHabit)=>({
                    ...prevHabit,
                    value:0
            }));
        }
        //If habit type is "Time"
        else if(habit.type==2){
            if(num<=600){
                setHabit((prevHabit)=>({
                    ...prevHabit,
                    value:num
                }));
            }
        }
        //If habit type is "Numeric"
        else if(num<=99999){
            setHabit((prevHabit)=>({
                ...prevHabit,
                value:num
            }));
        }
        
    }

     //Changes the habit type and in effect the above function renderValueOutput() is effected by this
    const handleTypeChange = (event:string)=>{
        const num = Number.parseInt(event);
        const valueUnitType = num==2 ? "minutes" : "";
        setHabit((prevHabit)=>({
            ...prevHabit,
            type: num,
            value: 1,
            valueUnitType:valueUnitType
        }))
    }

    //On click for the days shown during habit creation
    const handleDateSelection = (day:string)=>{
        const daysActive:string[] = habit.daysActive;
        if(daysActive.includes(day))
            setHabit((prevHabit)=>(
                {
                    ...prevHabit,
                    daysActive: daysActive.filter(h=>h!==day)
                }
            ))
        
        else
            setHabit((prevHabit)=>(
                {
                    ...prevHabit,
                    daysActive: [...daysActive,day]
                }
            ))
    }

    /*Renders the different creation components based on the type of habit being created.
        Directly tied to the current habit.type*/
    const renderValueOutput = ()=>{
        //If habit type is binary
        const valueInput = habit.type!==1 &&
            <input 
                id="value"
                name="value" 
                className={inputStyling} 
                value={habit.value == 0 ? "" : habit.value}
                onChange={(e) => handleValueChange(e.target.value)}
            />;

        //If habit type is numeric
        const unitInput = habit.type === 3 ? 
            <input 
                id="unit"
                name="unit" 
                className={inputStyling}
                value={habit.valueUnitType}
                //Prevent long unit types
                onChange={(e) => setHabit((prevHabit)=>({...prevHabit, valueUnitType:(e.target.value.length<=10 ? e.target.value : prevHabit.valueUnitType)}))}
            />
            :
            <p className="text-4xl mx-auto">{habit.type===2 && "Minutes"}</p>;

        return (
            <>
                <div className={inputLayoutStyling}>
                    <label htmlFor="value" className={labelStyling}>Value: </label>
                    {valueInput}
                </div>
                <div className={inputLayoutStyling}>
                    <label htmlFor="unitType" className={labelStyling}>Unit: </label>
                    {unitInput}
                </div>
            </>
        );
    }

    return(
        <div className="grid p-2 gap-y-2">
            {/*Name and input */}
            <div className={inputLayoutStyling}>
                <label htmlFor="name" className={labelStyling}>Name: </label>
                <input 
                    id="name"
                    name="name" 
                    className={inputStyling} 
                    value={habit.name}
                    onChange={(e) => {
                        const newName:string = e.target.value;
                        if(newName.length<=25){
                            setHabit((prevHabit: Habit) => ({
                                ...prevHabit,
                                name: e.target.value,
                            }));
                        }
                    }}
                />
            </div>
            {/*Type and input*/}
            <div className={inputLayoutStyling}>
                <label htmlFor="type" className={labelStyling}>Type: </label>
                {
                    //if habit initially habit was passed through props
                    initialHabit ? 
                        <p className="text-4xl mx-auto">{HabitConverstions.typeConverstion[habit.type]}</p>
                        :
                        <select 
                            id="type" 
                            name="type" 
                            className={inputStyling} 
                            onChange={(e)=>handleTypeChange(e.target.value)}
                        >
                            <option value={1}>Binary</option>
                            <option value={2}>Time</option>
                            <option value={3}>Numeric</option>
                        </select>
                }
            </div>
            {/*Value and value unit type inputs*/}
            {renderValueOutput()}
            <div className="grid sm:flex">
                <label htmlFor="days" className={labelStyling}>Days: </label>
                <div id="days" className="flex text-3xl md:text-4xl justify-between m-auto w-[100%] sm:w-[80%]">
                    {DateData.days.map((day)=>
                        <p 
                        key={day} 
                        onClick={()=>handleDateSelection(day)} 
                        className={`cursor-pointer ${!habit.daysActive.includes(day) && "text-gray-500"}`}>
                            {day.substring(0,3)}
                        </p>
                    )}
                </div>
            </div>
            <div className="grid grid-cols-2 mt-5">
                <button className={buttonStyling}>
                    <img src="./x.webp" alt="x" className="w-10 h-10 mx-auto" onClick={handleCancelation}/>
                </button>
                <button className={buttonStyling} onClick={
                    async () => { await handleHabitCompletion(habit);}
                }>
                    <img src="./checkMark.webp" alt="check mark" className="w-8 h-8 mx-auto"/>
                </button>
            </div>
        </div>
    );
}

export default CreateHabit;