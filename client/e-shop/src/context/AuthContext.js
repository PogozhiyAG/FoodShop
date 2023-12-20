import { createContext, useSyncExternalStore } from "react";
import AuthState from "../services/authState";


const state = new AuthState();


export const AuthContext = createContext({});

export const AuthContextProvider = ({children}) => {    
    const sync = useSyncExternalStore(state.subscribe, state.getSnapshot);
    
    const value = {        
        state,
        sync
    };

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );    
}