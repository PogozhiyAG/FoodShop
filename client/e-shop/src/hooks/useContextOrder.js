import { useContext } from "react";
import { BasketContext } from "../context/BasketContext";

const useOrderContext = () => {
    const {order} = useContext(BasketContext);
    return order;
};

export default useOrderContext;