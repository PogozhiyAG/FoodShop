import useAuth from './useAuth';

const AuthApiUrls = {
    RefreshUrl: 'https://localhost:11443/Authentication/refresh',
    AnonymousUrl: 'https://localhost:11443/Authentication/anonymous'
};

const useHttpClient = () => {    
    const auth = useAuth();

    const getAccessToken = async (issuedToken) => {   
        let result = null; 

        if(auth.token){
          if(!issuedToken || auth.token !== issuedToken){
            return auth.token;
          }
        }
        
        if(auth.refreshToken){    
          result = await fetch(AuthApiUrls.RefreshUrl, {
            method: 'POST',
            headers: { 
              'Content-Type': 'application/json' 
            },
            body: JSON.stringify({refreshToken: auth.refreshToken})
          })
          .then(async r => {
            if(r.ok){
              const j = await r.json();
              auth.signIn(j.token, j.refreshToken);
              return auth.token;
            }
            if(r.status == 401){
              auth.signOut();
            }
          }).catch(e => {
            
          });
        }    
    
        if(result) return result;    
    
        if(auth.anonymousToken){
          if(auth.anonymousToken === issuedToken){
            auth.signOutAnonymous();
          }else{
            return auth.anonymousToken;        
          }
        }
            
        result = await fetch(AuthApiUrls.AnonymousUrl)      
          .then(async r => {
            if(r.ok){
              const j = await r.json();
              auth.signInAnonymous(j.token);
              return auth.token;
            } 
          })
          .catch(e => {
    
          });
    
        return result;
      };
    
      
      const getAccessTokenSafe = async (issuedToken) => {
        let accessToken;

        await navigator.locks.request('REFRESH_TOKEN', async lock => {          
          accessToken = await getAccessToken(issuedToken);
        });

        return accessToken;
      };
        
    
      const getData = async (url, requestOptions) => {
        let token; 
        let repeat = 2;
        let result;
        let error;
    
        do {
          token = await getAccessTokenSafe(token);
    
          await fetch(url, {
            ...requestOptions,
            headers: {
              Authorization: `Bearer ${token}`
            }
          })
          .then(async r => {
            result = r;  
            if(r.ok){
              repeat = 0;
            } else if(r.status == 401) {
              repeat--;
            } else {
              repeat = 0;
            }
          })
          .catch(e => {
            error = e;
            repeat = 0;
          });
    
        } while (repeat)
    
    
        if(error) return Promise.reject(error);
        if(result) return Promise.resolve(result);
      };

    return {
        getData
    };
};

export default useHttpClient;