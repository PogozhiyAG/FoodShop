const PlusMinus = (props) => {

    return (
        <div className="d-flex justify-content-between fs-5 fw-500">                            
            <button className="basket-button" onClick={props.onPlus}>-</button>
            <span className="mx-2 pt-1">{props.value}</span>
            <button className="basket-button" onClick={props.onMinus}>+</button>
        </div>
    );
}

export default PlusMinus;