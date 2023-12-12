import { useEffect } from "react";
import httpClient from "../api/httpClient";
import useAuth from './useAuth';

const useHttpClient = () => {    
    const {auth} = useAuth();

    useEffect(() => {
        const requestInterceptor = httpClient.interceptors.request.use(
            config => {
                //config.headers['Authorization'] = `Bearer ${auth.token}`
                config.headers['Authorization'] = 'Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNyc2Etc2hhMjU2IiwidHlwIjoiSldUIn0.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYjA4Zjg0MWUtNzA3Zi00NTZlLWIwNzMtZWExZTI0YTQ5ZDkzIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvYW5vbnltb3VzIjoiYjA4Zjg0MWUtNzA3Zi00NTZlLWIwNzMtZWExZTI0YTQ5ZDkzIiwiZXhwIjo0ODU3NjQ3MjQ1LCJpc3MiOiJGb29kU2hvcC5BcGkuQXV0aCIsImF1ZCI6IkZvb2RTaG9wIn0.I3M0C66KqJL-UPjkZ8z_4rvXjKpTad3UhpHfRfUKQLakPIFjAa9zO-Ek-HxAIjPySCewuaQ2xZKwsq_b4DYrmPvT-8ncCmyjSbXzq7dHCi_7sA07UFUIhzVnN7-7CCYBauegPS63cRZpzyCAorxf6BWCn7W4Mu_jbuS71zmzqRs'
            },
            error => {
                return new Promise.reject(error);
            }
        );

        return () => {
            httpClient.interceptors.request.eject(requestInterceptor);
        };
    }, [auth]);

    return httpClient;
};

export default useHttpClient;