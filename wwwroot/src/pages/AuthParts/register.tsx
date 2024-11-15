import { useEffect, useState } from "react";
import axios from 'axios'; 
import { toast } from 'react-toastify';
import Cookies from 'js-cookie';
import { config } from '../../global';

const Register = ({ setAppState, target }: any) => {
    const [RegState, setRegState] = useState(true);
    const [Email, setEmail] = useState("");
    const [Password, setPassword] = useState("");
    const [RePassword, setRePassword] = useState("");

    const register = async () => {
        setRegState(true); // Disable the button
      
        try {
          const response = await axios.post(config.BASE_URL + '/register/user', {
            email: Email,
            password: Password,
          });
      
          switch (response.data.status) {
            case 0:
                Cookies.set('activation_token', response.data.token);
                setAppState(target.activate);
                break;
            case -100:
                toast.error('Submit too often. Please try again later.');
                break;
            case -101:
            case -102:
                toast.error('Invalid email or password. Please try again.');
                break;
            case -103:
                toast.error('Email already exists. Please try again.');
                break;
            default:
                toast.error('Could not connect to server. Please try again.');
                break;
          }
          setRegState(false);
        } catch (error) {
            toast.error('Could not connect to server. Please try again.');
            //console.error(error);
        } finally {
            setRegState(false); // Enable the button
        }
      };


    useEffect(() => {
        const emailRegex = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/; // Regular expression for email validation

        if (Email && emailRegex.test(Email) && Password && Password.length >= 4 && Password === RePassword) {
            setRegState(false);
        } else {
            setRegState(true);
        }
    }, [Email, Password, RePassword]);
    return (
        <>
            <>
                <input
                    value={Email}
                    onChange={(e) => {
                        setEmail(e.target.value);
                    }}
                    type="email"
                    placeholder="Enter Email"
                    className="input input-bordered my-2 w-full "
                />
                <input
                    value={Password}
                    onChange={(e) => {
                        setPassword(e.target.value);
                    }}
                    type="password"
                    placeholder="Enter Password"
                    className="input input-bordered my-2 w-full "
                />
                <input
                    value={RePassword}
                    onChange={(e) => {
                        setRePassword(e.target.value);
                    }}
                    type="password"
                    placeholder="Re-enter Password"
                    className="input input-bordered my-2 w-full "
                />
                <div className="w-full flex justify-between items-center my-2">
                    { config.IS_APP ? (
                        <>
                            <button
                                className="rounded-[8px] border-[1px] py-1 border-[#BBBCBD] w-[45%] bg-white "
                                onClick={() => {
                                    setAppState(target.log);
                                }}
                            >
                                Login
                            </button>

                            <button
                                disabled={RegState}
                                onClick={register}
                                className={`${
                                    RegState
                                        ? "bg-white text-[#BBBCBD]  rounded-[8px] border-[1px] py-1 border-[#BBBCBD]  w-[45%]"
                                        : "text-white bg-[#3283FC]  rounded-[8px] border-[1px] py-1 border-[#BBBCBD]  w-[45%]"
                                }   h-[32px]`}
                            >
                                Register
                            </button>
                        </>
                    ) : (
                        <>
                            <button
                                disabled={RegState}
                                onClick={register}
                                className={`${
                                    RegState
                                        ? "bg-white text-[#BBBCBD]  rounded-[8px] border-[1px] py-1 border-[#BBBCBD]  w-[45%]"
                                        : "text-white bg-[#3283FC]  rounded-[8px] border-[1px] py-1 border-[#BBBCBD]  w-[45%]"
                                }   h-[32px]`}
                            >
                                Register
                            </button>
                            <button
                                className="rounded-[8px] border-[1px] py-1 border-[#BBBCBD] w-[45%] bg-white "
                                onClick={() => {
                                    setAppState(target.res);
                                }}
                            >
                                Forget Password
                            </button>                            
                        </>
                    )}
                </div>
            </>
        </>
    );
};

export default Register;
