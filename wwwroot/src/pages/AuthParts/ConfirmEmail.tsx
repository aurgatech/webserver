import { useEffect, useState } from "react";
import { Bounce, toast } from "react-toastify";
const ConfirmEmail = ({ setAppState, target }: any) => {
    const [changepassword, setchangepassword] = useState(true);
    const [RePassword, setRePassword] = useState("");
    const [Password, setPassword] = useState("");

    useEffect(() => {
        if (Password && RePassword) {
            setchangepassword(false);
        } else {
            setchangepassword(true);
        }
    }, [Password, RePassword]);
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
            <div className="w-full my-4 flex justify-between items-center">
                <button
                    className="rounded-[8px] border-[1px] py-1 border-[#BBBCBD] w-[45%] bg-white "
                    onClick={() => {
                        setAppState(target.log);
                    }}
                >
                    Maybe Later
                </button>

                <button
                    disabled={changepassword}
                    className={`${
                        changepassword
                            ? "bg-white text-[#BBBCBD]  rounded-[8px] border-[1px] py-1 border-[#BBBCBD]  w-[45%]"
                            : "text-white bg-[#3283FC]  rounded-[8px] border-[1px] py-1 border-[#BBBCBD]  w-[45%]"
                    }   h-[32px]`}
                    onClick={() => {
                        if (Password.length === 6) {
                            toast.success(
                                <>
                                    <h1 className="text-black text-[16px] font-normal	">
                                        Authorization Code Saved
                                    </h1>
                                    <p className="text-[#333739] text-[14px] font-normal">
                                        Code has been saved successfully
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
                            setTimeout(() => {
                                setAppState(target.log);
                            }, 5000);
                        } else {
                            toast.error(
                                <>
                                    <h1 className="text-black text-[16px] font-normal	">
                                        Wrong Code!
                                    </h1>
                                    <p className="text-[#333739] text-[14px] font-normal">
                                        Please enter your 6-digit code correctly.
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

export default ConfirmEmail;
