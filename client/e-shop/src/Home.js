import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import Product from "./components/Product";
import useAuth from "./hooks/useAuth";


const Home = () => {
  const [products, setProducts] = useState([]);
  const [searchText, setSearchText] = useState('');
  const [sortOrder, setSortOrder] = useState('');
  const auth = useAuth();
  

  // let LoginState = {
  //   token: null,
  //   refreshToken: localStorage.refreshToken,
  //   anonymousToken: localStorage.anonymousToken,

  //   deleteAnonymous: function(){
  //     this.anonymousToken = null;
  //     delete localStorage.anonymousToken;
  //   },

  //   processLogin: function (token , refreshToken) {
  //     this.token = token;
  //     this.refreshToken = refreshToken;
  //     localStorage.refreshToken = refreshToken;
  //   },
  //   processAnonymous: function (token) {
  //     this.token = token;
  //     this.refreshToken = null;
  //     this.anonymousToken = token;
  //     localStorage.anonymousToken = token;
  //     delete localStorage.refreshToken;
  //   },
  //   processLogout: function () {
  //     this.token = null;
  //     this.refreshToken = null;
  //     delete localStorage.refreshToken;
  //   }
  // }

 

  // const login = async () => await fetch('https://localhost:11443/Authentication/login', {
  //     method: 'POST',
  //     headers: { 
  //       'Content-Type': 'application/json' 
  //     },
  //     body: JSON.stringify({
  //       "userName": "string",
  //       "password": "String@1"
  //     })
  //     }).then(async r => {
  //       if(r.ok){
  //         const j = await r.json();
  //         LoginState.processLogin(j.token, j.refreshToken);
  //         return j.token;
  //       } 
  //       if(r.status == 401){
  //         console.log('Invalid login or password');
  //       }
  //     }).catch(e => {

  //     });




  const getAccessToken = async (issuedToken) => {   
    let result = null; 

    if(auth.token){
      if(!issuedToken || auth.token !== issuedToken){
        return auth.token;
      }
    }


    if(auth.refreshToken){
      console.log('DEBUG: We are going to refresh');

      result = await fetch('https://localhost:11443/Authentication/refresh', {
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
          return j.token;
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

    console.log('DEBUG: We are going to switch to anonymous');
    result = await fetch('https://localhost:11443/Authentication/anonymous')      
      .then(async r => {
        if(r.ok){
          const j = await r.json();
          auth.signInAnonymous(j.token);
          return j.token;
        } 
      })
      .catch(e => {

      });

    if(result) return result;
  };





  let requestPromise = null;
  const getAccessTokenSafe = async (issuedToken) =>{
    if(requestPromise == null){
      console.log('DBG_AT');
      requestPromise = getAccessToken(issuedToken);
    }

    const accessToken =  await requestPromise;
    requestPromise = null;
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
  }



  const getDataUrl = () => {
    let url = 'https://localhost:10443/Catalog?';
    url = url + `text=${searchText}`;
    url = url + `&sort=${sortOrder}`;
    return url;
  }



  useEffect(() => {
    getData(getDataUrl())
    .then(async r => {
      setProducts(await r.json());
    });
  }, [searchText, sortOrder]);

 


  return (
    <>
    
      <header className="header">
        <img src="logo.png" style={{width: '50px', height: '50px'}}/>
        <Link to="/basket">Basket</Link>
        <Link to="/login">Login</Link>
        <span className="p-2">{auth.refreshToken ? 'Logged in' : 'Anonymous'}</span>
      </header>
    


      <main className="container">
        <section className="section">
          <img src="banner.jpg" className="banner-image"/>
        </section>

        <section className="row section">
          <a href="#" className="category-galery-item category-galery-item-one p-5 col-md-6 col-sm-12">Drinks</a>
          <a href="#" className="category-galery-item category-galery-item-two p-5 col-md-6 col-sm-12">Meat</a>
          <a href="#" className="category-galery-item category-galery-item-three p-5 col-md-2 col-sm-12">Vegitables</a>
          <a href="#" className="category-galery-item category-galery-item-two p-5 col-md-2 col-sm-12">Fruit</a>
          <a href="#" className="category-galery-item category-galery-item-three p-5 col-md-4 col-sm-12">Sweets</a>
          <a href="#" className="category-galery-item category-galery-item-one p-5 col-md-4 col-sm-12">Take away</a>
        </section>

        <section className="section">
          <div className="d-flex gap-3">
            <Link to="/tag/1">Christmas</Link>
            <Link to="/tag/2">Health</Link>
            <Link to="/tag/3">FS Recommend</Link>
            <Link to="/tag/4">Product of the Week</Link>
          </div>
        </section>
        
        <section className="section">
          <div>
            <input className="search-input" value={searchText} onChange={e => setSearchText(e.target.value)}></input>
            <select className="pull-right" value={sortOrder} onChange={e => setSortOrder(e.target.value)}>
              <option value={0}>Popularity</option>
              <option value={1}>Price</option>
              <option value={2}>Rating</option>
            </select>
          </div>
        </section>

        <section className="row section">
          {products.map(product => <Product key={product.id} product={product}/>)}
        </section>
      </main>
    </>
  );
};

export default Home;