//All basic imge buttons such as the add icon in create habit for active habits in myhabits page user this
const ImageButton = (props:{image?:React.ReactNode, onClick?: ()=>void, className?: string})=>{
    const {image, onClick,className} = props;
    return(
        <button className={"border-3 border-black rounded-4xl cursor-pointer w-[3rem] h-[3rem] dropShadow "+className}
        onClick={onClick}>{image}</button>
    );
}

export default ImageButton;