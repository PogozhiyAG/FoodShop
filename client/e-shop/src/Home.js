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
     
      <nav className="navbar navbar-expand-lg fixed-top bg-light">
        <div className="container-fluid">
          <img src="logo.png" style={{width: '50px', height: '50px'}}/>
          <ul className="navbar-nav me-auto mb-2 mb-lg-0">          
            <li className="nav-item">
              <Link to="/category" className="nav-link">Category</Link>
            </li>
            <li className="nav-item">
              <Link to="/product" className="nav-link">Product</Link>
            </li>
            <li className="nav-item">
              <Link to="/basket" className="nav-link">Basket</Link>
            </li>
          </ul>
        </div>
      </nav>
      <div className="container">
        <div className="row">
        {products.map(i => <div className="col-md-6 col-lg-4 col-xl-3 p-3">{i.name}</div>)}
        </div>
      </div>
    </>
  );
};

export default Home;