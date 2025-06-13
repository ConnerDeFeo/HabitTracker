import { useNavigate } from "react-router-dom";

const NavigationItem = (props:{name:string, navigateTo:string, imagePath?:string, alt?:string, onClick?:()=>void, className?:string})=>{
    const navigate = useNavigate();
    const {name,imagePath,alt,navigateTo, onClick, className} = props;

    const handleClick = ()=>{
        navigate(navigateTo);
        onClick && onClick();
    }

    return (
        <div className={"cursor-pointer mt-auto flex gap-2 crossOut w-fit "+(className || "")} onClick={handleClick}>
            <p className="text-3xl md:text-4xl">{name}</p>
            {imagePath && alt && <img src={imagePath} alt={alt} className="w-8 h-8 my-auto"/>}
        </div>
    );
}

export default NavigationItem;

