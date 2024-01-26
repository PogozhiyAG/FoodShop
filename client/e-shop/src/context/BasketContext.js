import { createContext, useEffect } from "react";
import useAuth from "../hooks/useAuth";
import useOrder from "../hooks/useOrder";
import { useCustomerProfile } from "../hooks/useCustomerProfile";
import { useBasket } from "../hooks/useBasket";


export const BasketContext = createContext({});


export const BasketProvider = ({children}) => {    
    const auth = useAuth();    
    const customerProfile = useCustomerProfile();
    const basket = useBasket();
    const order = useOrder({customerProfile, basket});
    

    useEffect(() => {
        Promise.allSettled([
            basket.fetchData(),
            customerProfile.fetchData()
        ]).then(values => {            
            basket.setBasket(values[0].value);
            customerProfile.setProfile(values[1].value)
        });
    }, [auth.sync]);

       
    return (
        <BasketContext.Provider value={{
            order, 
            customerProfile, 
            basket
        }}>
            {children}
        </BasketContext.Provider>
    );
}