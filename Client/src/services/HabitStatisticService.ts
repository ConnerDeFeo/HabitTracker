import API from "./API";
const url = import.meta.env.VITE_SERVER_URL+"habitStatistics/";

const HabitStatisticService = {
    getHistoricalData: async (habitId: string)=>{
        return await API.get(`${url}${habitId}`);
    },
}

export default HabitStatisticService;