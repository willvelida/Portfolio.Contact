using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Portfolio.Contact.Models;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portfolio.Contact.Mappers
{
    public interface ISendGridMessageMapper
    {
        SendGridMessage MapRequestToMessage(string requestBody);
    }

    public class SendGridMessageMapper : ISendGridMessageMapper
    {
        private readonly IConfiguration _config;

        public SendGridMessageMapper(IConfiguration config)
        {
            _config = config;
        }

        public SendGridMessage MapRequestToMessage(string requestBody)
        {
            var input = JsonConvert.DeserializeObject<OutgoingEmailMessage>(requestBody);

            var outgoingEmail = new OutgoingEmailMessage
            {
                SenderName = input.SenderName,
                SenderEmail = input.SenderEmail,
                EmailSubject = input.EmailSubject,
                EmailBody = input.EmailBody
            };

            // Send the parsed message to SendGrid Client
            var message = new SendGridMessage()
            {
                From = new EmailAddress(outgoingEmail.SenderEmail, outgoingEmail.SenderName),
                Subject = outgoingEmail.EmailSubject,
                PlainTextContent = outgoingEmail.EmailBody,
                HtmlContent = outgoingEmail.EmailBody,
            };
            message.AddTo(_config["RecipientEmail"]);

            return message;
        }
    }
}
