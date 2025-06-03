const Button = (props:{label?:string, onClick?: ()=>void, className?:string})=>{
    const {label, onClick, className} = props;
    return(
        <button className={"color-black border border-black ml-auto font-hand text-4xl bg-black text-white rounded-2xl cursor-pointer p-2 "+className}
        onClick={onClick}>{label}</button>
    );
}

export default Button;