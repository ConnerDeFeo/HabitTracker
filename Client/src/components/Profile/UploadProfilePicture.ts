import PhotoService from "../../services/PhotoService";

//Resizes and adjust quality of a given canvas element
const resizeCanvasAndExportBlob = (
    sourceCanvas: HTMLCanvasElement,
    maxWidth: number,
    quality: number
): Promise<Blob> => {
    return new Promise((resolve) => {
        const scale = Math.min(maxWidth / sourceCanvas.width, 1);
        const canvas = document.createElement("canvas");
        canvas.width = sourceCanvas.width * scale;
        canvas.height = sourceCanvas.height * scale;

        const ctx = canvas.getContext("2d");
        if (!ctx) 
            return;

        ctx.drawImage(sourceCanvas, 0, 0, canvas.width, canvas.height);

        canvas.toBlob((blob) => {
        if (!blob) 
            return;
            resolve(blob);
        }, "image/jpeg", quality);
    });
};


const UploadProfilePicture = async (canvas: HTMLCanvasElement, setImageUrl:React.Dispatch<React.SetStateAction<string>>,onClose:()=>void) => {
    // Resize and compress blob incase file size is to large
    const blob = await resizeCanvasAndExportBlob(canvas, 500, 0.75);
    const file = new File([blob], "ProfilePicture.jpg", { type: "image/jpeg" });

    const resp = await PhotoService.uploadProfilePhoto(file);
    if (resp.status === 200) {
        const url = await resp.text();
        console.log(url)
        setImageUrl(url);
    }
    onClose();
};

export default UploadProfilePicture;