const Input = (props: { title: string; value: string; updateValue: (character: string) => void; type?:string })=>{
    const { title, value, updateValue, type } = props;
    return(
        <>
            <label htmlFor={title} className="font-hand text-4xl">{title}</label>
            <input type={type} id={title} name={title} className="resize-none habitBorder text-xl h-12 pl-3 align-center" value={value} onChange={(e)=>updateValue(e.target.value)}/>
        </>
    );
} 

export default Input;