import { PixelCrop } from "react-image-crop";

const Canvas = (
    image: HTMLImageElement,
    canvas: HTMLCanvasElement,
    crop: PixelCrop
) => {
    const ctx = canvas.getContext("2d");
    if (!ctx) 
        throw new Error("No 2d context");

    const pixelRatio = window.devicePixelRatio;
    const scaleX = image.naturalWidth / image.width;
    const scaleY = image.naturalHeight / image.height;

    const cropX = crop.x * scaleX;
    const cropY = crop.y * scaleY;
    const cropWidth = crop.width * scaleX;
    const cropHeight = crop.height * scaleY;

    canvas.width = Math.floor(cropWidth * pixelRatio);
    canvas.height = Math.floor(cropHeight * pixelRatio);

    ctx.setTransform(pixelRatio, 0, 0, pixelRatio, 0, 0);
    ctx.imageSmoothingQuality = "high";

    ctx.drawImage(
        image,
        cropX,       // Start X in source image
        cropY,       // Start Y in source image
        cropWidth,   // Width to crop from source
        cropHeight,  // Height to crop from source
        0,           // X on canvas
        0,           // Y on canvas
        cropWidth,   // Width on canvas
        cropHeight   // Height on canvas
    );
};

export default Canvas;