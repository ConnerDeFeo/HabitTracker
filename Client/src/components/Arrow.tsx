const Arrow = (props:{onClick: ()=>void, inverse?: boolean, className?: string, show?:boolean})=>{
    const {onClick, inverse, className,show} = props;
    
    let styling = "text-9xl absolute left-35 top-50 cursor-pointer ";
    if(inverse)
        styling = "text-9xl absolute right-35 top-50 cursor-pointer ";
    
    return show==undefined || show ? 
        <p className={styling+className} onClick={onClick}>{inverse ? ">": "<"}</p>
        :
        <></>;
    
}

export default Arrow;