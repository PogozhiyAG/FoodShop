
import { useEffect, useState } from "react";
import useHttpClient from "./useHttpClient";


const useOrder = ({customerProfile, basket}) => {    
    const [delivery, setDelivery] = useState();    
    const [calculatedOrder, setCalculatedOrder] = useState();

    const {getData} = useHttpClient();
    
    useEffect(() => { 
        calculateOrder();
    }, [basket.items, customerProfile.profile]);
    

    const createCalculationRequest = () => {
        return {
            items: basket?.items?.map(i => ({productId: `${i.id}`, quantity: i.quantity})),
            delivery: customerProfile?.profile?.delivery
        }
    }

    const verifyParameters = () => {
        return customerProfile.profile && basket.items;
    }

    const calculateOrder = () => {
        if(!verifyParameters()){
            setCalculatedOrder(null);
            return;
        }

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

    const enumerateOrderItems = () => {
        if(!calculatedOrder){
            return [];
        }

        return calculatedOrder.order.items.map(i => {
            const calculations  = calculatedOrder.order.orderCalculations.filter(c => c.orderItemId === i.id);
            const sum = (...typeCodes) => calculations.filter(c => typeCodes.length === 0 || typeCodes.includes(c.typeCode)).reduce((a, p) => a + p.amount, 0).toFixed(2);
            return {
                product: calculatedOrder.productBatchInfos.find(p => `${p.id}` === i.productId),
                calculations,
                totalAmount: sum(),
                amount: sum('P'),
                saving: sum('PD')
            }
        });
    }

    const getOrderSummary = () => {
        if(!calculatedOrder){
            return {};
        }
        return calculatedOrder.order.orderCalculations.reduce((a, i) => {a[i.typeCode] = i.amount + (a[i.typeCode] ? a[i.typeCode] : 0.0); return a;}, {});        
    }

    return {
        delivery,
        setDelivery,
        
        calculatedOrder,
        calculateOrder,

        getTotalAmount,
        enumerateOrderItems,
        getOrderSummary
    }
}

export default useOrder