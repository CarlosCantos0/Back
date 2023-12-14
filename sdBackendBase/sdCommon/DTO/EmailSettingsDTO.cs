using System;
using System.Collections.Generic;
using System.Text;

namespace sdCommon.DTO
{
    public class EmailSettingsDTO
    {
        public string MailServer { get; set; }
        public int MailPort { get; set; }
        public string SenderName { get; set; }
        public string EmailSender { get; set; }
        public string Password { get; set; }
        public string User { get; set; }
        public bool EnableSsl { get; set; }
    }
}
