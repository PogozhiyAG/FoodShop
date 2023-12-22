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
                <h1 className="mb-5">Basket</h1>                
                

                {basket.positions.map(product => {
                    const imageSrc = `food${product.id % 10}.jpg`;
                    
                    return( 
                        <section className="row border-bottom py-2">
                            <div className="col-md-1">
                                <img src={imageSrc} className="basket-product-image"/>    
                            </div>
                            <div className="col-md-6">
                                <Link>{product.name}</Link>
                            </div>
                            <div className="col-md-1 text-end fs-5">
                                £{product.basePrice}
                            </div>
                            <div className="col-md-2">
                                <PlusMinus 
                                    onMinus={() => handleRemoveFromBasket(product)} 
                                    onPlus={() => handleAddToBasket(product)}
                                    value={product.quantity}
                                />
                            </div>
                            <div className="col-md-1 text-end fs-5">
                                £{product.offerAmount}
                            </div>
                            <div className="col-md-1">
                                <button className="button-light" onClick={() => handleRemoveProduct(product)}>
                                    X
                                </button>
                            </div>
                        </section>
                    );
                })}


                <div className="row">
                    <div className="col-md-11 d-flex flex-row-reverse">
                        <span className="fs-2">£{basket.getTotalAmount() }</span>
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


