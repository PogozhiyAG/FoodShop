import { useSyncExternalStore } from "react";
import AuthState from "../services/authState";

const authState = new AuthState();

const useAuth = () => {
    const authSync = useSyncExternalStore(authState.subscribe, authState.getSnapshot);
    return [authState, authSync];
}

export default useAuth;