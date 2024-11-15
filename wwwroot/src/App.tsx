import { AnimatePresence } from "framer-motion";
import Cookies from "js-cookie";
import { useEffect, useState, useRef } from "react";
import { Route, Routes, useLocation } from "react-router-dom";
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { config } from "./global";
import Wrapper from "./layout/index";
import Auth from "./pages/auth";
import SplashScreen from "./pages/splashScreen";
import { routes } from "./routes";
import axios from 'axios'; 
import useDeviceStore from "./zustand/devices.store";

const target = {
    reg: "register",
    log: "login",
    res: "resetPassword",
    rec: "recoverPassword",
    auth: "LoginauthCode",
    activate: "activate",
    confirm: "ConfirmEmail",
    success: "Success",
};

declare global {
    interface Window {
        onDeviceItemDiscovered: (json: string) => void;
        onAuthDevice: (auth: boolean) => void;
    }
}

const App = () => {
    const [showSplash, setShowSplash] = useState(config.IS_APP);
    const [authState, setAuthState] = useState(false);
    const [authTarget, setAuthTarget] = useState(target.reg);
    const location = useLocation();

    useEffect(() => {
        location.pathname = "/devices";
        const splashTimeout = setTimeout( async() => {
            setShowSplash(false);
        }, 3000);

        return () => clearTimeout(splashTimeout);
    }, []);

    return (
        <>
            <ToastContainer
                position="top-right"
                autoClose={5000}
                hideProgressBar={true}
                newestOnTop={false}
                closeOnClick
                rtl={false}
                pauseOnFocusLoss
                draggable
                pauseOnHover
                theme="light"
            />
            <div className="h-full">
                {showSplash ? (
                    <SplashScreen />
                ) : authState ? (
                    <AnimatePresence>
                        <Routes location={location} key={location.pathname}>
                            {routes.map((route, i) => (
                                <Route
                                    key={`route-${i}`}
                                    path={route.path}
                                    element={
                                        <Wrapper>
                                            <route.element
                                                key={`route-${i}`}
                                                setAuthState={setAuthState}
                                            />
                                        </Wrapper>
                                    }
                                />
                            ))}
                        </Routes>
                    </AnimatePresence>
                ) : (
                    <Auth setAuthState={setAuthState} authTarget={authTarget} target={target} />
                )}
            </div>
        </>
    );
};

export default App;
