//Base button for this application
const Button = (props:{label?:string, onClick?: ()=>void, className?:string})=>{
    const {label, onClick, className} = props;
    return(
        <button 
            className={`color-black border border-black text-4xl font-hand bg-black text-white rounded-2xl cursor-pointer py-1 ${className}`}
        onClick={onClick}>{label}</button>
    );
}

export default Button;