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
    const productItemStyle = `${basketQuantity ? 'basket-added-product' : ''} col-sm-12 col-md-6 col-lg-4 col-xl-3 col-xxl-2 p-3 product-item`;

    return(
        <div className={productItemStyle}>
            <img src="food.jpg" className="product-image"/>
            <div className="product-item-description">{product.name}</div>
            <div>{product.popularity}</div>
            <div className="product-item-price">{product.price}</div>
            
            {
                basketQuantity 
                    ? (
                        <div className="d-flex justify-content-between">
                            <button className="basket-button" onClick={handleAddToBasket}>Add</button>
                            <span className="m-2">{basketQuantity}</span>
                            <button className="basket-button" onClick={handleRemoveToBasket}>Remove</button>
                        </div>
                    )
                    : (
                        <div className="d-flex justify-content-between">
                            <button className="basket-button w-100" onClick={handleAddToBasket}>Add</button>
                        </div>
                    )
            }
        </div>
    );
}

export default Product;