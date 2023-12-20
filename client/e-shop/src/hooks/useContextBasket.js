import { useContext } from "react";
import { BasketContext } from "../context/BasketContext";

const useBasketContext = () => {
    const basket = useContext(BasketContext);
    return basket;
};

export default useBasketContext;