using MimeKit;

namespace aurga.Common
{
    public class MailItem
    {
        public long CampaignId { get; set; }
        public long LeadId { get; set; }
        public int MailId { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime LastTimestamp { get; set; }
        public int Failures { get; set; } = 0;

    }

    public class MailSender
    {
        private static MailSender _sender;
        private bool running = false;
        public string EmailServer { get; set; }
        public string EmailAccount { get; set; }
        public string EmailPassword { get; set; }

        public static MailSender DefaultSender
        {
            get
            {
                if (_sender == null)
                {
                    _sender = new MailSender();
                }
                return _sender;
            }
        }


        public bool RandomPause { get; internal set; }
        private bool isPRSender = false;

        //SmtpClient client;
        MailKit.Net.Smtp.SmtpClient smtp;
        private Queue<MailItem> messages = new Queue<MailItem>();
        private List<MailItem> failureMessages = new List<MailItem>();

        private MailSender(bool pr = false)
        {
            isPRSender = pr;

            smtp = new MailKit.Net.Smtp.SmtpClient();
        }

        private void Client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }

        private bool ConnectSmtpServer()
        {
            try
            {
                smtp.Connect(this.EmailServer, 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(this.EmailAccount, this.EmailPassword);
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return smtp.IsAuthenticated;
        }

        private void RunThread()
        {
            List<MailItem> tmp = new List<MailItem>();
            while (running)
            {
                MailItem msg = null;
                lock (this)
                {
                    if (messages.Count > 0)
                    {
                        msg = messages.Dequeue();
                    }
                }

                if (msg == null)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    try
                    {
                        Console.WriteLine($"Start sending email... {msg.Address}");
                        if (!smtp.IsAuthenticated)
                        {
                            if (!this.ConnectSmtpServer())
                            {
                                Thread.Sleep(10000);
                                continue;
                            }
                        }

                        var message = new MimeKit.MimeMessage();
                        message.From.Add(new MimeKit.MailboxAddress("AURGA Account Team", this.EmailAccount));

                        message.To.Add(new MimeKit.MailboxAddress(msg.Name, msg.Address));

                        if (!string.IsNullOrEmpty(msg.Address2))
                        {
                            message.Cc.Add(new MimeKit.MailboxAddress(msg.Name, msg.Address2));
                        }

                        message.Subject = msg.Subject;
                        message.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = msg.Message };
                        msg.LastTimestamp = DateTime.Now;
                        smtp.Send(message);
                        Console.WriteLine($"Sent: {msg.Subject}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex.Message}");
                        msg.Failures++;
                        msg.LastTimestamp = DateTime.Now;

                        lock (messages)
                        {
                            if (msg.Failures <= 3)
                            {
                                messages.Enqueue(msg);
                            }
                        }
                    }
                }
            }
        }

        public void SendMail(MailItem message)
        {
            lock (this)
            {
                if (!running)
                {
                    running = true;
                    new Thread(new ThreadStart(RunThread)).Start();
                }

                messages.Enqueue(message);
            }
        }
    
        public void Stop()
        {
            running = false;
        }
    }
}
