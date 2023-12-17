import useBasketContext from "../hooks/useContextBasket";


const Product = ({product}) => {
    const basket = useBasketContext();

    const handleAddToBasket = () => {
        basket.addToBasket(product.id, 1);
    }

    const handleRemoveToBasket = () => {
        basket.addToBasket(product.id, -1);
    }

    return(
        <div className="col-sm-12 col-md-6 col-lg-4 col-xl-3 col-xxl-2 p-3 product-item">
            <img src="food.jpg" className="product-image"/>
            <div className="product-item-description">{product.name}</div>
            <div>{product.popularity}</div>
            <div className="product-item-price">{product.price}</div>
            <div className="d-flex justify-content-between">
                <button className="basket-button" onClick={handleAddToBasket}>Add</button>
                <span className="m-2">{basket.getPosition(product.id)?.quantity}</span>
                <button className="basket-button" onClick={handleRemoveToBasket}>Remove</button>
            </div>
        </div>
    );
}

export default Product;