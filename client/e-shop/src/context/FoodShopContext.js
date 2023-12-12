import { createContext, useState } from "react";

const FoodShopContext = createContext({});

export const FoodShopProvider = ({children}) => {
    const [token, setToken] = useState();
    const [refreshToken, setRefreshToken] = useState(localStorage.refreshToken);
    const [anonymousToken, setAnonymousToken] = useState(localStorage.refreshToken);

    

    const setAnonymous = (at) => {
        setToken(at);

        setAnonymousToken(at);
        localStorage.anonymousToken = at;

        delete localStorage.refreshToken;
        setRefreshToken(null);
        
    };

    const setSignIn = (t, rt) => {
        setToken(t);

        localStorage.refreshToken = rt;
        setRefreshToken(rt);
    };

    const value = {
        token, setToken,
        refreshToken, setRefreshToken,
        anonymousToken, setAnonymousToken,
        setAnonymous,
        setSignIn
    };

    return (
        <FoodShopContext.Provider value={value}>
            {children}
        </FoodShopContext.Provider>
    );
}

export default FoodShopContext;