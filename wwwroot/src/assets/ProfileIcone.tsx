import React from "react";

const ProfileIcone = ({ ...props }: React.SVGAttributes<SVGSVGElement>) => {
  return (
    <svg
      {...props}
      width="52"
      height="52"
      viewBox="0 0 52 52"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <g filter="url(#filter0_dd_60_9163)">
        <rect
          x="4"
          width="44"
          height="44"
          rx="22"
          fill="#1D4C91"
          shapeRendering="crispEdges"
        />
        <path
          d="M34.5076 28.1747C34.0444 27.0776 33.3723 26.081 32.5286 25.2405C31.6875 24.3976 30.6911 23.7256 29.5944 23.2615C29.5846 23.2566 29.5748 23.2541 29.565 23.2492C31.0946 22.1443 32.0891 20.3445 32.0891 18.314C32.0891 14.9501 29.3636 12.2247 25.9998 12.2247C22.6359 12.2247 19.9105 14.9501 19.9105 18.314C19.9105 20.3445 20.9049 22.1443 22.4346 23.2517C22.4248 23.2566 22.415 23.259 22.4051 23.264C21.3051 23.728 20.3181 24.3934 19.471 25.243C18.6281 26.0841 17.956 27.0805 17.492 28.1771C17.0361 29.2507 16.7902 30.4018 16.7676 31.568C16.767 31.5942 16.7716 31.6203 16.7812 31.6447C16.7907 31.6691 16.8051 31.6913 16.8234 31.7101C16.8417 31.7288 16.8636 31.7437 16.8877 31.7539C16.9119 31.7641 16.9379 31.7693 16.9641 31.7693H18.4373C18.5453 31.7693 18.6313 31.6834 18.6337 31.5778C18.6828 29.6823 19.444 27.907 20.7895 26.5615C22.1817 25.1693 24.0306 24.4032 25.9998 24.4032C27.969 24.4032 29.8179 25.1693 31.2101 26.5615C32.5556 27.907 33.3167 29.6823 33.3659 31.5778C33.3683 31.6858 33.4542 31.7693 33.5623 31.7693H35.0355C35.0617 31.7693 35.0877 31.7641 35.1118 31.7539C35.136 31.7437 35.1579 31.7288 35.1762 31.7101C35.1945 31.6913 35.2088 31.6691 35.2184 31.6447C35.228 31.6203 35.2326 31.5942 35.2319 31.568C35.2074 30.3943 34.9643 29.2526 34.5076 28.1747ZM25.9998 22.5372C24.8728 22.5372 23.8121 22.0977 23.0141 21.2997C22.2161 20.5017 21.7766 19.441 21.7766 18.314C21.7766 17.1869 22.2161 16.1262 23.0141 15.3282C23.8121 14.5303 24.8728 14.0907 25.9998 14.0907C27.1268 14.0907 28.1875 14.5303 28.9855 15.3282C29.7835 16.1262 30.223 17.1869 30.223 18.314C30.223 19.441 29.7835 20.5017 28.9855 21.2997C28.1875 22.0977 27.1268 22.5372 25.9998 22.5372Z"
          fill="white"
        />
      </g>
      <defs>
        <filter
          id="filter0_dd_60_9163"
          x="0"
          y="0"
          width="52"
          height="52"
          filterUnits="userSpaceOnUse"
          colorInterpolationFilters="sRGB"
        >
          <feFlood floodOpacity="0" result="BackgroundImageFix" />
          <feColorMatrix
            in="SourceAlpha"
            type="matrix"
            values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0"
            result="hardAlpha"
          />
          <feOffset dy="4" />
          <feGaussianBlur stdDeviation="2" />
          <feComposite in2="hardAlpha" operator="out" />
          <feColorMatrix
            type="matrix"
            values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.25 0"
          />
          <feBlend
            mode="normal"
            in2="BackgroundImageFix"
            result="effect1_dropShadow_60_9163"
          />
          <feColorMatrix
            in="SourceAlpha"
            type="matrix"
            values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0"
            result="hardAlpha"
          />
          <feOffset dy="4" />
          <feGaussianBlur stdDeviation="2" />
          <feComposite in2="hardAlpha" operator="out" />
          <feColorMatrix
            type="matrix"
            values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.25 0"
          />
          <feBlend
            mode="normal"
            in2="effect1_dropShadow_60_9163"
            result="effect2_dropShadow_60_9163"
          />
          <feBlend
            mode="normal"
            in="SourceGraphic"
            in2="effect2_dropShadow_60_9163"
            result="shape"
          />
        </filter>
      </defs>
    </svg>
  );
};

export default ProfileIcone;
