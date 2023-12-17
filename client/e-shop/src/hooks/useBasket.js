import { useState } from "react";
import useHttpClient from "./useHttpClient";

const useBasket = () => {
    const [positions, setPositions] = useState([]);    
    const {getData} = useHttpClient();

    const reloadBasket = () => {
        getData( 'https://localhost:13443/Basket')
        .then(r => r.json())
        .then(r => {
            setPositions(r)
        })
    };

    const addToBasket = (productId, qty = 1) => {
        getData(`https://localhost:13443/Basket/add?product=${productId}&qty=${qty}`, {method: 'POST'})
        .then(r => r.json())
        .then(r => setPositions(r));
    };

    const getTotalAmount = () => positions.reduce((a, p) => a + p.offerAmount, 0).toFixed(2);

    const getPosition = (productId) => {
        return positions.find(p => p.id == productId);
    };

    return {
        positions,
        addToBasket,
        reloadBasket,
        getTotalAmount,
        getPosition
    }
}

export default useBasket;