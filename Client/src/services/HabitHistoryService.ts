import API from "./API";

const url = import.meta.env.VITE_SERVER_URL+"/habitHistory";

const HabitHistoryService ={
    getMonth: async (yyyyMM: string)=>{
        return await API.get(`${url}/${yyyyMM}`);
    },
    completeHabit: async (habitId: string, date:string, completed: boolean)=>{
        return await API.put(url,{ HabitId:habitId, Date: date, Completed: completed });
    },
}

export default HabitHistoryService;