import { createContext, useContext, useEffect } from "react";
import useBasket from "../hooks/useBasket";
import useAuth from "../hooks/useAuth";


export const BasketContext = createContext({});


export const BasketProvider = ({children}) => {    
    const auth = useAuth();
    const basket = useBasket();

    useEffect(() => basket.reload(), [auth.sync]);
       
    return (
        <BasketContext.Provider value={basket}>
            {children}
        </BasketContext.Provider>
    );
}