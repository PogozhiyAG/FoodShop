import { createContext, useEffect } from "react";
import useAuth from "../hooks/useAuth";
import useOrder from "../hooks/useOrder";


export const BasketContext = createContext({});


export const BasketProvider = ({children}) => {    
    const auth = useAuth();    
    const order = useOrder();

    useEffect(() => {
        console.log('DBG_01');
        order.basket.reload();
    }, [auth.sync]);
    
       
    return (
        <BasketContext.Provider value={order}>
            {children}
        </BasketContext.Provider>
    );
}