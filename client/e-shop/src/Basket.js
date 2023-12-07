import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import Product from "./components/Product";

const Basket = () => {
  const [products, setProducts] = useState([]);
  
  const getDataUrl = () => {
    let url = 'https://localhost:13443/Basket';
    return url;
  }

  useEffect(() => {
    fetch(getDataUrl(), {
      headers: {
        Authorization: 'Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNyc2Etc2hhMjU2IiwidHlwIjoiSldUIn0.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYjA4Zjg0MWUtNzA3Zi00NTZlLWIwNzMtZWExZTI0YTQ5ZDkzIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvYW5vbnltb3VzIjoiYjA4Zjg0MWUtNzA3Zi00NTZlLWIwNzMtZWExZTI0YTQ5ZDkzIiwiZXhwIjo0ODU3NjQ3MjQ1LCJpc3MiOiJGb29kU2hvcC5BcGkuQXV0aCIsImF1ZCI6IkZvb2RTaG9wIn0.I3M0C66KqJL-UPjkZ8z_4rvXjKpTad3UhpHfRfUKQLakPIFjAa9zO-Ek-HxAIjPySCewuaQ2xZKwsq_b4DYrmPvT-8ncCmyjSbXzq7dHCi_7sA07UFUIhzVnN7-7CCYBauegPS63cRZpzyCAorxf6BWCn7W4Mu_jbuS71zmzqRs'
      }
    })
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
                            <div>{product.popularity}</div>
                            <div className="product-item-price">{product.price}</div>
                            <div className="product-item-qty">{product.quantity}</div>
                            <div className="product-item-offer-amount">{product.offerAmount}</div>                            
                        </div>
                    );
                })}
                
            </section>
        </>
    );
};

export default Basket;