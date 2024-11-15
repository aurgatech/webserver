import { useEffect, useState } from "react";
import { Bounce, toast } from "react-toastify";
import axios from 'axios'; 
import Cookies from 'js-cookie';
import { config } from '../../global';

const ForgotPassword = ({ setAppState, target }: any) => {
    const [forgotState, setforgotState] = useState(true);

    const [Email, setEmail] = useState("");

    const sendEmail = async () => {
        setforgotState(true); // Disable the button
      
        try {
          const response = await axios.post(config.BASE_URL + '/sendemail/resetpassword', {
            email: Email,
          });
      
          if (response.data.status === 0) {
            Cookies.set('reset_token', response.data.token);
            setAppState(target.rec);
          }else if (response.data.status === -100) {
            toast.error('Request too often! Please try again later.');
          }else if (response.data.status === -104) {
            Cookies.set('reset_token', '');
            toast.error('Invalid email. Please try again later.');
          }else {
            toast.error('Unknown error. Please try again.');
          }
        } catch (error) {
            //console.error(error);
            toast.error('Could not connect to server. Please try again.');
        } finally {
            setforgotState(false); // Enable the button
        }
    };

    //const [Password, setPassword] = useState("");
    useEffect(() => {
        const emailRegex = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/; // Regular expression for email validation

        if (Email && emailRegex.test(Email)) {
            setforgotState(false);
        } else {
            setforgotState(true);
        }
    }, [Email]);

    return (
        <>
            <>
                <input
                    type="email"
                    value={Email}
                    onChange={(e) => {
                        setEmail(e.target.value);
                    }}
                    placeholder="Email"
                    className="input input-bordered my-2 w-full "
                />
                <div className="w-full my-4 flex justify-between items-center">
                    <button
                        className="rounded-[8px] border-[1px] py-1 border-[#BBBCBD] w-[45%] bg-white "
                        onClick={() => {
                            if (config.IS_APP) {
                                setAppState(target.log);
                            }else{
                                setAppState(target.reg);
                            }
                        }}
                    >
                        Back
                    </button>

                    <button
                        disabled={forgotState}
                        className={`${
                            forgotState
                                ? "bg-white text-[#BBBCBD]  rounded-[8px] border-[1px] py-1 border-[#BBBCBD]  w-[45%]"
                                : "text-white bg-[#3283FC]  rounded-[8px] border-[1px] py-1 border-[#BBBCBD]  w-[45%]"
                        }   h-[32px]`}
                        onClick={() => {
                            sendEmail();
                            // setTimeout(() => {
                            //     setAppState(target.rec);
                            // }, 5000);
                            // toast.success(
                            //     <>
                            //         <h1 className="text-black text-[16px] font-normal	">
                            //             Reset Link Sent Successfully
                            //         </h1>
                            //         <p className="text-[#333739] text-[14px] font-normal">
                            //             Please check your email inbox.
                            //         </p>
                            //     </>,
                            //     {
                            //         position: "top-right",
                            //         autoClose: 5000,
                            //         hideProgressBar: true,
                            //         closeOnClick: true,
                            //         pauseOnHover: true,
                            //         draggable: true,
                            //         progress: undefined,
                            //         theme: "light",
                            //         transition: Bounce,
                            //     }
                            // );
                        }}
                    >
                        Send
                    </button>
                </div>
            </>
        </>
    );
};

export default ForgotPassword;
