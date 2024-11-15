import { motion } from "framer-motion";
import SelectDropDownAutoDetect from "../components/select/SelectDropDownAutoDetect";
import SelectDropDownHardWare from "../components/select/SelectDropDownHardWare";
import SelectDropDownWinMode from "../components/select/SelectDropDownWinMode";
import SelectDropDownRender from "../components/select/SelectDropDownRender";
import { config } from "../global";
import { useEffect, useRef, useState } from "react";
import Cookies from 'js-cookie';
import axios from 'axios';
import { toast } from "react-toastify";
import Switch from 'react-switch';

const Settings = ({ setAuthState }: any) => {
    
    return (
        <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            className="flex flex-col text-[22px] gap-6 text-site-black-100 px-10 transition-opacity "
        >
            <div className="text-[22px] flex ">Settings</div>
        </motion.div>
    );
};

export default Settings;
