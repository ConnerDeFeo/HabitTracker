import { ChangeEvent, SyntheticEvent, useEffect, useRef, useState } from "react";
import ReactCrop, { centerCrop, convertToPixelCrop, Crop, makeAspectCrop } from "react-image-crop";
import Button from "../General/Button";
import Modal from "../General/Modal";
import ConvertToCanvas from "./ConvertToCanvas";
import heic2any from "heic2any";
import UploadProfilePicture from "./UploadProfilePicture";

//When use gos to upload a new pfp
const AddProfilePic = (
    props:{ 
        onClose: ()=>void,
        setImageUrl: React.Dispatch<React.SetStateAction<string>>
    }
) =>{
    const {onClose, setImageUrl} = props;
    const canvas: HTMLCanvasElement = document.createElement("canvas");
    canvas.width=150;
    canvas.height=150;
    const ASPECT = 1;
    const MINWIDTH = 150;

    const imgRef = useRef<HTMLImageElement | null>(null);
    const fileInputRef = useRef<HTMLInputElement | null>(null);

    const [imgSrc,setImgSrc] = useState<string>("");
    const [crop, setCrop] = useState<Crop>({
        unit: '%', // Can be 'px' or '%'
        x: 200,
        y: 100,
        width: 25,
        height: 25,
    });

    //Once file is selected, set the current uplaodedimg to that file
    const handleFileChange = async (e: ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if(file){

            let fileToUse = file;

            // Convert HEIC to JPEG because HEIC sucks why does apple use it god damn
            if (file.type === "image/heic" || file.name.endsWith(".heic")) {
                const converted = await heic2any({
                    blob: file,
                    toType: "image/jpeg",
                    quality: 0.9,
                });
                fileToUse = new File([converted as BlobPart], "converted.jpg", { type: "image/jpeg" });
            }

            const reader = new FileReader();
            reader.addEventListener('load',()=>{
                const uploadedFile = reader.result?.toString() || "";
                setImgSrc(uploadedFile);
            });
            reader.readAsDataURL(fileToUse);
        }
    };

    //Once image is uploaded, set image crop to default to center of picture
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

    //On completion of the image being uploaded and cropped, send to backend for url
    const handleFileUpload = async () => {
        UploadProfilePicture(canvas,setImageUrl,onClose)
    };

    return(
        <Modal content={
            <div className="grid gap-y-5">
                <img src="/x.webp" alt="exit modal" className="cursor-pointer h-15 w-15" onClick={onClose}/>
                <label className="color-black border border-black font-hand text-4xl bg-black text-white rounded-2xl cursor-pointer p-2 w-fit">
                    Choose File
                    <input
                        type="file"
                        accept="image/*"
                        ref={fileInputRef}
                        onChange={handleFileChange}
                        className="hidden"
                    />
                </label>
                {imgSrc && 
                    <ReactCrop
                        crop={crop}
                        keepSelection
                        circularCrop
                        aspect={ASPECT}
                        onChange={(_,percent)=>setCrop(percent)}
                        minWidth={MINWIDTH}
                        className="max-h-100 mx-auto"
                    >
                         <img 
                            src={imgSrc} 
                            alt={"Upload"}
                            onLoad={e=>onImageLoad(e)}
                            ref={imgRef}
                        />
                    </ReactCrop>

                }
                {imgRef.current && 
                    <Button label="Confirm" className="w-30" onClick={
                    ()=>{
                        if(imgRef.current)
                            ConvertToCanvas(
                                imgRef.current,
                                canvas,
                                convertToPixelCrop(crop,imgRef.current.width,imgRef.current.height)
                            )
                        handleFileUpload();
                    }}/>
                }
            </div>
        } onClose={onClose}/>
    );
}

export default AddProfilePic;