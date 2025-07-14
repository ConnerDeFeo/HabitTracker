import API from "./API";

const url = import.meta.env.VITE_SERVER_URL+"/googleAuth";

const GoogleAuthService ={
    login: async (jwt:string,deviceId:string)=>{
        return await API.post(`${url}/login`,{Jwt:jwt, DeviceId:deviceId});
    }
}

export default GoogleAuthService;