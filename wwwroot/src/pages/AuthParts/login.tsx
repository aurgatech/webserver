import { useEffect, useState } from "react";
import axios from 'axios'; 
import Cookies from 'js-cookie';
import { MD5 } from 'crypto-js';
import { toast } from 'react-toastify';
import { config } from '../../global';
import Switch from 'react-switch';

const Login = ({ setAppState, setAuthState, target }: any) => {
    const [LoginState, setloginState] = useState(true);
    const [Email, setEmail] = useState("");
    const [Password, setPassword] = useState("");
    const [showCustomCloudServer, setShowCustomCloudServer] = useState(false);
    const [customCloudServer, setCustomCloudServer] = useState('');

    const verifyCustomCloudServer = () => {
        if(!config.IS_APP)return true;

        if (!showCustomCloudServer) {
            config.BASE_URL = 'https://account.aurga.com';
            config.URL_UPDATED = false;
            Cookies.set('custom_server_url', '');
            Cookies.set('use_custom_server', '0');
            return true;
        }

        var url = customCloudServer.trim().replace(/\/+$/, '');;
        const urlRegex = /^(https?:\/\/)([\da-zA-Z.-]+)(:\d{1,5})?([/\w .-]*)*\/?$/;

        if (urlRegex.test(url)) {
            config.BASE_URL = url;
            config.URL_UPDATED = true;
            config.CUSTOM_SERVER_URL = url;
            Cookies.set('custom_server_url', url);
            Cookies.set('use_custom_server', '1');
            return true;
        }

        toast.error('Invalid private server URL. Please try again!');
        return false;
    };
    
    const handleLogin = async () => {
        if(!verifyCustomCloudServer())return;

        setloginState(true); // Disable the button
      
        try {
            const emailHash = MD5(Email.trim().toLowerCase()).toString();
            const passwordHash = MD5(Password).toString();
            
            const response = await axios.post(config.BASE_URL + '/login', {
                email: emailHash,
                password: passwordHash,
            });
      
          if (response.data.status === 0) {
            Cookies.set('uid', response.data.uid);
            Cookies.set('token', response.data.token);
            Cookies.set('account', Email);
            Cookies.set('loginTime', new Date().toLocaleString());

            if (config.OS === 'osx') {
                var dic = {
                  func: "update_userinfo",
                  version: response.data.v,
                  uid: response.data.uid,
                  token: response.data.token,
                  payload: response.data.payload,
                };
                
                window.webkit.messageHandlers['js2objc'].postMessage(dic);
            }else{
                if (typeof window.cef_update_userinfo === 'function') {
                    window.cef_update_userinfo(response.data.v, response.data.uid, response.data.token, response.data.payload);
                }
            }

            setAuthState(true);
          }else if (response.data.status === -100) {
            toast.error('Login too fast. Please try again.');
          }else if (response.data.status === -107) {
            Cookies.set('activation_token', response.data.token);
            setAppState(target.activate);
          } else {
            Cookies.set('uid', '');
            Cookies.set('token', '');
            Cookies.set('account', '');
            toast.error('Invalid email or password. Please try again.');
          }
        } catch (error) {
            //console.error(error);
            toast.error('Could not connect to server. Please try again.');
        } finally {
            setloginState(false); // Enable the button
        }
      };

    useEffect(() => {
        const emailRegex = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/; // Regular expression for email validation

        if (Email && emailRegex.test(Email) && Password && Password.length >= 4) {
            setloginState(false);
        } else {
            setloginState(true);
        }
    }, [Email, Password]);

    useEffect(() => {
        setShowCustomCloudServer(config.USE_CUSTOM_SERVER);
        setCustomCloudServer(config.CUSTOM_SERVER_URL);
    }, []);
    return (
        <>
            <>
                <input
                    type="email"
                    value={Email}
                    autoComplete="username"
                    onChange={(e) => {
                        setEmail(e.target.value);
                    }}
                    placeholder="Email"
                    className="input input-bordered my-2 w-full "
                />
                <input
                    type="password"
                    value={Password}
                    autoComplete="current-password"
                    onChange={(e) => {
                        setPassword(e.target.value);
                    }}
                    placeholder="Password"
                    className="input input-bordered my-2 w-full "
                />
                
                <div className="flex justify-between items-center mb-4 w-full">
                    {config.IS_APP && (
                        <div className="flex justify-start">
                            <Switch
                                checked={showCustomCloudServer}
                                onChange={(checked) => setShowCustomCloudServer(checked)}
                                onColor="#86d3ff"
                                onHandleColor="#2693e6"
                                handleDiameter={20}
                                uncheckedIcon={false}
                                checkedIcon={false}
                                boxShadow="0px 1px 5px rgba(0, 0, 0, 0.6)"
                                activeBoxShadow="0px 0px 1px 10px rgba(0, 0, 0, 0.2)"
                                height={15}
                                width={35}
                            />
                        </div>
                    )}
                    <div
                        className="cursor-pointer text-[#1C2123] text-[16px] font-[400]"
                        onClick={() => {
                            if(!verifyCustomCloudServer())return;
                            setAppState(target.res);
                        }}
                    >
                        Forget Password
                    </div>
                </div>

                {showCustomCloudServer && (
                    <input
                        type="text"
                        value={customCloudServer}
                        onChange={(e) => setCustomCloudServer(e.target.value)}
                        placeholder="Private Cloud Server URL"
                        className="input input-bordered my-2 w-full "
                    />
                )}
                <div className="w-full flex justify-between items-center">
                    <button
                        className="rounded-[8px] border-[1px] py-1 border-[#BBBCBD] w-[45%] bg-white "
                        onClick={() => {
                            if(!verifyCustomCloudServer())return;

                            setAppState(target.reg);
                        }}
                    >
                        Register
                    </button>

                    <button
                        disabled={LoginState}
                        onClick={handleLogin}
                        className={`${
                            LoginState
                                ? "bg-white text-[#BBBCBD]  rounded-[8px] border-[1px] py-1 border-[#BBBCBD]  w-[45%]"
                                : "text-white bg-[#3283FC]  rounded-[8px] border-[1px] py-1 border-[#BBBCBD]  w-[45%]"
                        }   h-[32px]`}
                    >
                        Login
                    </button>
                </div>
            </>
        </>
    );
};

export default Login;
