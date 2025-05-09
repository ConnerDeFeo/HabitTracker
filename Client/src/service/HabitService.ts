import API from "./API";

const url = import.meta.env.VITE_SERVER_URL+"habits/";

const HabitService ={
    GetHabits: async ()=>{
        return await API.get(url);
    },
    CreateHabit: async (habitName:string)=>{
        return await API.post(url,{Name:habitName});
    },
}

export default HabitService;