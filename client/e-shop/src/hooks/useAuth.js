import { useContext } from "react";
import FoodShopContext from "../context/FoodShopContext";

const useAuth = () => useContext(FoodShopContext);

export default useAuth;