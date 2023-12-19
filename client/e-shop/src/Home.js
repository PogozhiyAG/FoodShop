import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import Product from "./components/Product";
import useHttpClient from "./hooks/useHttpClient";
import useBasketContext from "./hooks/useContextBasket";
import useAuth from "./hooks/useAuth";
import { authState } from "./hooks/useAuth";


const Home = () => {
  const basket = useBasketContext();
  const [products, setProducts] = useState([]);
  const [searchText, setSearchText] = useState('');
  const [sortOrder, setSortOrder] = useState(0);
  
  const {getData} = useHttpClient();

  const getUserDisplayName = () => authState.userName ? 'Logged in' : 'Anonymous';

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
        <Link className="p-2" to="/basket">Basket</Link>
        <Link className="p-2" to="/login">Login</Link>
        <span className="p-2">{getUserDisplayName()}</span>
        <span className="m-2 h2 badge badge-success">{basket.getTotalAmount()}</span>
      </header>
    


      <main className="container">
        <section className="">
          <img src="banner.jpg" className="banner-image"/>
        </section>

        <section className="row g-0">          
          <a href="#" className="  col-md-2 col-sm-12">
             <span className="p-4 category-galery-item category-galery-item-three">Meat</span>
          </a>
          <a href="#" className=" col-md-2 col-sm-12">
            <span className="p-4 category-galery-item category-galery-item-two">Meat</span>
          </a>
          <a href="#" className="  col-md-4 col-sm-12">
            <span className="p-4 category-galery-item category-galery-item-three">Meat</span>
          </a>
          <a href="#" className="  col-md-4 col-sm-12">
            <span className="p-4 category-galery-item category-galery-item-one">Meat</span>
          </a>
          <a href="#" className="  col-md-6 col-sm-12">
            <span className="p-5 category-galery-item category-galery-item-one">Drinks</span>
          </a>
          <a href="#" className="  col-md-6 col-sm-12">
            <span className="p-5 category-galery-item category-galery-item-two">Meat</span>
          </a>
        </section>

        <section className="">
          <div className="d-flex gap-3">
            <Link to="/tag/1">Christmas</Link>
            <Link to="/tag/2">Health</Link>
            <Link to="/tag/3">FS Recommend</Link>
            <Link to="/tag/4">Product of the Week</Link>
          </div>
        </section>
        
        <section className="">
          <div>
            <input className="search-input" value={searchText} onChange={e => setSearchText(e.target.value)}></input>
            <select className="pull-right" value={sortOrder} onChange={e => setSortOrder(e.target.value)}>
              <option value={0}>Popularity</option>
              <option value={1}>Price</option>
              <option value={2}>Rating</option>
            </select>
          </div>
        </section>

        

        <section className="row mt-3 g-3">
          {products.map(product => <Product key={product.id} product={product}/>)}
        </section>
      </main>
    </>
  );
};

export default Home;