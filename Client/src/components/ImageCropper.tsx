import { useState, useCallback } from "react";
import Cropper, { Area } from "react-easy-crop";

const ImageCropper = (props:{ imageSrc:string, onCropComplete:(croppedAreaPixels: Area) => void }) => {
    const {imageSrc,onCropComplete} = props;
    const [crop, setCrop] = useState<{ x: number; y: number }>({ x: 0, y: 0 });
    const [zoom, setZoom] = useState<number>(1);

    const handleCropComplete = useCallback(
        (croppedAreaPixels: Area) => {
            onCropComplete(croppedAreaPixels);
        },
        [onCropComplete]
    );

    return (
        <div className="relative w-40 h-40">
        <Cropper
            image={imageSrc}
            crop={crop}
            zoom={zoom}
            aspect={1}
            onCropChange={setCrop}
            onZoomChange={setZoom}
            onCropComplete={handleCropComplete}
        />
        </div>
    );
};

export default ImageCropper;