const ThrottlingTimeouts = {};

export const useThrottling = ({key, delay}) => {
    return func => {
        clearTimeout(ThrottlingTimeouts[key]);
        return ThrottlingTimeouts[key] = setTimeout(func, delay);
    }
}