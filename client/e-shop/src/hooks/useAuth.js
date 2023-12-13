import AuthState from "../services/authState";

const authState = new AuthState()

const useAuth = () => authState;

export default useAuth;