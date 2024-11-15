/** @type {import('tailwindcss').Config} */
export default {
    content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
    theme: {
        extend: {
            colors: {
                "primary-light": {
                    10: "#EEFCFB",
                    20: "#DCFAF7",
                    30: "#CBF7F3",
                    40: "#BAF4EF",
                    50: "#A8F1EB",
                    60: "#97EFE8",
                    70: "#86ECE4",
                    80: "#75E9E0",
                    90: "#63E7DC",
                    100: "#52E4D8",
                },

                "primary-dark": {
                    10: "#E7F0F0",
                    20: "#CFE1E0",
                    30: "#B6D2D1",
                    40: "#9EC3C2",
                    50: "#86B4B3",
                    60: "#6EA5A3",
                    70: "#569694",
                    80: "#3D8785",
                    90: "#257875",
                    100: "#0D6966",
                    "100-se": "#133544",
                },
                "secondary-light": {
                    10: "#EBF3FF",
                    20: "#D6E6FE",
                    30: "#C2DAFE",
                    40: "#ADCDFE",
                    50: "#98C1FD",
                    60: "#84B5FD",
                    70: "#70A8FD",
                    80: "#5B9CFD",
                    90: "#478FFC",
                    100: "#3283FC",
                },

                "secondary-dark": {
                    10: "#E9EFF7",
                    20: "#D3DEEF",
                    30: "#BDCEE7",
                    40: "#A7BDDF",
                    50: "#91ADD7",
                    60: "#7B9DCF",
                    70: "#658CC7",
                    80: "#4F7CBF",
                    90: "#396CB7",
                    100: "#235BAF",
                    "100-se": "#1D4C91",
                },
                "tertiary-orange": {
                    10: "#FFF0EB",
                    20: "#FFE1D7",
                    30: "#FFD3C2",
                    40: "#FFC4AE",
                    50: "#FFB59A",
                    60: "#FFA686",
                    70: "#FF9772",
                    80: "#FF895D",
                    90: "#FF7A49",
                    100: "#FF6B35",
                },
                "tertiary-purple": {
                    10: "#F0E8FA",
                    20: "#E2D2F5",
                    30: "#D3BBF1",
                    40: "#C4A5EC",
                    50: "#B58FE7",
                    60: "#A778E2",
                    70: "#9862DD",
                    80: "#894BD9",
                    90: "#7B35D4",
                    100: "#6C1ECF",
                },
                "site-black": {
                    10: "#E8E9E9",
                    20: "#D2D3D3",
                    30: "#BBBCBD",
                    40: "#A4A6A7",
                    50: "#8D9091",
                    60: "#777A7B",
                    70: "#606465",
                    80: "#494D4F",
                    90: "#333739",
                    100: "#1C2123",
                },
                "site-white": {
                    10: "#FEFEFE",
                    20: "#FCFCFC",
                    30: "#FBFBFB",
                    40: "#FAFAFA",
                    50: "#F9F9F9",
                    60: "#F7F7F7",
                    70: "#F6F6F6",
                    80: "#F5F5F5",
                    90: "#F4F4F4",
                    100: "#F2F2F2",
                },
            },
        },
    },
    // eslint-disable-next-line no-undef
    plugins: [require("daisyui")],
    daisyui: {
        darkTheme: "light",
    },
};
