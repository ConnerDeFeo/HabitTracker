import ReactCrop, { Crop } from "react-image-crop";


const ImageCropper = (
    props:{
        imageSrc:string, 
        className?:string, 
        crop:Crop,
        setCrop:React.Dispatch<React.SetStateAction<Crop>>
    }
)=>{

    const {imageSrc,className,crop,setCrop} = props;

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