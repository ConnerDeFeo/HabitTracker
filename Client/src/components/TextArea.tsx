const TextArea = (props: { title: string; value: string; updateValue: (character: string) => void })=>{
    const { title, value, updateValue } = props;
    return(
        <>
            <label htmlFor={title} className="font-hand text-4xl">{title}</label>
            <textarea id={title} name={title} className="resize-none border-2 shadow-xl rounded-2xl text-xl h-12 pt-2 pl-3 align-center" value={value} onChange={(e)=>updateValue(e.target.value)}/>
        </>
    );
} 

export default TextArea;