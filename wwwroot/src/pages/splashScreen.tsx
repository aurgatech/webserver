import forth from "../assets/images/forth.jpg";
import logo from "../assets/images/logo.png";
import one from "../assets/images/one.jpg";
import tree from "../assets/images/tree.jpg";
import two from "../assets/images/two.jpg";

import { useEffect, useState } from "react";

const SplashScreen = () => {
    const [imageIndex, setImageIndex] = useState(0);

    useEffect(() => {
        // Function to switch to the next image after a delay (e.g., 1 second)
        const switchImage = () => {
            const timeoutId = setTimeout(() => {
                setImageIndex((prevIndex) => (prevIndex < 3 ? prevIndex + 1 : 0));
            }, 1000); // Change image every 1 second (adjust timing as needed)

            // Clean up the timeout to avoid memory leaks
            return () => clearTimeout(timeoutId);
        };

        // Start the image switching process
        switchImage();
    }, [imageIndex]); // Re-run this effect whenever imageIndex changes

    return (
        <>
            <div className=" w-full h-full overflow-hidden">
                <img
                    src={one}
                    alt={`Image ${imageIndex + 1}`}
                    className={`absolute top-0 right-0  w-full h-full inset-0 transition duration-500 ${
                        imageIndex === 0 ? "opacity-100 z-50" : "opacity-0 z-0"
                    }`}
                />
                <img
                    src={two}
                    alt={`Image ${((imageIndex + 1) % 4) + 1}`}
                    className={`absolute top-0 right-0  w-full h-full inset-0 transition duration-500 ${
                        imageIndex === 1 ? "opacity-100 z-50" : "opacity-0 z-0"
                    }`}
                />
                <img
                    src={tree}
                    alt={`Image ${((imageIndex + 2) % 4) + 1}`}
                    className={`absolute top-0 right-0  w-full h-full inset-0 transition duration-500 ${
                        imageIndex === 2 ? "opacity-100 z-50" : "opacity-0 z-0"
                    }`}
                />
                <img
                    src={forth}
                    alt={`Image ${((imageIndex + 3) % 4) + 1}`}
                    className={`absolute top-0 right-0  w-full h-full inset-0 transition duration-500 ${
                        imageIndex === 3 ? "opacity-100 z-50" : "opacity-0 z-0"
                    }`}
                />
            </div>
            <div className="w-full justify-center items-center flex h-screen z-50">
                <img src={logo} alt={`logo`} className={`w-[200px] h-[168px] z-50`} />
            </div>
        </>
    );
};

export default SplashScreen;
