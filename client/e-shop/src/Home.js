import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

const Home = () => {
  const [products, setProducts] = useState([]);

  useEffect(() => {
    fetch('https://localhost:62788/Catalog')
    .then(r => r.json())
    .then(r => setProducts(r))
  }, []);


  return (
    <>
      <h1>Home</h1>
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
      {products.map(i => <div>{i.name}</div>)}
    </>
  );
};

export default Home;