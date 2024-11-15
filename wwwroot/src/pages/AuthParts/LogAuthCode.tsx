import { useEffect, useState } from "react";
import { Bounce, toast } from "react-toastify";
import { config } from "../../global";

const LogAuthCode = ({ setAppState, target }: any) => {
    const [enableSubmit, setEnableSubmit] = useState(true);

    const [Password, setPassword] = useState("");
    let authenticating: boolean = false;

    const handleAuth = async (pwd: string) => {
        if (config.OS === 'osx') {
            var dic = {
              func: "auth_device",
              password: pwd
            };
            
            window.webkit.messageHandlers['js2objc'].postMessage(dic);
          }else{
            if (typeof window.cef_auth_device === 'function') {
                authenticating = true;
                setEnableSubmit(false);
                window.cef_auth_device(pwd);
            }
        }
    };

    useEffect(() => {
        if (Password.length >= 4 && Password.length <= 16) {
            setEnableSubmit(!authenticating);
        } else {
            setEnableSubmit(false);
        }
    }, [Password]);
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
            <div className="w-full my-4 flex justify-between items-center">
                <button
                    className="rounded-[8px] border-[1px] py-1 border-[#BBBCBD] w-[45%] bg-white "
                    onClick={() => {
                        if(config.IS_APP){
                            if (typeof window.onAuthDevice === 'function') {
                                window.onAuthDevice(true);
                            }
                        }else{
                            setAppState(target.log);
                        }
                    }}
                >
                    Cancel
                </button>

                <button
                    disabled={!enableSubmit}
                    className={`${
                        enableSubmit
                            ? "text-white bg-[#3283FC]  rounded-[8px] border-[1px] py-1 border-[#BBBCBD]  w-[45%]"
                            : "bg-white text-[#BBBCBD]  rounded-[8px] border-[1px] py-1 border-[#BBBCBD]  w-[45%]"
                    }   h-[32px]`}
                    onClick={() => {
                        if (Password.length >= 4 && Password.length <= 16) {
                            handleAuth(Password);
                        } else {
                            toast.error(
                                <>
                                    <h1 className="text-black text-[16px] font-normal	">
                                        Wrong Code!
                                    </h1>
                                    <p className="text-[#333739] text-[14px] font-normal">
                                        {
                                            config.IS_APP ?
                                            ("Please enter your authentication code correctly.")
                                            :
                                            ("Please enter your 6-digit code correctly.")
                                        }
                                        
                                    </p>
                                </>,
                                {
                                    position: "top-right",
                                    autoClose: 5000,
                                    hideProgressBar: true,
                                    closeOnClick: true,
                                    pauseOnHover: true,
                                    draggable: true,
                                    progress: undefined,
                                    theme: "light",
                                    transition: Bounce,
                                }
                            );
                        }
                    }}
                >
                    Confirm
                </button>
            </div>
        </>
    );
};

export default LogAuthCode;
