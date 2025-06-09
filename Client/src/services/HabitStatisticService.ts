import Habit from "../types/Habit";
import API from "./API";

const url = import.meta.env.VITE_SERVER_URL+"habitStatistic/";

const HabitStatisticService = {
    getHistoricalData: async (habit:Habit)=>{
        return await API.get(url);
    },
}

export default HabitStatisticService;