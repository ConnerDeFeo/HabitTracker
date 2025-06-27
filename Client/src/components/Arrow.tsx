
//Basic text based arrows used in larger parts of the application
const Arrow = (props:{onClick: ()=>void, inverse?: boolean, className?: string, show?:boolean})=>{
    const {onClick, inverse, className,show} = props;
    
    let styling = "absolute cursor-pointer h-10 sm:h-15";
    if(inverse)
        styling+=" rotate-180";

    return show==undefined || show ? 
        <img src="./NextPage.jpg" className={`${styling} ${className}`} onClick={onClick} alt={inverse ? "Previous" : "Next"} />
        :
        <></>;
    
}

export default Arrow;