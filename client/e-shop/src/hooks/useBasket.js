import { useState } from "react";
import useHttpClient from "./useHttpClient";

export const useBasket = () => {
    const [items, setItems] = useState();

    const {getData} = useHttpClient();

    const fetchData = () => 
        getData( 'https://localhost:13443/Basket')
        .then(r => r.json());
    
    const reload = () =>
        fetchData()
        .then(r => setItems(r));
    
    const add = (productId, qty = 1) =>
        getData(`https://localhost:13443/Basket/add?product=${productId}&qty=${qty}`, {method: 'POST'})
        .then(r => r.json())
        .then(r => setItems(r));

    const set = (productId, qty) =>
        getData(`https://localhost:13443/Basket/set?product=${productId}&qty=${qty}`, {method: 'POST'})
        .then(r => r.json())
        .then(r => setItems(r));

    const clear = () =>
        getData(`https://localhost:13443/Basket/clear`, {method: 'POST'})        
        .then(r => setItems([]));

    const getPosition = productId => items?.find(p => p.id === productId);

    return {
        items,
        setItems,
        fetchData,
        reload,
        add,
        set,
        clear,
        getPosition
    };
}