using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace aurga.Common
{
    public static class EmailHelper
    {
        public static void SendActivationEmail(string email, string verificationCode)
        {
            var msg = new MailItem();
            msg.Subject = "Activate Your Account Now!";
            msg.CampaignId = -1;
            msg.Address = email;
            msg.Message = $@"Hey,

Welcome aboard to our platform! We're excited to have you join us. To get started on your journey, we need you to activate your account. This step is crucial as it unlocks a powerful feature: remote device access.

Once your account is activated, you'll be able to securely access the devices associated with your account from anywhere via the internet. Your activated account is the key to seamless remote access.

To activate your account, please enter the following 6-digit verification code on our website:

Verification Code:   {verificationCode}

Please note that this verification code will expire in 10 minutes, so be sure to enter it promptly.

Should you encounter any difficulties or have questions along the way, our support team is here to assist you. Just drop us a message via this email.

Thank you for choosing us. We can't wait to see how you make the most of your remotely accessible devices!

Best regards,
The AURGA Team";

            MailSender.DefaultSender.SendMail(msg);
        }

        public static void SendResetPasswordEmail(string email, string verificationCode)
        {
            var msg = new MailItem();
            msg.Subject = "Password Reset Request - Action Required";
            msg.CampaignId = -1;
            msg.Address = email;
            msg.Message = $@"Hey,

We've received a request to reset your password for your account. To proceed with resetting your password, please enter the following 6-digit verification code on our website:

Verification Code:   {verificationCode}

Please note that this verification code will expire in 10 minutes, so be sure to enter it promptly.

If you did not initiate this request, please ignore this email and your password will remain unchanged.

If you have any questions or need further assistance, feel free to contact our support team via this email.

Best regards,
The AURGA Team";

            MailSender.DefaultSender.SendMail(msg);
        }

        public static void SendDeactivationCodeEmail(string email, string verificationCode)
        {
            var msg = new MailItem();
            msg.Subject = "Verification Code for Account Deletion Request";
            msg.CampaignId = -1;
            msg.Address = email;
            msg.Message = $@"Hey,

We have received a request to delete your account. To ensure the security of your account, please enter the following 6-digit verification code to proceed with the deletion:

Verification Code: {verificationCode}

Please be aware that this verification code will expire in 10 minutes. Kindly enter it promptly to avoid any inconvenience.

If you did not initiate this request, please disregard this email. Your account will remain active unless the verification code is entered and the deletion process is confirmed.

Should you have any questions or require further assistance, please do not hesitate to reach out to our support team at supports@aurga.com.

Thank you for your attention to this matter.

Best regards,
The AURGA Team";

            MailSender.DefaultSender.SendMail(msg);
        }

        public static void SendDeactivationConfirmationEmail(string email)
        {
            var msg = new MailItem();
            msg.Subject = "Account Deletion Confirmation";
            msg.CampaignId = -1;
            msg.Address = email;
            msg.Message = $@"Hey,

We hope this message finds you well. We are writing to inform you that your account deletion request has been successfully processed. As per your instructions, your account and all associated data have been permanently removed from our website.

Please note that this action is irreversible, and we will not be able to restore your account or any of its content. If you have any concerns or if there was an error in the deletion process, please contact our support team immediately at supports@aurga.com.

We appreciate your time with us and hope that you had a positive experience. If you ever decide to return, we would be delighted to welcome you back.

Thank you for being a part of our community.

Warm regards,
The AURGA Team";

            MailSender.DefaultSender.SendMail(msg);
        }

        public static void SendActivationSuccess(string email)
        {
            var msg = new MailItem();
            msg.Subject = "Your Account is Activated!";
            msg.CampaignId = -1;
            msg.Address = email;
            msg.Message = $@"Hey,

Great news! Your account with AURGA is now activated and ready to go. 🎉

If you have any questions or need assistance with anything, don't hesitate to reach out to us. We're here to help!

Thank you for choosing AURGA. We can't wait to see what you'll achieve with your activated account!

Best regards,
The AURGA Team";

            MailSender.DefaultSender.SendMail(msg);
        }

        public static void SendResetPasswordSuccess(string email)
        {
            var msg = new MailItem();
            msg.Subject = "Password Reset Successful";
            msg.CampaignId = -1;
            msg.Address = email;
            msg.Message = $@"Hey,

We wanted to inform you that your password reset request has been successfully processed. Your password has been updated, and you can now access your account with the new credentials.

If you have any further questions or need assistance, feel free to reach out to our support team. We're here to help!

Thank you for choosing AURGA.

Best regards,
The AURGA Team";

            MailSender.DefaultSender.SendMail(msg);
        }
    }
}
