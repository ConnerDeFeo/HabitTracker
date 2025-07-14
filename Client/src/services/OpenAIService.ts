import API from "./API";
const url = import.meta.env.VITE_SERVER_URL+"/openAI";

const OpenAIService = {
    getReccomendation: async ()=>{
        return await API.get(url);
    }
}

export default OpenAIService;