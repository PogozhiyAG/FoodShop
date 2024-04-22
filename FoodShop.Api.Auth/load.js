import { check } from "k6";
import http from "k6/http";

export default function () {
    let data = {
        userName: "string",
        password: "String@1"
    };

    let res = http.post(
        "https://localhost:11443/Authentication/login",
        JSON.stringify(data),
        {
            headers: { 'Content-Type': 'application/json' },
        }
    );

    check(res, {
        "is status 200": (r) => r.status === 200
    });
};