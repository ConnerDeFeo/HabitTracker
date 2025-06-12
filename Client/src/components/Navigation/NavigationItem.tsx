import { useNavigate } from "react-router-dom";

const NavigationItem = (props:{name:string, imagePath:string, alt:string, navigateTo:string})=>{
    const navigate = useNavigate();
    const {name,imagePath,alt,navigateTo} = props;
    return (
        <div className="cursor-pointer mt-auto flex gap-2 crossOut" onClick={()=>navigate(navigateTo)}>
            <p className="text-4xl">{name}</p>
            <img src={imagePath} alt={alt} className="w-8 h-8 my-auto"/>
        </div>
    );
}

export default NavigationItem;

