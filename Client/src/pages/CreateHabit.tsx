import { useState } from "react";
import Habit from "../types/Habit";

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
 * @param props 
 * @returns 
 */
const CreateHabit = (props: {
    handleCancelation:()=>void,
    handleHabitCompletion: (habit: Habit)=>Promise<void>,
    initialHabit?: Habit,
})=>{
    const {handleCancelation, handleHabitCompletion, initialHabit,} = props;

    const defaultHabit: Habit = initialHabit??{
        name: "",
        //Type is set default to "Boolean"
        type: 0,
        completed:false
    }
    const [habit, setHabit] = useState<Habit>(defaultHabit);

    const valueInputStyling = "text-2xl text-center border-2 shadow-xl rounded-2xl h-9";
    const valueUnitStyling = "font-hand border-2 shadow-xl rounded-2xl h-9 text-center";
    const buttonStyling = "border-2 border-black w-12 h-12 rounded-xl cursor-pointer mx-auto";

    
    const handleValueChange = (number: string)=>{
        const num = Number.parseInt(number);
        console.log(num);

        if(num<0)
            return;
        //If habit type is "Time"
        if(habit.type==2){
            if(num<=600){
                setHabit((prevHabit)=>({
                    ...prevHabit,
                    value:num
                }));
            }
        }
        else if(num<=99999){
            setHabit((prevHabit)=>({
                ...prevHabit,
                value:num
            }));
        }
        
    }

    const renderValueOutput = ()=>{
        switch(habit.type){
            case 2:
                return(
                    <div className="grid grid-cols-2 grid-rows-2 gap-x-1">
                        <label htmlFor="timeValue" className="font-hand text-4xl text-left col-span-2">Value: </label>
                        <input 
                            id="timeValue" 
                            type="number" 
                            name="type" 
                            className={valueInputStyling} 
                            value={habit.value}
                            onChange={(e)=>handleValueChange(e.target.value)}
                        />
                        <span className={valueUnitStyling+" text-4xl"}>Minutes</span>
                    </div>
                );
            case 3:
                return(
                    <div className="grid grid-cols-2 grid-rows-2 gap-x-1">
                        <label htmlFor="numericValue" className="font-hand text-4xl text-left col-span-2">Value: </label>
                        <input 
                            id="numericValue" 
                            type="number" 
                            name="type" 
                            title=""
                            className={valueInputStyling}
                            onChange={(e)=>handleValueChange(e.target.value)}
                            value={habit.value}
                        />
                        <input 
                            id="valueUnitType" 
                            className={valueUnitStyling+" text-3xl"}
                            value={habit.valueUnitType}
                            onChange={(e)=>setHabit((prevHabit)=>{
                                    const value = e.target.value;
                                    if(value.length<15){
                                        return {
                                            ...prevHabit,
                                            valueUnitType: value
                                        }
                                    }
                                    return prevHabit
                                }
                            )}
                        />
                    </div>
                ); 
            default:
                return <></>;       
        }
    }

    const handleTypeChange = (event:string)=>{
        const num = Number.parseInt(event);
        const valueUnitType = num==2 ? "minutes" : "";
        setHabit((prevHabit)=>({
            ...prevHabit,
            type: num,
            value: 0,
            valueUnitType:valueUnitType
        }))
    }

    return(
        <div className="grid">
            <label htmlFor="name" className="font-hand text-4xl text-left">{"Habit Name: "}</label>
            <input 
                id="name"
                name="name" 
                className="resize-none border-2 shadow-xl rounded-2xl text-xl h-8 pl-3 " 
                value={habit.name}
                onChange={(e) => {
                    setHabit((prevHabit: Habit) => ({
                    ...prevHabit,
                    name: e.target.value,
                    }));
                }}
            />
            <div className="grid">
                <label htmlFor="type" className="font-hand text-4xl text-left">Type: </label>
                <select 
                    id="type" 
                    name="type" 
                    className="border-2 shadow-xl rounded-2xl text-xl h-8 pl-3" 
                    onChange={(e)=>handleTypeChange(e.target.value)}
                >
                    <option value={1}>Binary</option>
                    <option value={2}>Time</option>
                    <option value={3}>Numeric</option>
                </select>
                {renderValueOutput()}
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