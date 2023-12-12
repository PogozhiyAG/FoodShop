import { useState } from "react";

const AuthState = () => {
    const [token, setToken] = useState();
    const [refreshToken, setRefreshTokenState] = useState(localStorage.refreshToken);
    const [anonymousToken, setAnonymousTokenState] = useState(localStorage.anonymousToken);

    
    const setRefreshToken = (t) => {
        setRefreshTokenState(t);
        if(t){
            localStorage.refreshToken = t;
        } else {
            delete localStorage.refreshToken;
        }
    };

    const setAnonymousToken = (t) => {
        setAnonymousTokenState(t);
        if(t){
            localStorage.anonymousToken = t;
        } else {
            delete localStorage.anonymousToken;
        }
    };

    const signIn = (t, rt) => {
        setToken(t);
        setRefreshToken(rt);
    };

    const signOut = () => {
        setToken(null);
        setRefreshToken(null);
    };

    const signInAnonymous = (t) => {
        setToken(t);
        setAnonymousToken(t);
        setRefreshToken(null);
    };

    const signOutAnonymous = () => {
        setToken(null);
        setAnonymousToken(null);
    };


    return {
        token,
        refreshToken,
        anonymousToken,
        signIn,
        signOut,
        signInAnonymous,
        signOutAnonymous
    };
}

export default AuthState;