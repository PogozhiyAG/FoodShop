import { createContext, useState } from "react";

const FoodShopContext = createContext({});

export const FoodShopProvider = ({children}) => {
    const [auth, setAuth] = useState();

    return (
        <FoodShopContext.Provider value={{auth, setAuth}}>
            {children}
        </FoodShopContext.Provider>
    );
}

export default FoodShopContext;