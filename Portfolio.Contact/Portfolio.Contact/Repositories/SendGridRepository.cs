using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Portfolio.Contact.Repositories
{
    public interface ISendGridRepository
    {
        Task<Response> SendEmail(SendGridMessage sendGridMessage);
    }

    public class SendGridRepository : ISendGridRepository
    {
        private readonly SendGridClient _sendGridClient;

        public SendGridRepository(SendGridClient sendGridClient)
        {
            _sendGridClient = sendGridClient;
        }

        public async Task<Response> SendEmail(SendGridMessage sendGridMessage)
        {
            return await _sendGridClient.SendEmailAsync(sendGridMessage);
        }
    }
}
