const AuthState = function () {
    this.token = null;     
    this.refreshToken = localStorage.refreshToken;     
    this.anonymousToken = localStorage.anonymousToken; 
    
    this.setRefreshToken = function (t) {
        this.refreshToken = t;
        if(t){
            localStorage.refreshToken = t;
        } else {
            delete localStorage.refreshToken;
        }
    };

    this.setAnonymousToken = (t) => {
        this.anonymousToken = t;
        if(t){
            localStorage.anonymousToken = t;
        } else {
            delete localStorage.anonymousToken;
        }
    };

    this.signIn = (t, rt) => {
        this.token = t;
        this.setRefreshToken(rt);
    };

    this.signOut = () => {
        this.token = null;
        this.setRefreshToken(null);
    };

    this.signInAnonymous = (t) => {
        this.token = t;
        this.setAnonymousToken(t);
        this.setRefreshToken(null);
    };

    this.signOutAnonymous = () => {
        this.token = null;
        this.setAnonymousToken(null);
    };
}

export default AuthState;