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
    .then(r => {
        setProducts(r)
    })
  }, []);


    return (
        <>
            <h1>Basket</h1>
            
            <section className="row section"> 
                <div className="d-flex">
                    <Link to="/">Home</Link>
                    <span className="h2">{products.reduce((a, p) => a + p.offerAmount, 0).toFixed(2) }</span>
                </div>               
                <table>
                    <tbody>
                        {products.map((product, i) => {
                            return (
                                <tr key={i}>
                                    <td><img src="food.jpg" className="basket-product-image"/></td>
                                    <td>{product.name}</td>
                                    <td>{product.basePrice}</td>
                                    <td>{product.quantity}</td>
                                    <td>{product.offerAmount} ({product.baseAmount})</td>
                                </tr>                       
                            );
                        })}
                    </tbody>
                </table>
            </section>
        </>
    );
};

export default Basket;