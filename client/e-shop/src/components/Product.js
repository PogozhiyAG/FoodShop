const Product = ({product}) => {
    return(
        <div className="col-sm-12 col-md-6 col-lg-4 col-xl-3 col-xxl-2 p-3 product-item">
            <img src="food.jpg" className="product-image"/>
            <div className="product-item-description">{product.name}</div>
            <div>{product.popularity}</div>
            <div className="product-item-price">{product.price}</div>
            <div><button>Add</button></div>
        </div>
    );
}

export default Product;