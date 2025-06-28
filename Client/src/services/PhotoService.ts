import API from "./API";
const url = import.meta.env.VITE_SERVER_URL+"photos/";

const PhotoService = {
    getProfilePhoto: async ()=>{
        return await API.get(url);
    },
    uploadProfilePhoto: async (file: File)=>{
        return await API.upload(url, file);
    }
}

export default PhotoService;