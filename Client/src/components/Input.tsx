import { useState } from "react";

//Base input used for the application
const Input = (
    props: 
        { 
            title?: string, 
            value: string, 
            updateValue: (character: string) => void, 
            type?:string, 
            placeholder?:string,
            className?:string
        }
)=>{
    const { title, value, updateValue, type, placeholder, className } = props;
    
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
            {title && <label htmlFor={title} className="font-hand text-2xl md:text-4xl">{title}</label>}
            <div className="relative">
                <input 
                    type={inputType} 
                    id={title} 
                    name={title} 
                    className={"resize-none habitBorder w-full text-xs md:text-xl h-8 md:h-12 pl-3 align-center "+(className??"")}
                    value={value} 
                    onChange={(e)=>updateValue(e.target.value)}
                    placeholder={placeholder}
                />
                {type=="password" &&
                    <img src="./Eye.png" 
                        className="absolute h-8 md:h-10 w-6 md:w-8 right-1 md:right-3 top-0 md:top-[0.25rem] cursor-pointer"
                        alt="Toggle visibility"
                        onClick={handleEyeToggle}    
                    />
                }
                {!passwordVisible && type=="password" && 
                    <div className="absolute right-[0.94rem] top-[0.2rem] w-[0.2rem] h-7 md:right-[1.6rem] md:top-[0.4rem] md:h-9 bg-black rotate-45"/>
                }
            </div>
        </>
    );
} 

export default Input;