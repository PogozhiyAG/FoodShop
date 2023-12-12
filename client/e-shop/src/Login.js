import { useState } from "react";
import { Link } from "react-router-dom";
import useAuth from "./hooks/useAuth";

const Login = () => {
    const auth = useAuth();
    const [userName, setUserName] = useState();
    const [password, setPassword] = useState();

    const handleSubmit = async (e) => {
        e.preventDefault();

        fetch('https://localhost:11443/Authentication/login', {
            method: "POST",
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({userName, password})
        })
        .then(r => {
            if(r.ok){
                return r.json();
            }
            return new Promise.reject(new Error('Smt went wrong'));
        })
        .then(r => {            
            auth.setSignIn(r.token, r.refreshToken);
        })
        .catch(e => console.log(e));
    };

    return (
        <>
            <h1>Category</h1>
            <Link to="/">Home</Link>

            <form onSubmit={handleSubmit}>
                <input value={userName} onChange={(e) => setUserName(e.target.value)}/>
                <input value={password} onChange={(e) => setPassword(e.target.value)}/>
                <button>Sign in</button>
            </form>

            <div>{JSON.stringify(auth)}</div>
        </>
    );
};

export default Login;