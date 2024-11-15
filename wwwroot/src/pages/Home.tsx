import { Disclosure } from "@headlessui/react";
import { motion } from "framer-motion";
import Cookies from "js-cookie";
import { useEffect, useState } from "react";
import RightArrowS from "../assets/RightArrowS";
import useDeviceStore from "../zustand/devices.store";
import { config } from "../global";

const Home = ({ setAuthState }: any) => {
    const [logged, setLogged] = useState(true);

    const handelLogin = () => {
        setLogged(true);
        setAuthState(false);
    };

    return (
        <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            className="flex flex-col gap-10 text-site-black-100 px-10  transition-opacity "
        >

        </motion.div>
    );
};

export default Home;
