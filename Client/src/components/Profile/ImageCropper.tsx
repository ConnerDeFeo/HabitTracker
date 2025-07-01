import { SyntheticEvent } from "react";
import ReactCrop, { centerCrop, Crop, makeAspectCrop } from "react-image-crop";


const ImageCropper = (
    props:{
        imageSrc:string, 
        className?:string, 
        crop:Crop,
        setCrop:React.Dispatch<React.SetStateAction<Crop>>
    }
)=>{
    const ASPECT = 1;
    const MINWIDTH = 150;
    const {imageSrc,className,crop,setCrop} = props;

    //Sets crop to middle of screen and then
    const onImageLoad = (e:SyntheticEvent<HTMLImageElement, Event>)=>{
        const {width,height} = e.currentTarget;
        const cropWidthAsPercent = (MINWIDTH / width) * 100;
        const crop = makeAspectCrop(
            {
                unit:'%',
                width:cropWidthAsPercent
            },
            ASPECT,
            width,
            height
        )   
        const centeredCrop = centerCrop(crop,width,height);
        setCrop(centeredCrop);
    }

    return(
        <ReactCrop
            crop={crop} //Crop doesnt go away
            keepSelection
            circularCrop
            aspect={ASPECT}
            onChange={(_,percent)=>setCrop(percent)} // use percent crop for scalling
            className={className}
            minWidth={MINWIDTH}
        >
            <img 
                src={imageSrc} 
                alt={"Upload"}
                onLoad={e=>onImageLoad(e)}
            />
        </ReactCrop>
    );
}

export default ImageCropper;