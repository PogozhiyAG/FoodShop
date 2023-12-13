import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import Product from "./components/Product";
import useHttpClient from "./hooks/useHttpClient";

const Basket = () => {
  const [products, setProducts] = useState([]);
  const {getData} = useHttpClient();
  
  const getDataUrl = () => {
    let url = 'https://localhost:13443/Basket';
    return url;
  }

  useEffect(() => {
    getData(getDataUrl())
    .then(r => r.json())
    .then(r => setProducts(r))
  }, []);


    return (
        <>
            <h1>Basket</h1>
            <Link to="/">Home</Link>
            <section className="row section">
                {products.map(product => {
                    return (
                        <div className="col-sm-12 col-md-6 col-lg-4 col-xl-3 col-xxl-2 product-item">
                            <img src="food.jpg" className="product-image"/>
                            <div className="product-item-description">{product.name}</div>
                            
                            <div className="product-item-price">{product.basePrice}</div>
                            <div className="product-item-qty">{product.quantity}</div>
                            <div className="product-item-offer-amount">{product.offerAmount} ({product.baseAmount})</div>                            
                        </div>
                    );
                })}
                
            </section>
        </>
    );
};

export default Basket;