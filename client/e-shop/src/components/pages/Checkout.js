import { loadStripe } from "@stripe/stripe-js";
import { useContext, useEffect, useState } from "react";
import { BasketContext } from "../../context/BasketContext";
import useHttpClient from "../../hooks/useHttpClient";
import { Elements } from "@stripe/react-stripe-js";
import CheckoutForm from "../CheckoutForm";
import { useThrottling } from "../../hooks/useThrottling";


const stripePromise = loadStripe('pk_test_51Nw5GxGp5S1pkuIE0PbjDl0bFDJyYq5ec3ygYhFvWTMF27T3FFRSH5N6TmxbuSJA6dTwWCTLMqUSfDd9Z6ws3CR500BTdVkMTC');


export const Checkout = () => {
    const [clientSecret, setClientSecret] = useState("");    
    const {order} = useContext(BasketContext);

    const {getData} = useHttpClient();

    const throttling = useThrottling({key: 'create-payment-intent', delay: 200});

    //TODO: Auth
    useEffect(() => {
        throttling(() => {
            getData("https://localhost:14443/PaymentIntent", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(order.createCalculationRequest()),
            })
            .then(r => r.json())
            .then(r => setClientSecret(r.clientSecret));

        });        
    }, []);

    const appearance = {
        theme: 'stripe',
    };
    
    const options = {
        clientSecret,
        appearance,
    };

    return (
        <>
            <h1>Checkout</h1>
            {clientSecret 
                ? (
                    <Elements options={options} stripe={stripePromise}>
                      <CheckoutForm clientSecret={clientSecret}  />
                    </Elements>
                  )
                : <div>Preparing the checkout..</div>
            }
            
        </>
    );
}