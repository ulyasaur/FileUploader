import axios from "axios"; 
 
export default axios.create({ 
    baseURL: "https://reenbitfileuploaderapi.azurewebsites.net/api", 
    headers: { 
        'Content-Type': 'application/json' 
    }, 
})