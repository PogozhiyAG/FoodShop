import { createContext, useEffect } from "react";
import useBasket from "../hooks/useBasket";


const FoodShopContext = createContext({});

export const FoodShopProvider = ({children}) => {

    const basket = useBasket();

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