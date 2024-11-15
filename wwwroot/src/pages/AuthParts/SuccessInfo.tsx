import { useEffect, useState } from "react";
import { Bounce, toast } from "react-toastify";
import { config } from '../../global';

const SuccessInfo = ({ setAppState, target }: any) => {

    return (
        <>
            <div className="w-full my-4 flex justify-between items-center">
                <button
                    className="rounded-[8px] border-[1px] py-1 border-[#BBBCBD] w-[100%] bg-white "
                    onClick={() => {
                        if(config.IS_APP){
                            setAppState(target.log);
                        }else{
                            setAppState(target.reg);
                        }
                    }}
                >
                    Home
                </button>
            </div>
        </>
    );
};

export default SuccessInfo;
