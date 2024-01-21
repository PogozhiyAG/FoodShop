import { Link } from "react-router-dom";
import useAuth from "../../hooks/useAuth";
import PlusMinus from "../PlusMinus";
import useContextOrder from "../../hooks/useContextOrder";

const Basket = () => {
    const order = useContextOrder();
    const auth = useAuth();

    const getUserDisplayName = () => auth.state.userName ? 'Logged in' : 'Anonymous';

    const handleAddToBasket = (product) => {
        order.basket.add(product.id, 1);
    }

    const handleRemoveFromBasket = (product) => {
        order.basket.add(product.id, -1);
    }

    const handleRemoveProduct = (product) => {
        order.basket.set(product.id, 0);
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
                <h1 className="mb-5">Basket</h1>                
                

                {order.enumerateOrderItems().map((item, i) => {
                    const imageSrc = `food${item.product.id % 10}.jpg`;
                    
                    return( 
                        <section className="row border-bottom py-2" key={item.product.id}>
                            <div className="col-md-1">
                                <img src={imageSrc} className="basket-product-image"/>    
                            </div>
                            <div className="col-md-4">
                                <Link>{item.product.name}</Link>
                            </div>
                            <div className="col-md-1 text-end fs-5">
                                £{item.product.price}
                            </div>
                            <div className="col-md-2">
                                <PlusMinus 
                                    onMinus={() => handleRemoveFromBasket(item.product)} 
                                    onPlus={() => handleAddToBasket(item.product)}
                                    value={item.product.quantity}
                                />
                            </div>
                            <div className="col-md-1 text-end fs-5">
                                £{item.amount}
                            </div>
                            <div className="col-md-1 text-end fs-5">
                                £{item.saving}
                            </div>
                            <div className="col-md-1 text-end fs-5">
                                £{item.totalAmount}
                            </div>
                            <div className="col-md-1">
                                <button className="button-light" onClick={() => handleRemoveProduct(item.product)}>
                                    X
                                </button>
                            </div>
                        </section>
                    );
                })}


                <div className="row">
                    <div className="col-md-11 d-flex flex-row-reverse">
                        <span className="fs-2">£{order.getTotalAmount() }</span>
                    </div>                    
                </div> 
                <div className="row mt-3">
                    <div className="col-md-11 d-flex flex-row-reverse">
                        <button className="basket-button px-5">Checkout</button>
                    </div>                    
                </div>
            </main>
        </>
    );
};

export default Basket;


