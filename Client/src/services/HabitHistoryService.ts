import API from "./API";

const url = import.meta.env.VITE_SERVER_URL+"habitHistory/";

const HabitHistoryService ={
    getMonth: async (yyyyMM: string)=>{
        return await API.get(`${url}${yyyyMM}`);
    }
}

export default HabitHistoryService;