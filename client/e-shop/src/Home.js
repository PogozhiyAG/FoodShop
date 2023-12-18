import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import Product from "./components/Product";
import useHttpClient from "./hooks/useHttpClient";
import useAuth from "./hooks/useAuth";
import useBasketContext from "./hooks/useContextBasket";


const Home = () => {
  const basket = useBasketContext();
  const [products, setProducts] = useState([]);
  const [searchText, setSearchText] = useState('');
  const [sortOrder, setSortOrder] = useState(0);
  const authSync = useAuth();
  const {getData} = useHttpClient();



  const getDataUrl = () => {
    let url = 'https://localhost:10443/Catalog?';
    url = url + `text=${searchText}`;
    url = url + `&sort=${sortOrder}`;
    return url;
  }

  useEffect(() => {
    basket.reloadBasket()
  }, []);


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
        <span className="p-2">{authSync.refreshToken ? 'Logged in' : 'Anonymous'}</span>
        <span className="m-2 h2 badge badge-success">{basket.getTotalAmount()}</span>
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

        <section className="row section mt-3 gap-3">
          {products.map(product => <Product key={product.id} product={product}/>)}
        </section>
      </main>
    </>
  );
};

export default Home;