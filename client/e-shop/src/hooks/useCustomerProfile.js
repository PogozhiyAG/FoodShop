import { useState } from "react";
import useHttpClient from "./useHttpClient";

export const useCustomerProfile = () => {
    const[profile, setProfile] = useState();
    const {getData} = useHttpClient();

    const fetchData = () => 
        getData( 'https://localhost:12443/CustomerProfile')
        .then(r => r.json());

    const reload = () =>
        fetchData()       
        .then(r => setProfile(r));

    return {
        profile,
        reload,
        setProfile,
        fetchData
    };
}