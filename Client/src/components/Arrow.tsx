
//Basic text based arrows used in larger parts of the application
const Arrow = (props:{onClick: ()=>void, inverse?: boolean, className?: string, show?:boolean, notAbsolute?: boolean})=>{
    const {onClick, inverse, className,show, notAbsolute} = props;
    let styling = "cursor-pointer h-10 md:h-15";
    if(inverse)
        styling+=" rotate-180";
    //default is absolute, if notAbsolute is set to true, then it will not be absolute
    if(!notAbsolute)
        styling+=" absolute";

    return show==undefined || show ? 
        <img src="./NextPage.jpg" className={`${styling} ${className}`} onClick={onClick} alt={inverse ? "Previous" : "Next"} />
        :
        <></>;
    
}

export default Arrow;