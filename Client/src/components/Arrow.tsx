
//Basic text based arrows used in larger parts of the application
const Arrow = (props:{onClick: ()=>void, inverse?: boolean, className?: string, show?:boolean})=>{
    const {onClick, inverse, className,show} = props;
    
    let styling = "absolute text-4xl cursor-pointer sm:text-6xl md:text-9xl  ";
    if(inverse)
        styling = "absolute text-4xl cursor-pointer sm:text-6xl md:text-9xl  ";
    
    return show==undefined || show ? 
        <p className={styling+className} onClick={onClick}>{inverse ? ">": "<"}</p>
        :
        <></>;
    
}

export default Arrow;