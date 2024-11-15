import { useEffect, useState } from "react";
import axios from 'axios'; 
import Cookies from 'js-cookie';
import { toast } from 'react-toastify';
import { config } from '../../global';

const Activation = ({setAppState, target}: any) => {
    const [Code, setCode] = useState("");
    const [SubmitDisallowed, setSubmitDisallowed] = useState(true);

    const submitCode = async () => {
        setSubmitDisallowed(true); // Disable the button
      
        try {
            var token = Cookies.get('activation_token');
            const response = await axios.post(config.BASE_URL + '/verify/activation', {
                token: token,
                verificationcode: Code,
            });
        
            if (response.data.status === 0) {
                setAppState(target.success);
                setSubmitDisallowed(true);
            }else if (response.data.status === -100) {
                toast.error('Submit too often. Please try again later.');
            }else if (response.data.status === -104) {
                toast.error('The code is wrong! Please try again.');
            } else if (response.data.status === -106) {
                toast.error('The code is expired! Please check your email for the latest code.');
                Cookies.set('activation_token', response.data.token);
            } else {
                toast.error('Unknown error. Please try again.');
            }
        } catch (error) {
            //console.error(error);
            toast.error('Could not connect to server. Please try again.');
        } finally {
            setSubmitDisallowed(false); // Enable the button
        }
      };

    useEffect(() => {
        const codeRegex = /^\d{6}$/;
        
        if (Code && codeRegex.test(Code)) {
            setSubmitDisallowed(false);
        } else {
            setSubmitDisallowed(true);
        }
    }, [Code]);

    return (
        <>
         <input
            type="text"
            value={Code}
            onChange={(e) => {
                setCode(e.target.value);
            }}
            placeholder="6 Digit Verification Code"
            className="input input-bordered my-2 w-full "
        />

        <button
            disabled={SubmitDisallowed}
            onClick={submitCode}
            className={`${
                SubmitDisallowed
                    ? "bg-white text-[#BBBCBD]  rounded-[8px] border-[1px] my-2 py-1 border-[#BBBCBD]  w-[100%]"
                    : "text-white bg-[#3283FC]  rounded-[8px] border-[1px] my-2 py-1 border-[#BBBCBD]  w-[100%]"
            }   h-[32px]`}
        >
            Activate
        </button>
        </>
    );
};

export default Activation;