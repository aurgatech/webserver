import { useEffect, useState } from "react";

const SVGComponent = ({ svgs }: any) => {
    const [currentSVGIndex, setCurrentSVGIndex] = useState(0);

    useEffect(() => {
        const interval = setInterval(() => {
            setCurrentSVGIndex((prevIndex) => (prevIndex === svgs.length - 1 ? 0 : prevIndex + 1));
        }, 200);
        return () => clearInterval(interval);
    }, [svgs]);

    return (
        <div>
            {svgs.map((svg: any, index: any) => (
                <div key={index} style={{ display: index === currentSVGIndex ? "block" : "none" }}>
                    <img src={svg} alt="" className="w-[48px] h-[48px]" />
                </div>
            ))}
        </div>
    );
};

export default SVGComponent;
