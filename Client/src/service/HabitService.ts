import Habit from "../types/Habit";
import API from "./API";

const url = import.meta.env.VITE_SERVER_URL+"habits/";

const HabitService ={
    GetHabits: async ()=>{
        return await API.get(url);
    },
    CreateHabit: async (habit:Habit)=>{
        return await API.post(url,habit);
    },
    EditHabit: async (habit: Habit)=>{
        return await API.put(url,habit);
    }
}

export default HabitService;