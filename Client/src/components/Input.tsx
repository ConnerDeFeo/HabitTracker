import { useState } from "react";

const Input = (props: { title: string; value: string; updateValue: (character: string) => void; type?:string })=>{
    const { title, value, updateValue, type } = props;
    
    //only used if the type is password
    const [inputType, setInputType] = useState<string>(type || "");
    const [passwordVisible, setPasswordVisible] = useState<boolean>(false);
    
    //Toggles the eye between crossed out and not
    const handleEyeToggle = ()=>{
        setPasswordVisible(!passwordVisible);
        setInputType(inputType=="" ? "password" : "")
    }

    return(
        <>
            <label htmlFor={title} className="font-hand text-4xl">{title}</label>
            <div className="relative">
                <input 
                    type={inputType} 
                    id={title} 
                    name={title} 
                    className="resize-none habitBorder text-xl h-12 pl-3 align-center w-full" 
                    value={value} 
                    onChange={(e)=>updateValue(e.target.value)}
                />
                {type=="password" &&
                    <img src="./Eye.png" 
                        className="h-10 w-8 absolute right-3 top-1 cursor-pointer"
                        alt="Toggle visibility"
                        onClick={handleEyeToggle}    
                    />
                }
                {!passwordVisible && type=="password" && 
                    <div className="absolute right-[1.6rem] top-[0.4rem] w-[0.2rem] h-9 bg-black rotate-45"/>
                }
            </div>
        </>
    );
} 

export default Input;