const API = {
    get: async (path: string): Promise<Response> => {
      return await fetch(path, {
        credentials: 'include', 
        method: 'GET',  
        headers: {
          'Content-Type': 'application/json',  
        }
      });
    },
    post: async (path: string, data: Record<string, any>): Promise<Response> => {
      return await fetch(path, {
        credentials: 'include', 
        method: 'POST',  
        headers: {
          'Content-Type': 'application/json',  
        },
        body: JSON.stringify(data),
      });
    }
  };
  
  export default API;