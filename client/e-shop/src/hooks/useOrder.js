
import { useEffect, useState } from "react";
import useHttpClient from "./useHttpClient";


const useOrder = (positions) => {
    const [order, setOrder] = useState();
    const {getData} = useHttpClient();

    useEffect(() => calculate(positions), [positions]);

    const createCalculationRequest = () => {
        return {
            items: positions.map(i => ({productId: `${i.id}`, quantity: i.quantity})),
            delivery:{
              address: "3 Normansfield Court, 22 Langdon Park, Teddington, TW11 9FE",
              contactName: "Aleksander",
              contactPhone: "+44321654987"
            }
          }
    }

    const calculate = () => {
        getData('https://localhost:14443/Order/calculate', {
            method: "POST",
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(createCalculationRequest())
        })
        .then(r => r.json())       
        .then(r => setOrder(r))
    };

    const getTotalAmount = () => {
        if(!order){
            return 0.0;
        }
        return order.order.orderCalculations.reduce((a, p) => a + p.amount, 0).toFixed(2);
    }

    return {
        order,
        calculate,
        getTotalAmount
    }
}

export default useOrder