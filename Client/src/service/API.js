const API = {
    post: async (path, data) => {
      console.log(JSON.stringify(data));
        return await fetch(path, {
          credentials: "include", 
          method: "POST",  
          headers: {
            "Content-Type": "application/json",  
          },
          body: JSON.stringify(data),
        });
    }
}

export default API;
