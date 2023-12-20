import { Link } from "react-router-dom";
import useBasketContext from "../hooks/useContextBasket";


const Product = ({product}) => {
    const basket = useBasketContext();

    const handleAddToBasket = () => {
        basket.addToBasket(product.id, 1);
    }

    const handleRemoveToBasket = () => {
        basket.addToBasket(product.id, -1);
    }

    const basketQuantity = basket.getPosition(product.id)?.quantity;    

    return(
        <div className="col-xxl-2 col-xl-3 col-lg-4 col-md-6 col-sm-12 product-item-outer">            
            <div className="product-item-inner">
                <div className="product-item-header">

                </div>
                <img src="food.jpg" className="product-image"/>
                <div className="product-item-info p-3">
                    <div className="product-item-description fw-500">
                        <Link>{product.name}</Link>
                    </div>
                    
                    <div className="product-item-price fs-4 mb-2">£{product.price.toFixed(2)}</div>
                    
                    {
                        basketQuantity 
                            ? (
                                <div className="d-flex justify-content-between">                            
                                    <button className="basket-button" onClick={handleRemoveToBasket}>-</button>
                                    <span className="m-2">{basketQuantity}</span>
                                    <button className="basket-button" onClick={handleAddToBasket}>+</button>
                                </div>
                            )
                            : (
                                <div className="d-flex justify-content-between">
                                    <button className="basket-button w-100" onClick={handleAddToBasket}>Add</button>
                                </div>
                            )
                    }
                </div>
                
            </div>
        </div>
    );
}

export default Product;