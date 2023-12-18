const AuthState = function () {
    this.token = null;     
    this.refreshToken = localStorage.refreshToken;     
    this.anonymousToken = localStorage.anonymousToken; 
    this.userName = localStorage.userName;
    
    this.setRefreshToken = t => {
        this.refreshToken = t;
        if(t){
            localStorage.refreshToken = t;
        } else {
            delete localStorage.refreshToken;
        }
    };

    this.setAnonymousToken = t => {
        this.anonymousToken = t;
        if(t){
            localStorage.anonymousToken = t;
        } else {
            delete localStorage.anonymousToken;
        }
    };

    this.setUserName = userName => {
        this.userName = userName;
        if(userName){
            localStorage.userName = userName;
        } else {
            delete localStorage.userName;
        }
    };

    this.signIn = (t, rt, un) => {
        this.token = t;
        this.setRefreshToken(rt);
        this.setUserName(un);
        this.emitChange();
    };

    this.signOut = () => {
        this.token = null;
        this.setRefreshToken(null);
        this.setUserName(null);
        this.emitChange();
    };

    this.signInAnonymous = (t) => {
        this.token = null;
        this.setAnonymousToken(t);
        this.setRefreshToken(null);
        this.setUserName(null);
        this.emitChange();
    };

    this.signOutAnonymous = () => {
        this.token = null;
        this.setAnonymousToken(null);
        this.setUserName(null);
        this.emitChange();
    };

    this.listeners = [];
    this.subscribe = listener => {
        this.listeners = [...this.listeners, listener];
        return () => {
            this.listeners = this.listeners.filter(l => l !== listener);
        };
    }

    
    this.getSnapshot = () => this.userName ?? this.anonymousToken;
    
    this.emitChange = () => { 
        for (let listener of this.listeners) {
          listener();
        }
    }
}

export default AuthState;