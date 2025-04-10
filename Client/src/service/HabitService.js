import API from "./API";

const HabitService = {
    PostUser: async (username)=>{
        return await API.post(process.env.REACT_APP_SERVER_URL,{username: username});
    }
}

export default HabitService;