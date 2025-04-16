import API from "./API";

const UserService = {
    PostUser: async (username, password)=>{
        return await API.post(import.meta.env.VITE_SERVER_URL+"users",{Username: username, Password: password});
    }
}

export default UserService;