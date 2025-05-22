const ImageButton = (props:{image?:React.ReactNode, onClick?: ()=>void, className?: string})=>{
    const {image, onClick,className} = props;
    return(
        <button className={"border-3 border-black text-white rounded-4xl cursor-pointer w-[3rem] h-[3rem] "+className}
        onClick={onClick}>{image}</button>
    );
}

export default ImageButton;