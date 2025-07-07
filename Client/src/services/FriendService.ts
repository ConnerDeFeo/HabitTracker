import API from "./API";

const url = import.meta.env.VITE_SERVER_URL+"friends";

const FriendService ={
    findUsers: async (phrase:string)=>{
        return await API.get(`${url}/find/${phrase}`);
    },
}

export default FriendService;