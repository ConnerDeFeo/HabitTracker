import API from "./API";

const UserService ={
    PostUser: async (username: string, password:string)=>{
        return await API.post(import.meta.env.VITE_SERVER_URL+"users",{Username: username, Password: password});
    },
    GetUser: async (sessionKey:string)=>{
        return await API.get(import.meta.env.VITE_SERVER_URL+`users/${sessionKey}`);
    }
}

export default UserService;