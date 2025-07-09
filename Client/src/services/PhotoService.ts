import API from "./API";
const url = import.meta.env.VITE_SERVER_URL+"photos";

const PhotoService = {
    getProfilePhoto: async ()=>{
        return await API.get(url);
    },
    uploadProfilePhoto: async (file: File)=>{
        return await API.upload(url, file);
    },
    validateImageUrl: async (imageUrl:string): Promise<boolean>=>{
        if(imageUrl){
            const res = await fetch(imageUrl, { method: "HEAD" }); // HEAD is faster, gets headers only
            if (res.ok) 
                return true;
        }
        return false;
    }
}

export default PhotoService;