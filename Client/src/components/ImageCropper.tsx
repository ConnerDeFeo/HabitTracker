import { useState } from "react";
import ReactCrop, { Crop } from "react-image-crop";

const ImageCropper = (props:{imageSrc:string, className?:string})=>{

    const {imageSrc,className} = props;
    const [crop,setCrop] = useState<Crop>();

    return(
        <ReactCrop
            crop={crop}
            circularCrop
            keepSelection
            aspect={1}
            onChange={(c)=>setCrop(c)}
            className={className}
        >
            <img 
                src={imageSrc} 
                alt={"Upload"}
            />
        </ReactCrop>
    );
}

export default ImageCropper;