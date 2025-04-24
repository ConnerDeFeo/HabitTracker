import API from "./API";

const UserService ={
    PostUser: async (username: string, password:string)=>{
        return await API.post(import.meta.env.VITE_SERVER_URL+"users",{Username: username, Password: password});
    },
    GetUser: async ()=>{
        return await API.get(import.meta.env.VITE_SERVER_URL+'users');
    }
}

export default UserService;