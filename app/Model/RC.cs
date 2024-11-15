namespace aurga.Model
{
    public static class RC
    {
        public const int SUCCESS = 0;
        public const int IP_RESTRICT = -100;

        public const int INVALID_PARAMETERS = -101;
        public const int INVALID_EMAIL_FORMAT = -102;
        public const int EMAIL_REGISTERED = -103;
        public const int ACCOUNT_NOT_EXISTS = -104;
        public const int DEVICE_NOT_EXISTS = -105;
        public const int TOKEN_EXPIRED = -106;
        public const int ACCOUNT_NOT_ACTIVATED = -107;
        public const int ACCOUNT_IS_ACTIVATED = -108;
        public const int TOKEN_MISMATCH = -109;
        public const int VERIFICATION_CODE_MISMATCH = -110;
        public const int VERIFICATION_CODE_EXPIRED = -111;
        public const int EXCEPTION = -1000;

        public const int SERVER_IS_NOT_READY = -2000;
        public const int CUSTOM_ERROR = -3000;
    }
}
