import { useContext, useEffect } from "react";
import { BasketContext } from "../../context/BasketContext";
import { useNavigate } from "react-router-dom";

export const CheckoutComplete = () => {
    const {basket} = useContext(BasketContext);
    const navigate = useNavigate();
    
    useEffect(() => {
        basket
            .clear()            
            .then(navigate('/'));
    }, []);    

    return(
        <h1>Thank you for shopping!</h1>
    );
}