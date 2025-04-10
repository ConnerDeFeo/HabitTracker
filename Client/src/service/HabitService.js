import API from "./API";

const HabitService = {
    PostUser: async (username)=>{
        return await API.post("http://localhost:5009/users",username);
    }
}

export default HabitService;