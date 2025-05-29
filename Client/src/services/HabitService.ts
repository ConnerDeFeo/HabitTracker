import Habit from "../types/Habit";
import API from "./API";

const url = import.meta.env.VITE_SERVER_URL+"habits/";

const HabitService ={
    getHabits: async (date:string)=>{
        return await API.get(`${url}${date}`);
    },
    createHabit: async (habit:Habit)=>{
        return await API.post(url,habit);
    },
    editHabit: async (habit: Habit)=>{
        return await API.put(url,habit);
    },
    deleteHabit: async (habitId: string)=>{
        return await API.delete(`${url}month/${habitId}`);
    },
    completeHabit: async (habitId: string, date:string, completed: boolean)=>{
        return await API.put(url+"habitCompletion",{ HabitId:habitId, Date: date, Completed: completed });
    },
    getMonth: async (yyyyMM: string)=>{
        return await API.get(`${url}month/${yyyyMM}`);
    }
}

export default HabitService;