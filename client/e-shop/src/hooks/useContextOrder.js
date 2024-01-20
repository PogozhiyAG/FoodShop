import { useContext } from "react";
import { BasketContext } from "../context/BasketContext";

const useContextOrder = () => useContext(BasketContext);

export default useContextOrder;