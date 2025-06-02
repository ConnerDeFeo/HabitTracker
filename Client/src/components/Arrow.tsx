const Arrow = (props:{onClick: ()=>void, inverse?: boolean, className?: string})=>{
    const {onClick, inverse, className} = props;
    
    let styling = "text-9xl absolute left-35 top-50 cursor-pointer ";
    if(inverse)
        styling = "text-9xl absolute right-35 top-50 cursor-pointer ";
    
    return (
        <p className={styling+className} onClick={onClick}>{inverse ? ">": "<"}</p>
    );
}

export default Arrow;