import axios from "axios";
import { handleError } from "./utils";

const api = process.env.REACT_APP_API_URL;

export async function registerAPI(username, email, password, confirmPassword) {
    
    try{

        const response = await axios.post(api + '/users/register',{
            'username' : username,
            'email' : email,
            'password' : password,
            'confirmPassword' : confirmPassword
        });

        return response.data;

    }
    catch(error){
        return {
            errorMessage: handleError(error)
        };
    }

}

export async function loginAPI(username, password) {
    
    try{

        const response = await axios.post(api + '/users/login',{
            'username' : username,
            'password' : password
        });

        return response.data;

    }
    catch(error){
        return {
            errorMessage: handleError(error)
        };
    }

}