const TextArea = (props: { title: string; value: string; updateValue: (character: string) => void })=>{
    const { title, value, updateValue } = props;
    return(
        <>
            <label htmlFor={title}>{title}</label>
            <textarea id={title} name={title} className="resize-none" value={value} onChange={(e)=>updateValue(e.target.value)}/>
        </>
    );
} 

export default TextArea;