import API from "./API";

const url = import.meta.env.VITE_SERVER_URL+"habits/";

const HabitService ={
    GetHabits: async ()=>{
        return await API.get(url);
    },
}

export default HabitService;