import { useState } from "react";
import Habit from "../types/Habit";
import HabitService from "../service/HabitService";

const CreateHabit = (props: {setHabits: React.Dispatch<React.SetStateAction<Habit[]>>, handleCancelation:()=>void })=>{
    const {setHabits, handleCancelation} = props;

    const defaultHabit: Habit = {
        name: "",
        type: "boolean",
        completed:false,
        value:0,
        valueUnitType:""
    }
    const [habit, setHabit] = useState<Habit>(defaultHabit);

    const valueInputStyling = "text-2xl text-center border-2 shadow-xl rounded-2xl h-9";
    const valueUnitStyling = "font-hand border-2 shadow-xl rounded-2xl h-9 text-center";
    const buttonStyling = "border-2 border-black w-12 h-12 rounded-xl cursor-pointer mx-auto";

    
    const handleValueChange = (number: number)=>{
        if(number<0)
            return;
        if(habit.type=="time"){
            if(number<=600){
                setHabit((prevHabit)=>({
                    ...prevHabit,
                    value:number
                }));
            }
        }
        else if(number<=99999){
            setHabit((prevHabit)=>({
                ...prevHabit,
                value:number
            }));
        }
        
    }

    const renderValueOutput = ()=>{
        switch(habit.type){
            case "time":
                return(
                    <div className="grid grid-cols-2 grid-rows-2 gap-x-1">
                        <label htmlFor="timeValue" className="font-hand text-4xl text-left col-span-2">Value: </label>
                        <input 
                            id="timeValue" 
                            type="number" 
                            name="type" 
                            className={valueInputStyling} 
                            onChange={(e)=>handleValueChange(Number(e.target.value))}
                            value={habit.value}
                        />
                        <span className={valueUnitStyling+" text-4xl"}>Minutes</span>
                    </div>
                );
            case "numeric":
                return(
                    <div className="grid grid-cols-2 grid-rows-2 gap-x-1">
                        <label htmlFor="numericValue" className="font-hand text-4xl text-left col-span-2">Value: </label>
                        <input 
                            id="numericValue" 
                            type="number" 
                            name="type" 
                            title=""
                            className={valueInputStyling}
                            onChange={(e)=>handleValueChange(Number(e.target.value))}
                            value={habit.value}
                        />
                        <input 
                            id="valueUnitType" 
                            className={valueUnitStyling+" text-3xl"}
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
                            value={habit.valueUnitType}
                        />
                    </div>
                ); 
            default:
                return <></>;       
        }
    }

    const handleTypeChange = (event:string)=>{
        const valueUnitType = event=="time" ? "minutes" : "";
        setHabit((prevHabit)=>({
            ...prevHabit,
            type: event,
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
                    <option value="boolean">Boolean</option>
                    <option value="time">Time</option>
                    <option value="numeric">Numeric</option>
                </select>
                {renderValueOutput()}
            </div>
            <div className="grid grid-cols-2 mt-5">
                <button className={buttonStyling}>
                    <img src="./x.webp" alt="x" className="w-10 h-10 mx-auto" onClick={handleCancelation}/>
                </button>
                <button className={buttonStyling} onClick={
                    async () => {
                    }
                }>
                    <img src="./checkMark.webp" alt="check mark" className="w-8 h-8 mx-auto"/>
                </button>
            </div>
        </div>
    );
}

export default CreateHabit;