import Habit from "../types/Habit";
import API from "./API";

const url = import.meta.env.VITE_SERVER_URL+"/habits";

const HabitService ={
    getExistingHabits: async ()=>{
        return await API.get(url);
    },
    getHabits: async (date:string)=>{
        return await API.get(`${url}/${date}`);
    },
    createHabit: async (habit:Habit)=>{
        return await API.post(url,habit);
    },
    deactivateHabit: async (habitId: string )=>{
        return await API.delete(`${url}/deactivate/${habitId}`);
    },
    reactivateHabit: async (habitId: string )=>{
        return await API.post(`${url}/reactivate/${habitId}`);
    },
    editHabit: async (habit: Habit)=>{
        return await API.put(url,habit);
    },
    deleteHabit: async (habitId: string)=>{
        return await API.delete(`${url}/${habitId}`);
    }
}

export default HabitService;