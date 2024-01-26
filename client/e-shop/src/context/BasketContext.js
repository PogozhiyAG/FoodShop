import { createContext } from "react";
import useOrder from "../hooks/useOrder";
import { useCustomerProfile } from "../hooks/useCustomerProfile";
import { useBasket } from "../hooks/useBasket";


export const BasketContext = createContext({});


export const BasketProvider = ({children}) => {
    const customerProfile = useCustomerProfile();
    const basket = useBasket();
    const order = useOrder({customerProfile, basket});
    
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