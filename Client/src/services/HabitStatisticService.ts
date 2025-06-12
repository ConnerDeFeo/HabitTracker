import API from "./API";
const url = import.meta.env.VITE_SERVER_URL+"habitStatistics/";

const HabitStatisticService = {
    getHistoricalData: async (habitId: string)=>{
        return await API.get(`${url}${habitId}`);
    },
    getTotalValueByMonth: async (habitId:string, year:number)=>{
        return await API.get(`${url}totalValues?habitId=${habitId}&year=${year}`);
    }
}

export default HabitStatisticService;