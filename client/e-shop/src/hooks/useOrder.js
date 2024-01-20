
import { useEffect, useState } from "react";
import useHttpClient from "./useHttpClient";


const useOrder = () => {
    const [items, setItems] = useState([]);
    const [delivery, setDelivery] = useState();
    const [promoCodes, setPromoCodes] = useState();
    const [calculatedOrder, setCalculatedOrder] = useState();
    const {getData} = useHttpClient();

    useEffect(() => calculateOrder(), [items, delivery, promoCodes]);

    const createCalculationRequest = () => {
        return {
            items: items.map(i => ({productId: `${i.id}`, quantity: i.quantity})),
            delivery,
            promoCodes
        }
    }

    const calculateOrder = () => {
        getData('https://localhost:14443/Order/calculate', {
            method: "POST",
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(createCalculationRequest())
        })
        .then(r => r.json())       
        .then(r => setCalculatedOrder(r))
    };

    const getTotalAmount = () => {
        if(!calculatedOrder){
            return 0.0;
        }
        return calculatedOrder.order.orderCalculations.reduce((a, p) => a + p.amount, 0).toFixed(2);
    }

    const enumerateOrderItems = () => 
        calculatedOrder.order.items.map(i => ({
            product: calculatedOrder.productBatchInfos.find(p => `${p.id}` === i.productId),
            calculations: calculatedOrder.order.orderCalculations.filter(c => c.orderItemId === i.id),
            totalAmount: calculatedOrder.order.orderCalculations.filter(c => c.orderItemId === i.id).reduce((a, p) => a + p.amount, 0).toFixed(2)
        }));

    return {
        basket: {
            items,
            reload: () => {
                getData( 'https://localhost:13443/Basket')
                .then(r => r.json())
                .then(r => setItems(r))
            },
            add: (productId, qty = 1) => {
                getData(`https://localhost:13443/Basket/add?product=${productId}&qty=${qty}`, {method: 'POST'})
                .then(r => r.json())
                .then(r => setItems(r));
            },
            set: (productId, qty) => {
                getData(`https://localhost:13443/Basket/set?product=${productId}&qty=${qty}`, {method: 'POST'})
                .then(r => r.json())
                .then(r => setItems(r));
            },
            clear: () => {
                getData(`https://localhost:13443/Basket/clear`, {method: 'POST'})        
                .then(r => setItems([]));
            },
            getPosition: productId => items.find(p => p.id === productId)
        },
        delivery,
        setDelivery,
        promoCodes,
        setPromoCodes,
        calculatedOrder,
        calculateOrder,
        getTotalAmount,
        enumerateOrderItems
    }
}

export default useOrder