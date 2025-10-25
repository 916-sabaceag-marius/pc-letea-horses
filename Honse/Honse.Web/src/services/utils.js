import axios from "axios";

export function handleError(error){

    // console.log(error);
    if(!axios.isAxiosError(error) || error.code === "ERR_NETWORK"){
        return "Internal server error"; 
    }

    
    let errorData = error.response?.data.errorMessage;

    if(!errorData) errorData = error.response?.data;

    return errorData;
}
