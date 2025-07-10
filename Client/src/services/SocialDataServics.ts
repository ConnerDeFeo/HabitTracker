import API from "./API";

const url = import.meta.env.VITE_SERVER_URL+"friends";

const SocialDataService ={
    findUsers: async (phrase:string)=>{
        return await API.get(`${url}/find/${phrase}`);
    },
    getRandomUsers: async ()=>{
        return await API.get(`${url}/random`);
    },
    getProfile: async(username:string)=>{
        return await API.get(`${url}/profile/${username}`);
    }
}

export default SocialDataService;