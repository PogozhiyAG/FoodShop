import { useSyncExternalStore } from "react";
import AuthState from "../services/authState";

export const authState = new AuthState();

const useAuth = () => {
    const authSync = useSyncExternalStore(authState.subscribe, authState.getSnapshot);
    return authSync;
}

export default useAuth;