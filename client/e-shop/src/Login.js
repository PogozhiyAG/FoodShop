import { useState } from "react";
import { Link } from "react-router-dom";
import useAuth, { authState } from "./hooks/useAuth";


const Login = () => {    
    const [userName, setUserName] = useState();
    const [password, setPassword] = useState();
    const authSync = useAuth();

    const handleSubmit = async (e) => {
        e.preventDefault();

        await fetch('https://localhost:11443/Authentication/login', {
            method: "POST",
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({userName, password})
        })
        .then(async r => {
            if(r.ok){
                const j = await r.json();                
                authState.signIn(j.token, j.refreshToken, j.userName);
                return;
            } else if(r.status == 401){
                return Promise.reject(new Error('Login or password is incorrect'));    
            }
            return Promise.reject(new Error('Smth went wrong'));
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

            <hr/>
            <div>{JSON.stringify(authSync)}</div>           

            <hr/>
            <button onClick={() => authState.signOut()}>Sign out</button>
        </>
    );
};

export default Login;