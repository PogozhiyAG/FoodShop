import { Link } from "react-router-dom";
import useBasketContext from "./hooks/useContextBasket";

const Basket = () => {
    const basket = useBasketContext();

    return (
        <>
            <main className="container">
                <h1>Basket</h1>
                
                <section className="row section"> 
                    <div className="d-flex">
                        <Link to="/">Home</Link>
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
                                        <td>{product.quantity}</td>
                                        <td>{product.offerAmount} ({product.baseAmount})</td>
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