import Facebook from "../assets/Facebook";
import FullLogo from "../assets/FullLogo";
import Google from "../assets/Google";
import RightArrow from "../assets/RightArrow";
import forth from "../assets/images/forth.jpg";
import Lock from "../assets/images/lock.png";
import mail from "../assets/images/mail.png";
import one from "../assets/images/one.jpg";
import tree from "../assets/images/tree.jpg";
import two from "../assets/images/two.jpg";
import Tick from "../assets/Tick";

import { useEffect, useState } from "react";
import { Bounce, toast } from "react-toastify";
import ConfirmEmail from "./AuthParts/ConfirmEmail";
import LogAuthCode from "./AuthParts/LogAuthCode";
import ForgotPassword from "./AuthParts/forgotPassword";
import Login from "./AuthParts/login";
import Register from "./AuthParts/register";
import ResetPassword from "./AuthParts/resetPassword";
import Activation from "./AuthParts/Activation";
import SuccessInfo from "./AuthParts/SuccessInfo";
import { config } from "../global";

type TProps = {
    setAuthState: any;
    authTarget: any;
    target: any;
};

const Auth = ({ setAuthState, authTarget, target }: TProps) => {
    const [imageIndex, setImageIndex] = useState(0);
    const [AppState, setAppState] = useState( authTarget );
    const [sendCodeState, setsendCodeState] = useState(false);
    const [count, setCount] = useState(60);

    useEffect(() => {
        const switchImage = () => {
            const timeoutId = setTimeout(() => {
                setImageIndex((prevIndex) => (prevIndex < 3 ? prevIndex + 1 : 0));
            }, 2000);
            return () => clearTimeout(timeoutId);
        };
        switchImage();
    }, [imageIndex]);
    useEffect(() => {
        let timer: number | undefined;

        if (sendCodeState) {
            timer = setInterval(() => {
                if (count === 0) {
                    setsendCodeState(false);
                    clearInterval(timer);
                } else {
                    setCount((prevCount) => prevCount - 1);
                }
            }, 1000);
        }

        return () => clearInterval(timer);
    }, [count, sendCodeState]);

    const handleClick = () => {
        toast.success(
            <>
                <h1 className="text-black text-[16px] font-normal	">Email Verified Successfully</h1>
                <p className="text-[#333739] text-[14px] font-normal">
                    Your email address has been verified.
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
        setsendCodeState(true);
        setAppState(target.confirm);
        setCount(60);
    };

    return (
        <>
            <div className=" w-full h-full overflow-hidden">
                <img
                    src={one}
                    alt={`Image ${imageIndex + 1}`}
                    className={`absolute top-0 right-0  w-full h-full inset-0 transition duration-500 ${
                        imageIndex === 0 ? "opacity-100 z-40" : "opacity-0 z-0"
                    }`}
                />
                <img
                    src={two}
                    alt={`Image ${((imageIndex + 1) % 4) + 1}`}
                    className={`absolute top-0 right-0  w-full h-full inset-0 transition duration-500 ${
                        imageIndex === 1 ? "opacity-100 z-40" : "opacity-0 z-0"
                    }`}
                />
                <img
                    src={tree}
                    alt={`Image ${((imageIndex + 2) % 4) + 1}`}
                    className={`absolute top-0 right-0  w-full h-full inset-0 transition duration-500 ${
                        imageIndex === 2 ? "opacity-100 z-40" : "opacity-0 z-0"
                    }`}
                />
                <img
                    src={forth}
                    alt={`Image ${((imageIndex + 3) % 4) + 1}`}
                    className={`absolute top-0 right-0  w-full h-full inset-0 transition duration-500 ${
                        imageIndex === 3 ? "opacity-100 z-40" : "opacity-0 z-0"
                    }`}
                />
                <div className="absolute top-0 left-0 w-full h-full z-50 flex justify-center items-center">
                    <div
                        className={`w-[400px] min-w-[400px]  p-4 rounded-[24px] bg-[#F9F9F9] ${
                            AppState === target.log
                                ? ( config.IS_APP ? " min-h-[520px] h-[580px]" : " min-h-[520px] h-[520px]")
                                : AppState === target.reg
                                ? ( config.IS_APP ? " min-h-[540px] h-[540px]" : " min-h-[480px] h-[480px]")
                                : AppState === target.res
                                ? " min-h-[350px] h-[380px]"
                                : AppState === target.rec
                                ? " min-h-[480px] h-[480px]"
                                : AppState === target.auth
                                ? "min-h-[380px] h-[380px]"
                                : AppState === target.success
                                ? "min-h-[280px] h-[280px]"
                                : AppState === target.activate
                                ? " min-h-[360px] h-[500px]"
                                : " min-h-[450px] h-[450px]"
                        }`}
                    >
                        <div className="w-full flex flex-col justify-center items-center">
                            {AppState === target.auth || AppState === target.confirm ? (
                                <img src={Lock} />
                            ) : AppState === target.activate ? (
                                <img src={mail} />
                            ) : AppState === target.success ? (
                                <Tick />
                            ) : (
                                <FullLogo />
                            )}
                            <p className="my-4 text-[#1D4C91] text-[32px] font-bold">
                                {AppState === target.res || AppState === target.res
                                    ? "Forget Password"
                                    : AppState === target.auth || AppState === target.confirm
                                    ? "Authorization"
                                    : AppState === target.activate
                                    ? "Email Verification"
                                    : AppState === target.success
                                    ? "SUCCESS"
                                    : "Welcome"}
                            </p>
                            <p className=" mb-4 text-[#1C2123] text-[16px] flex-nowrap font-[400] ml-5 mr-5">
                                {AppState === target.reg
                                    ? "Please register to continue with AURGA"
                                    : AppState === target.log
                                    ? "Please login to continue with AURGA"
                                    : AppState === target.res
                                    ? "Please enter your email to reset your password."
                                    : AppState === target.auth || AppState === target.confirm
                                    ? ( config.IS_APP ? "Please enter your authorization code." : "Please enter your 6-digit authorization code." )
                                    : AppState === target.success
                                    ? ""
                                    : AppState === target.activate
                                    ? "Check your inbox to verify your email address. If you didn't see the email, please check your spam folder. If you still didn't receive the email, it's possible that your email server rejected our email. Please send an email to account@mail.aurga.com first, or try using a different email address."
                                    : "Please fill your new password."}
                            </p>
                            <div className="w-[90%] flex flex-col justify-end items-end ">
                                {AppState === target.reg ? (
                                    <>
                                        <Register setAppState={setAppState} target={target} />
                                    </>
                                ) : AppState === target.log ? (
                                    <>
                                        <Login setAppState={setAppState} setAuthState={setAuthState} target={target} />
                                    </>
                                ) : AppState === target.res ? (
                                    <>
                                        <ForgotPassword setAppState={setAppState} target={target} />
                                    </>
                                ) : AppState === target.rec ? (
                                    <>
                                        <ResetPassword setAppState={setAppState} target={target} />
                                    </>
                                ) : AppState === target.auth ? (
                                    <>
                                        <LogAuthCode setAppState={setAppState} target={target} />
                                    </>
                                ) : AppState === target.success ? (
                                    <>
                                        <SuccessInfo setAppState={setAppState} target={target} />
                                    </>
                                )  : AppState === target.activate ? (
                                    <>
                                       <Activation setAppState={setAppState} target={target} />
                                    </>
                                ) : (
                                    <>
                                        <ConfirmEmail setAppState={setAppState} target={target} />
                                    </>
                                )}
                            </div>
                            {(AppState === target.reg || AppState === target.log) && config.IS_APP ? (
                                <>
                                    {/* <div className="w-[90%] flex flex-col justify-center items-center ">
                                        <div className="divider my-8">OR</div>
                                    </div>
                                    <div className="w-[90%] mb-4 flex flex-col justify-center items-center ">
                                        <button
                                            type="button"
                                            className=" w-full  bg-[#fff] hover:bg-[#fff] focus:ring-4 text-black focus:outline-none focus:ring-[#fff]/50 font-medium rounded-lg text-sm px-5 py-2.5 text-center inline-flex items-center justify-between mr-2 mb-2"
                                        >
                                            <Google />
                                            Continue with Google<div></div>
                                        </button>
                                        <button
                                            type="button"
                                            className=" w-full  bg-[#fff] hover:bg-[#fff] focus:ring-4 text-black focus:outline-none focus:ring-[#fff]/50 font-medium rounded-lg text-sm px-5 py-2.5 text-center inline-flex items-center justify-between mr-2 mb-2"
                                        >
                                            <Facebook />
                                            Continue with Facebook<div></div>
                                        </button>
                                    </div> */}
                                    <div
                                        className={`w-[90%]  flex flex-col justify-center items-center ${
                                            AppState === target.reg ? "mt-2" : "mt-8"
                                        }`}
                                    >
                                        <button
                                            onClick={() => {
                                                setAuthState(true);
                                            }}
                                            type="button"
                                            className=" w-full  bg-[#5B9CFD] hover:bg-[#5B9CFD] focus:ring-4  focus:outline-none focus:ring-[#5B9CFD]/50 font-medium rounded-lg  px-5 py-2.5 text-center inline-flex items-center justify-center mr-2 mb-2"
                                        >
                                            <RightArrow />
                                            <p className="ml-2 text-white text-sm">
                                                Skip for now
                                            </p>{" "}
                                            <div></div>
                                        </button>
                                    </div>
                                </>
                            ) : (
                                ""
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
};

export default Auth;
