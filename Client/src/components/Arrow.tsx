
//Basic text based arrows used in larger parts of the application
const Arrow = (props:{onClick: ()=>void, inverse?: boolean, className?: string, show?:boolean})=>{
    const {onClick, inverse, className,show} = props;
    
    let styling = "absolute text-4xl left-4 top-55 cursor-pointer sm:text-6xl sm:left-15 md:text-9xl md:left-35 md:top-50";
    if(inverse)
        styling = "absolute text-4xl right-4 top-55 cursor-pointer sm:text-6xl sm:right-15 md:text-9xl md:right-35 md:top-50";
    
    return show==undefined || show ? 
        <p className={styling+className} onClick={onClick}>{inverse ? ">": "<"}</p>
        :
        <></>;
    
}

export default Arrow;