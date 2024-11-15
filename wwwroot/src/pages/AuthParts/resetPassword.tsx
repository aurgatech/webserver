import { useEffect, useState } from "react";
import { Bounce, toast } from "react-toastify";
import axios from 'axios'; 
import Cookies from 'js-cookie';
import { config } from '../../global';

const ResetPassword = ({ setAppState, target }: any) => {
    const [SubmitDisabled, setSubmitDisabled] = useState(true);
    const [PasswordUpdated, setPasswordUpdated] = useState(false);

    const [Password, setPassword] = useState("");
    const [RePassword, setRePassword] = useState("");
    const [VerificationCode, setVerificationCode] = useState("");

    const resetPassword = async () => {
        setSubmitDisabled(true); // Disable the button
      
        try {
            var token = Cookies.get('reset_token');
          const response = await axios.post(config.BASE_URL + '/verify/resetpassword', {
            token: token,
            verificationcode: VerificationCode,
            newpassword: Password,
          });
      
          if (response.data.status === 0) {
            setPasswordUpdated(true);

            setAppState(target.success);

          }else if (response.data.status === -100) {
            toast.error('Request too often! Please try again later.');
          }else if (response.data.status === -104) {
            toast.error('The code is wrong! Please try again.');
          }else if (response.data.status === -106) {
            Cookies.set('reset_token', response.data.token);
            toast.error('The code is expired! Please check your email for the latest code.');
          }else {
            toast.error('Unknown error. Please try again.');
          }
        } catch (error) {
            //console.error(error);
            toast.error('Could not connect to server. Please try again.');
        } finally {
            setSubmitDisabled(false); // Enable the button
        }
    };

    useEffect(() => {
        const codeRegex = /^\d{6}$/;

        if (Password && RePassword && Password.length > 4 && VerificationCode && codeRegex.test(VerificationCode) 
            && Password === RePassword){
                setSubmitDisabled(false);
        } else {
            setSubmitDisabled(true);
        }
    }, [RePassword, Password, VerificationCode]);
    return (
        <>
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

            <input
                value={VerificationCode}
                onChange={(e) => {
                    setVerificationCode(e.target.value);
                }}
                type="text"
                placeholder="6 Digits Verification Code"
                className="input input-bordered my-2 w-full "
            />
            <div className="w-full my-4 flex justify-between items-center">
                <button
                    className="rounded-[8px] border-[1px] py-1 border-[#BBBCBD] w-[45%] bg-white "
                    onClick={() => {
                        if(PasswordUpdated){
                            if(config.IS_APP){
                                setAppState(target.log);
                            }else{
                                setAppState(target.reg);
                            }
                        }else{
                            setAppState(target.res);
                        }
                    }}
                >
                    Cancel
                </button>

                <button
                    disabled={SubmitDisabled}
                    className={`${
                        SubmitDisabled
                            ? "bg-white text-[#BBBCBD]  rounded-[8px] border-[1px] py-1 border-[#BBBCBD]  w-[45%]"
                            : "text-white bg-[#3283FC]  rounded-[8px] border-[1px] py-1 border-[#BBBCBD]  w-[45%]"
                    }   h-[32px]`}
                    onClick={resetPassword}
                >
                    Reset Password
                </button>
            </div>
        </>
    );
};

export default ResetPassword;
