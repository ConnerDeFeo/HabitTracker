import API from "./API";

const url = import.meta.env.VITE_SERVER_URL+"friends";

const FriendModificationService ={
    sendFriendRequest: async (friendUsername:string)=>{
        return await API.put(`${url}/${friendUsername}`);
    },
    unSendFriendRequest: async (friendUsername:string)=>{
        return await API.delete(`${url}/${friendUsername}`);
    },
    acceptFriendRequest: async (friendUsername:string)=>{
        return await API.post(`${url}/${friendUsername}`);
    },
    rejectFriendRequest: async (friendUsername:string)=>{
        return await API.delete(`${url}/${friendUsername}`);
    },
    removeFriend: async(friendUsername:string)=>{
        return await API.delete(`${url}/remove/${friendUsername}`);
    },
}

export default FriendModificationService;