import { useState } from "react";
import useHttpClient from "./useHttpClient";

const useBasket = () => {
    const [positions, setPositions] = useState([]);    
    const {getData} = useHttpClient();

    const reload = () => {
        getData( 'https://localhost:13443/Basket')
        .then(r => r.json())
        .then(r => setPositions(r))
    };

    const add = (productId, qty = 1) => {
        getData(`https://localhost:13443/Basket/add?product=${productId}&qty=${qty}`, {method: 'POST'})
        .then(r => r.json())
        .then(r => setPositions(r));
    };

    const set = (productId, qty) => {
        getData(`https://localhost:13443/Basket/set?product=${productId}&qty=${qty}`, {method: 'POST'})
        .then(r => r.json())
        .then(r => setPositions(r));
    };

    const clear = () => {
        getData(`https://localhost:13443/Basket/clear`, {method: 'POST'})        
        .then(r => setPositions([]));
    };

    const getTotalAmount = () => positions.reduce((a, p) => a + p.offerAmount, 0).toFixed(2);

    const getPosition = (productId) => {
        return positions.find(p => p.id == productId);
    };
    
    return {
        positions,
        add,
        set,
        reload,
        clear,
        getTotalAmount,
        getPosition
    }
}

export default useBasket;