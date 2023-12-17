import {authState} from './useAuth';

const AuthApiUrls = {
    RefreshUrl: 'https://localhost:11443/Authentication/refresh',
    AnonymousUrl: 'https://localhost:11443/Authentication/anonymous'
};

const useHttpClient = () => {     

  const getAccessToken = async (issuedToken) => {   
      let result = null; 

      if(authState.token){
        if(!issuedToken || authState.token !== issuedToken){
          return authState.token;
        }
      }
      
      if(authState.refreshToken){    
        result = await fetch(AuthApiUrls.RefreshUrl, {
          method: 'POST',
          headers: { 
            'Content-Type': 'application/json' 
          },
          body: JSON.stringify({refreshToken: authState.refreshToken})
        })
        .then(async r => {
          if(r.ok){
            const j = await r.json();
            authState.signIn(j.token, j.refreshToken);
            return authState.token;
          }
          if(r.status === 401){
            authState.signOut();
          }
        }).catch(e => {
          
        });
      }    
  
      if(result) return result;    
  
      if(authState.anonymousToken){
        if(authState.anonymousToken === issuedToken){
          authState.signOutAnonymous();
        }else{
          return authState.anonymousToken;        
        }
      }
          
      result = await fetch(AuthApiUrls.AnonymousUrl)      
        .then(async r => {
          if(r.ok){
            const j = await r.json();
            authState.signInAnonymous(j.token);
            return authState.token;
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
      return new Promise(async (resolve, reject) => {
        let token; 
        let repeat = 2;
        let result;
        let error;
    
        do {
          result = null;
          error = null;

          token = await getAccessTokenSafe(token);
        
          await fetch(url, {
            ...requestOptions,            
            headers: {      
              ...requestOptions?.headers,        
              Authorization: `Bearer ${token}`
            }
          })
          .then(async r => {
            result = r;  
            if(r.ok){
              repeat = 0;
            } else if(r.status === 401) {
              repeat--;
            } else {            
              //TODO:
              throw new Error('Response error');            
            }
          })
          .catch(e => {
            error = e;
            repeat = 0;
          });
    
        } while (repeat)
    
    
        if(error) {
          reject(error);
          return;
        }
        if(result){
          resolve(result);
          return;
        } 
      });
      
    };

    return {
        getData
    };
};

export default useHttpClient;