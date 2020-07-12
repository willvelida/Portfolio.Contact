using System;
using System.Collections.Generic;
using System.Text;

namespace Portfolio.Contact.Models
{
    public class OutgoingEmailMessage
    {
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
    }
}
