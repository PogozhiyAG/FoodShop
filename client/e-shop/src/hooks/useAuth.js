import { useContext } from "react";
import FoodShopContext from "../context/FoodShopContext";
import AuthState from "../api/authState";

const authState = new AuthState()

//const useAuth = () => useContext(FoodShopContext);
const useAuth = () => authState;

export default useAuth;