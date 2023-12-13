import { useState } from "react";

const AuthState = function () {
   // [this.stateString, this.setStateString] = useState();

    this.token = null; 
    this.setToken = function (v) { 
        this.token = v;
    };
    this.refreshToken = localStorage.refreshToken; 
    this.setRefreshTokenState = function(v){ this.refreshToken = v}; 
    this.anonymousToken = localStorage.anonymousToken; 
    this.setAnonymousTokenState = function(v){ this.anonymousToken = v}; 

 
   

    this.setRefreshToken = function (t) {
        this.setRefreshTokenState(t);
        if(t){
            localStorage.refreshToken = t;
        } else {
            delete localStorage.refreshToken;
        }
    };

    this.setAnonymousToken = function (t) {
        this.setAnonymousTokenState(t);
        if(t){
            localStorage.anonymousToken = t;
        } else {
            delete localStorage.anonymousToken;
        }
    };

    this.signIn = function (t, rt) {
        this.setToken(t);
        this.setRefreshToken(rt);
        //this.setStateString(`signIn: ${t}`);
    };

    this.signOut = function () {
        this.setToken(null);
        this.setRefreshToken(null);
        //this.setStateString('signOut');
    };

    this.signInAnonymous = function (t) {
        this.setToken(t);
        this.setAnonymousToken(t);
        this.setRefreshToken(null);
        //this.setStateString('signInAnonymous');
    };

    this.signOutAnonymous = function () {
        this.setToken(null);
        this.setAnonymousToken(null);
       // this.setStateString('signOutAnonymous');
    };


    // return {
    //     token,
    //     refreshToken,
    //     anonymousToken,
    //     signIn,
    //     signOut,
    //     signInAnonymous,
    //     signOutAnonymous
    // };
}

export default AuthState;