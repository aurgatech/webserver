const PlusIcon = ({ ...props }: React.SVGAttributes<SVGSVGElement>) => {
  return (
    <svg
      {...props}
      width="20"
      height="20"
      viewBox="0 0 20 20"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <g clipPath="url(#clip0_1_6850)">
        <path
          d="M15.8337 10.8333H10.8337V15.8333H9.16699V10.8333H4.16699V9.16667H9.16699V4.16667H10.8337V9.16667H15.8337V10.8333Z"
          fill="#1C2123"
        />
      </g>
      <defs>
        <clipPath id="clip0_1_6850">
          <rect width="20" height="20" fill="white" />
        </clipPath>
      </defs>
    </svg>
  );
};

export default PlusIcon;
