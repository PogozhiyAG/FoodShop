import { createContext, useContext, useEffect } from "react";
import useBasket from "../hooks/useBasket";
import useAuth from "../hooks/useAuth";
import useOrder from "../hooks/useOrder";


export const BasketContext = createContext({});


export const BasketProvider = ({children}) => {    
    const auth = useAuth();
    const basket = useBasket();
    const order = useOrder(basket.positions);

    useEffect(() => basket.reload(), [auth.sync]);
    
       
    return (
        <BasketContext.Provider value={{basket, order}}>
            {children}
        </BasketContext.Provider>
    );
}