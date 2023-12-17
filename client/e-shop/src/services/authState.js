const AuthState = function () {
    this.token = null;     
    this.refreshToken = localStorage.refreshToken;     
    this.anonymousToken = localStorage.anonymousToken; 
    
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

    this.signIn = (t, rt) => {
        this.token = t;
        this.setRefreshToken(rt);
        this.emitChange();
    };

    this.signOut = () => {
        this.token = null;
        this.setRefreshToken(null);
        this.emitChange();
    };

    this.signInAnonymous = (t) => {
        this.token = t;
        this.setAnonymousToken(t);
        this.setRefreshToken(null);
        this.emitChange();
    };

    this.signOutAnonymous = () => {
        this.token = null;
        this.setAnonymousToken(null);
        this.emitChange();
    };

    this.listeners = [];
    this.subscribe = listener => {
        this.listeners = [...this.listeners, listener];
        return () => {
            this.listeners = this.listeners.filter(l => l !== listener);
        };
    }

    this.getSnapshot = () => this.snapshot;
    
    this.updateSnapshot = () => {
        this.snapshot = {
            token: this.token,
            refreshToken: this.refreshToken,
            anonymousToken: this.anonymousToken
        };
    }
    this.updateSnapshot();
    
    this.emitChange = () => { 
        this.updateSnapshot();
        for (let listener of this.listeners) {
          listener();
        }
    }
}

export default AuthState;