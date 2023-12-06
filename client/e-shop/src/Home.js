import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import Product from "./components/Product";


const Home = () => {
  const [products, setProducts] = useState([]);
  const [searchText, setSearchText] = useState('');
  const [sortOrder, setSortOrder] = useState('');

  const getDataUrl = () => {
    let url = 'https://localhost:10443/Catalog?';
    url = url + `text=${searchText}`;
    url = url + `&sort=${sortOrder}`;
    return url;
  }

  useEffect(() => {
    fetch(getDataUrl(), {
      headers: {
        Authorization: 'Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNyc2Etc2hhMjU2IiwidHlwIjoiSldUIn0.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiOGE0OTUwYWEtMzdiYi00ODU4LTg5ZjctNGJjYjEyNjA4NGRjIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvYW5vbnltb3VzIjoiOGE0OTUwYWEtMzdiYi00ODU4LTg5ZjctNGJjYjEyNjA4NGRjIiwiZXhwIjo0ODU3NDg5Mjg5LCJpc3MiOiJGb29kU2hvcC5BcGkuQXV0aCIsImF1ZCI6IkZvb2RTaG9wIn0.bTFqdLT1YTXpeBCsA_Tm3ly28ujtkm7N6yO8ghLMg0x32VBRQmshKaiCPNyp625HVCfyVuYrls9dUFPIzhKwT--EEf2w3vkor00Di7MQGCfLn52Z_zyMOLYE218U26Rk6XzYTxYsyp-ejUfWuaSpOIUGZuFB0h_X9b-1aCNstBs'
      }
    })
    .then(r => r.json())
    .then(r => setProducts(r))
  }, [searchText, sortOrder]);


  return (
    <>
    
      <header className="header">
        <img src="logo.png" style={{width: '50px', height: '50px'}}/>
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
          <a href="#" className="category-galery-item category-galery-item-one p-5 col-md-4 col-sm-12">To-Go</a>
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
          {products.map(product => <Product product={product}/>)}
        </section>
      </main>
    </>
  );
};

export default Home;