import { useContext } from "react";
import { Link, useNavigate } from "react-router-dom";
import useAuth from "../../hooks/useAuth";
import PlusMinus from "../PlusMinus";
import { ORDER_CALCULATION_TYPE_CODES } from "../../services/data";
import { BasketContext } from "../../context/BasketContext";

const Basket = () => {
    const {basket, order, customerProfile} = useContext(BasketContext);
    const auth = useAuth();
    const navigate = useNavigate();
    

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

    const handleCheckoutClick = () => {
        navigate('/checkout');
    }

    let orderSummary = order.getOrderSummary();

    return (
        <>
            <header className="header">
                <img src="logo.png" style={{width: '50px', height: '50px'}}/>
                <Link className="p-2" to="/">Home</Link>
                <Link className="p-2" to="/login">Login</Link> 
                <span className="p-2">{getUserDisplayName()}</span>               
            </header>
            <main className="container mb-5">
                <h1 className="mb-5">Basket</h1>                
                

                {order.enumerateOrderItems().map((item, i) => {
                    const imageSrc = `food${item.product.id % 20}.jpg`;
                    
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


                {Object.keys(orderSummary).map(k => {
                    return (
                        <div className="row" key={k}>
                            <div className="col-md-9 d-flex flex-row-reverse">                        
                                <span className="fs-4">{ORDER_CALCULATION_TYPE_CODES[k]?.name ?? k }:</span>
                            </div> 
                            <div className="col-md-2 d-flex flex-row-reverse">
                                <span className="fs-4">£{orderSummary[k].toFixed(2) }</span>
                            </div>                    
                        </div> 
                    )
                })}

                <div className="row mt-5">
                    <div className="col-md-9 d-flex flex-row-reverse">                        
                        <span className="fs-2">Total:</span>
                    </div> 
                    <div className="col-md-2 d-flex flex-row-reverse">
                        <span className="fs-2">£{order.getTotalAmount() }</span>
                    </div>                    
                </div> 

                {customerProfile.profile?.delivery && (
                    <div className="row mt-5">
                        <div className="col-md-6">
                            {customerProfile.profile.delivery.address}
                        </div>
                    </div>
                )}                

                <div className="row mt-3">
                    <div className="col-md-11 d-flex flex-row-reverse">
                        <button onClick={handleCheckoutClick} className="basket-button px-5">Checkout</button>
                    </div>                    
                </div>
            </main>
        </>
    );
};

export default Basket;


