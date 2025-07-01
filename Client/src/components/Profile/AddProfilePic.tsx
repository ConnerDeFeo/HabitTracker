import { RefObject, useState } from "react";
import { convertToPixelCrop, Crop } from "react-image-crop";
import ImageCropper from "./ImageCropper";
import Button from "../General/Button";
import Modal from "../General/Modal";
import SetCrop from "./SetCrop";

//When use gos to upload a new pfp
const AddProfilePic = (props:{imgSrc:string, imgRef:RefObject<HTMLImageElement | null> ,canvasRef:RefObject<HTMLCanvasElement | null>, onClose: ()=>void}) =>{
    const {imgSrc, imgRef, canvasRef, onClose} = props;
    const [crop, setCrop] = useState<Crop>({
        unit: '%', // Can be 'px' or '%'
        x: 200,
        y: 100,
        width: 25,
        height: 25,
    })
    return(
        <Modal content={
            <>
                <ImageCropper imageSrc={imgSrc} crop={crop} setCrop={setCrop}/>
                <Button label="Confirm" className="w-30" onClick={
                    ()=>{
                        if(imgRef.current && canvasRef.current)
                            SetCrop(
                                imgRef.current,
                                canvasRef.current,
                                convertToPixelCrop(crop,imgRef.current.width,imgRef.current.height)
                            )
                        console.log(canvasRef);
                    }}/>
            </>
        } onClose={onClose}/>
    );
}

export default AddProfilePic;