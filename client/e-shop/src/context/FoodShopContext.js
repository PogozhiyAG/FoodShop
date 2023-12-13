import { createContext } from "react";
import AuthState from "../api/authState";


const FoodShopContext = createContext({});

export const FoodShopProvider = ({children}) => {

    //const value = new AuthState();

    return (
        <FoodShopContext.Provider value={1}>
            {children}
        </FoodShopContext.Provider>
    );
}

export default FoodShopContext;