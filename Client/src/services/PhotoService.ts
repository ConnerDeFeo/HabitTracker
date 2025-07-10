import API from "./API";
const url = import.meta.env.VITE_SERVER_URL+"photos";

const PhotoService = {
    getPhotoUrl : (userId?:string): string=>{
        return `${import.meta.env.VITE_PROFILE_PHOTO_URL}/${userId}?t=${Date.now()}`;
    },
    uploadProfilePhoto: async (file: File)=>{
        return await API.upload(url, file);
    },
}

export default PhotoService;