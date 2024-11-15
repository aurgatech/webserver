import { useEffect, useState } from "react";
import { IoIosArrowDropleft } from "react-icons/io";
import { Link, useLocation, useNavigate } from "react-router-dom";
import packageJson from "../../package.json"; // Adjust the path as necessary
import DevicesIcon from "../assets/DevicesIcon";
import Logo from "../assets/Logo";
import Logo2 from "../assets/Logo2";
import MenuDesign2 from "../assets/MenuDesign2";
import MenuIcon from "../assets/MenuIcon";
import SettingsIcon from "../assets/SettingsIcon";
import MenuDesign from "../assets/menuDesign";
import { config } from "../global";

export default function Index({ children }: { children: React.ReactNode }) {
    const router = useNavigate();
    const pathname = useLocation().pathname;

    useEffect(() => {}, [router]);
    return (
        <div className="w-full flex h-screen overflow-hidden bg-slate-100">
            <SideBar pathname={pathname} />
            <div className="flex-1 w-full overflow-y-auto h-full m-2 transition-opacity">
                {children}
            </div>
        </div>
    );
}

export const SideBar = ({ pathname }: { pathname: string }) => {
    const location = useLocation();
    const [selected, setSelected] = useState<string>(location.pathname);

    const [open, setOpen] = useState<boolean>(true);
    const [appVersion, setAppVersion] = useState<string>("");

    const links: { name: string; icon: React.ReactNode; path: string }[] = [
        {
            name: "Home",
            icon: <MenuIcon className="w-12" fill={selected === "/" ? "#3283FC" : "black"} />,
            path: "/",
        },
        {
            name: "Devices",
            icon: (
                <DevicesIcon
                    className="w-12"
                    fill={selected === "/devices" ? "#3283FC" : "black"}
                />
            ),
            path: "/devices",
        },
        {
            name: "Settings",
            icon: (
                <SettingsIcon
                    className="w-12"
                    fill={selected === "/settings" ? "#3283FC" : "black"}
                />
            ),
            path: "/settings",
        },
    ];
    useEffect(() => {
        //console.log(selected);
    }, [selected]);
    useEffect(() => {
        setSelected(location.pathname);
    }, [location.pathname]);

    useEffect(() => {
        window.onAppVersion = (version: string) => {
            setAppVersion(version);
        };
        
        try {
            if (config.OS === "osx") {
                const dic = {
                    func: "get_app_version",
                };
    
                window.webkit.messageHandlers["js2objc"].postMessage(dic);
            } else {
                if (typeof window.cef_get_app_version === "function") {
                    window.cef_get_app_version();
                }
            }

            //write the blob to file system file:///c:/aurga/fw/latest.img
            
            //console.log('File cached successfully');
        } catch (error) {
            //console.error('Failed to download or cache file:', error);
        }

        return () => {
            delete window.onAppVersion;
        };
    }, []);

    return (
        <div
            className={`${
                open === true ? "w-[190px] menu-transition" : "w-[64px] menu-transition"
            } rounded-xl relative bg-site-black-10 py-5 flex flex-col items-center m-2 `}
        >
            <div className={`menu-transition ${open === true ? "" : "hidden"}`}>
                <Logo2 className="mb-36 mt-10" />
            </div>
            <div className={`menu-transition ${open === false ? "" : "hidden"}`}>
                <Logo className="mb-36 mt-10" />
            </div>
            <div className="absolute bottom-0 z-0	w-full rounded">
                {open === true ? (
                    <div className="relative flex w-full">
                        {" "}
                        <MenuDesign2 className="rounded-xl" />{" "}
                        <div className="bg-white rounded-full absolute bottom-4 left-1/2 transform -translate-x-1/2 h-[24px] w-[80%] text-center text-[#1D4C91] flex flex-row justify-center items-center px-2">
                            <p className="text-[12px] font-bold">App Version</p>
                            <p className="text-[12px] font-bold ml-2">{appVersion}</p>
                        </div>
                    </div>
                ) : (
                    <MenuDesign />
                )}
            </div>
            <ul className="w-full flex z-50 flex-col gap-3">
                {links.map((link) => {
                    return (
                        <Link
                            to={link.path}
                            // onClick={() => {
                            //     setSelected(link.name);
                            // }}
                            className="flex items-center w-full"
                        >
                            <li
                                key={link.path}
                                className={`flex w-full py-2 items-center border-r-4 ${
                                    pathname === link.path
                                        ? "bg-secondary-light-30 text-[#3283FC] border-secondary-light-80"
                                        : ""
                                } ${open === true ? "justify-start px-6 " : "justify-center"}`}
                            >
                                {link.icon}
                                {open === true ? link.name : ""}
                            </li>
                        </Link>
                    );
                })}
            </ul>
            <div
                className={`absolute -right-5 top-10 cursor-pointer rounded-full ${
                    open === true ? "" : "rotate-180"
                }`}
            >
                <IoIosArrowDropleft size={40} color="#3283FC" onClick={() => setOpen(!open)} />
            </div>
        </div>
    );
};
