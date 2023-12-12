import { createContext } from "react";
import AuthState from "../api/authState";


const FoodShopContext = createContext({});

export const FoodShopProvider = ({children}) => {

    const value = AuthState();

    return (
        <FoodShopContext.Provider value={value}>
            {children}
        </FoodShopContext.Provider>
    );
}

export default FoodShopContext;