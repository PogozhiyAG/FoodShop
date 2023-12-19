import { createContext, useEffect } from "react";
import useBasket from "../hooks/useBasket";
import useAuth from "../hooks/useAuth";


const FoodShopContext = createContext({});

export const FoodShopProvider = ({children}) => {
    const authSync = useAuth();
    const basket = useBasket();

    useEffect(() => {
        basket.reloadBasket()
      }, [authSync]);

    const value = {
        basket
    };
    
    return (
        <FoodShopContext.Provider value={value}>
            {children}
        </FoodShopContext.Provider>
    );
}

export default FoodShopContext;