import API from "./API";
const url = import.meta.env.VITE_SERVER_URL+"habitStatistics/";

const HabitStatisticService = {
    getHistoricalData: async (habitId: string)=>{
        return await API.get(`${url}${habitId}`);
    },
    getTotalValueByMonth: async (habitId:string, yearsBack?:number)=>{
        return await API.get(`${url}totalValues?habitId=${habitId}&yearsBack=${yearsBack ?? 0}`);
    }
}

export default HabitStatisticService;