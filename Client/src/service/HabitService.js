import API from "./API";

const HabitService = {
    PostUser: async (username, password)=>{
        return await API.post("http://localhost:5009/users",{Username: username, Password: password});
    }
}

export default HabitService;