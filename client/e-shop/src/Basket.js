import { Link } from "react-router-dom";
import useBasketContext from "./hooks/useContextBasket";
import useAuth from "./hooks/useAuth";
import PlusMinus from "./components/PlusMinus";

const Basket = () => {
    const basket = useBasketContext();
    const auth = useAuth();

    const getUserDisplayName = () => auth.state.userName ? 'Logged in' : 'Anonymous';

    const handleAddToBasket = (product) => {
        basket.add(product.id, 1);
    }

    const handleRemoveFromBasket = (product) => {
        basket.add(product.id, -1);
    }

    const handleRemoveProduct = (product) => {
        basket.set(product.id, 0);
    }
    
    return (
        <>
            <header className="header">
                <img src="logo.png" style={{width: '50px', height: '50px'}}/>
                <Link className="p-2" to="/">Home</Link>
                <Link className="p-2" to="/login">Login</Link> 
                <span className="p-2">{getUserDisplayName()}</span>               
            </header>
            <main className="container">
                <h1>Basket</h1>
                
                <section className="row section"> 
                    <div className="d-flex">                        
                        <span className="h2">£{basket.getTotalAmount() }</span>
                    </div>               
                    <table>
                        <tbody>
                            {basket.positions?.map((product, i) => {
                                const imageSrc = `food${product.id % 10}.jpg`;

                                return (
                                    <tr key={i} className="basket-product-row">
                                        <td><img src={imageSrc} className="basket-product-image"/></td>
                                        <td >{product.name}</td>                                        
                                        <td>{product.basePrice}</td>
                                        <td >
                                            <PlusMinus 
                                                onPlus={() => handleRemoveFromBasket(product)} 
                                                onMinus={() => handleAddToBasket(product)}
                                                value={product.quantity}
                                            />
                                        </td>                                        
                                        <td className="text-end fs-5 fw-500">£{product.offerAmount}</td>
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


