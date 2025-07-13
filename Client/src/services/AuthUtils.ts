const AuthUtils = {
    //Generates unique device id if not already present
    getDeviceId:():string=>{
        let deviceId = localStorage.getItem("deviceId");
        if (!deviceId) {
            deviceId = crypto.randomUUID();
            localStorage.setItem("deviceId", deviceId);
        }
        return deviceId;
    }
}

export default AuthUtils;