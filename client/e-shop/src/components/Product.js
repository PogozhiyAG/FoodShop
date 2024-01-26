import { Link } from "react-router-dom";
import PlusMinus from "./PlusMinus";
import { useContext } from "react";
import {BasketContext} from '../context/BasketContext'


const Product = ({product}) => {    
    const {basket} = useContext(BasketContext);
    

    const handleAddToBasket = () => {
        basket.add(product.id, 1);
    }

    const handleRemoveFromBasket = () => {
        basket.add(product.id, -1);
    }

    const basketQuantity = basket.getPosition(product.id);  
    
    const imageSrc = `food${product.id % 10}.jpg`;

    return(
        <div className="col-xxl-2 col-xl-3 col-lg-4 col-md-6 col-sm-12 product-item-outer">            
            <div className="product-item-inner">
                <div className="product-item-header">

                </div>
                <Link>
                    <img src={imageSrc} className="product-image"/>
                </Link>
                <div className="product-item-info p-3">
                    <div className="product-item-description fw-500">
                        <Link>{product.name}</Link>
                    </div>
                    
                    <div className="product-item-price fs-4 mb-2 fw-500">£{product.price.toFixed(2)}</div>
                    
                    {
                        basketQuantity 
                            ? (
                                <PlusMinus 
                                    onMinus={handleRemoveFromBasket} 
                                    onPlus={handleAddToBasket}
                                    value={basketQuantity}
                                />
                            )
                            : (
                                <div className="d-flex justify-content-between fs-5 fw-500">
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