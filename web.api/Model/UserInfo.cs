﻿namespace aurga.Model
{
    public class UserInfo
    {
        public required long Id { get; set; }
        public string? Name { get; set; }
        public required string EmailHash { get; set; }
        public required string PasswordHash { get; set; }
        public required string Email { get; set; }
        public required string UserGUID { get; set; }
        public required string Token { get; set; }
        public string LastToken { get; set; }
        public bool Activated { get; set; }
        public string ActivationVerificationCode { get; set; }
        public string ActivationToken { get; set; }
        public DateTime ActivationVerificationTime { get; set; }

        public string ResetPasswordVerificationCode { get; set; }
        public string ResetPasswordToken { get; set; }
        public DateTime ResetPasswordVerificationTime { get; set; }

        public string DeactivationVerificationCode { get; set; }
        public string DeactivationToken { get; set; }
        public DateTime DeactivationVerificationTime { get; set; }

        public DateTime LastAccess { get; set; } = DateTime.MinValue;
        public List<DeviceStatus> Devices { get; private set; } = new List<DeviceStatus>();
    }
}
