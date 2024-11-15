import { useEffect, useState, useRef } from "react";
// import { Bounce, toast } from "react-toastify";
import { motion } from "framer-motion";

const Devices = ({ setAuthState }: any) => {
    const [isSearching, setIsSearching] = useState<boolean>(false);

    const handelSearching = () => {
        setIsSearching(false);

        // for(var i = 0; i < 10; i++){
        //     const device = {
        //         id: "123",
        //         name: "Device " + i,
        //         model: "1",
        //         bip: "192.168.1.13",
        //         apip: "192.168.172.23",
        //         addr: "192.168.1.13",
        //         laddr: "192.168.1.13",
        //         is_online_device: false,
        //         is_usb: true,
        //         is_hid: false,
        //         timestamp: Date.now(),
        //         build: "12312312312",
        //         version: "2.0.0.1",
        //     };

        //     setDevices((prevDevices) => [...prevDevices, device]);
        // }
    };
    
    
    return (
        <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            className="flex flex-col gap-6 text-site-black-100 px-10 transition-opacity "
        >
            <div className="text-[22px] flex ">Search Your Device</div>
        </motion.div>
    );
};

export default Devices;
