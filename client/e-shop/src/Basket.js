import { Link } from "react-router-dom";
import useBasketContext from "./hooks/useContextBasket";

const Basket = () => {
    const basket = useBasketContext();

    const handleAddToBasket = (product) => {
        basket.add(product.id, 1);
    }

    const handleRemoveFromBasket = (product) => {
        basket.add(product.id, -1);
    }

    const handleRemoveProduct = (product) => {
        basket.set(product.id, 0);
    }

    const handleClear = () => {
        basket.clear();
    }

    return (
        <>
            <header className="header">
                <img src="logo.png" style={{width: '50px', height: '50px'}}/>
                <Link className="p-2" to="/">Home</Link>
                <Link className="p-2" to="/login">Login</Link>                
            </header>
            <main className="container">
                <h1>Basket</h1>
                
                <section className="row section"> 
                    <div className="d-flex">                        
                        <span className="h2">{basket.getTotalAmount() }</span>
                    </div>               
                    <table>
                        <tbody>
                            {basket.positions?.map((product, i) => {
                                const imageSrc = `food${product.id % 10}.jpg`;

                                return (
                                    <tr key={i} className="basket-product-row">
                                        <td><img src={imageSrc} className="basket-product-image"/></td>
                                        <td>{product.name}</td>                                        
                                        <td>{product.basePrice}</td>
                                        <td>
                                            <button  onClick={() => handleRemoveFromBasket(product)}>-</button>
                                            <span>{product.quantity}</span>
                                            <button  onClick={() => handleAddToBasket(product)}>+</button>
                                        </td>
                                        <td>{product.offerAmount} ({product.baseAmount})</td>
                                        <td><button onClick={() => handleRemoveProduct(product)}>X</button></td>
                                    </tr>                       
                                );
                            })}
                        </tbody>
                    </table>
                </section>
            </main>
        </>
    );
};

export default Basket;


