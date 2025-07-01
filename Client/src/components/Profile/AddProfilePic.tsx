import { ChangeEvent, SyntheticEvent, useRef, useState } from "react";
import ReactCrop, { centerCrop, convertToPixelCrop, Crop, makeAspectCrop } from "react-image-crop";
import Button from "../General/Button";
import Modal from "../General/Modal";
import ConvertToCanvas from "./ConvertToCanvas";
import PhotoService from "../../services/PhotoService";

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
            const reader = new FileReader();
            reader.addEventListener('load',()=>{
                const uploadedFile = reader.result?.toString() || "";
                setImgSrc(uploadedFile);
            });
            reader.readAsDataURL(file);
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
    const handleFileUpload = ()=>{
        canvas.toBlob(async (blob)=>{
            if(!blob)
                return;

            //Convert canvas element to image
            const file = new File([blob], "ProfilePicture.jpg", { type: "image/jpeg" });

            // Prepare FormData for upload
            const formData = new FormData();
            formData.append("file", file);

            const resp = await PhotoService.uploadProfilePhoto(file);
            if(resp.status==200){
                const url = await resp.text();
                setImageUrl(url);
            }
            onClose();
        });
    }

    return(
        <Modal content={
            <div className="grid">
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