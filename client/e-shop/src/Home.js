import { useEffect, useState } from "react";
import { Link } from "react-router-dom";


const Home = () => {
  const [products, setProducts] = useState([]);

  useEffect(() => {
    fetch('https://localhost:10443/Catalog')
    .then(r => r.json())
    .then(r => setProducts(r))
  }, []);


  return (
    <>
      <header style={{display: "flex", alignItems: "center", gap: "16px"}}>
        <img src="logo.png" style={{width: '50px', height: '50px'}}/>
        <h1 style={{margin: 0}}>Home</h1>
      </header>
      <nav>
        <ul>          
          <li>
            <Link to="/category">Category</Link>
          </li>
          <li>
            <Link to="/product">Product</Link>
          </li>
          <li>
            <Link to="/basket">Basket</Link>
          </li>
        </ul>
      </nav>
      <div className="container">
        {products.map(i => <div>{i.name}</div>)}
      </div>
    </>
  );
};

export default Home;