import { useContext } from "react";
import FoodShopContext from "../context/FoodShopContext";

const useBasketContext = () => {
    const {basket} = useContext(FoodShopContext);
    return basket;
};

export default useBasketContext;