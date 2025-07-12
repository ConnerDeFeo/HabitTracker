import API from "./API";

const url = import.meta.env.VITE_SERVER_URL+"users";

const UserService ={
    PostUser: async (username: string, password:string,email:string,deviceId:string)=>{
        return await API.post(url,{Username: username, Password: password,Email:email, DeviceId:deviceId});
    },
    GetUser: async ()=>{
        return await API.get(url);
    },
    Logout: async ()=>{
        return await API.post(`${url}/logout`,{});
    },
    Login: async (username: string, password:string, deviceId:string)=>{
        return await API.post(`${url}/login`,{Username: username, Password: password, DeviceId:deviceId});
    },
    GetProfile: async ()=>{
        return await API.get(`${url}/profile`);
    }
}

export default UserService;