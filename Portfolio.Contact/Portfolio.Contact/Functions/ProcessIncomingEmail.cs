using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid;
using Portfolio.Contact.Models;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;

namespace Portfolio.Contact.Functions
{
    public class ProcessIncomingEmail
    {
        private readonly ILogger<ProcessIncomingEmail> _logger;
        private readonly IConfiguration _config;
        private readonly ISendGridClient _sendGridClient;

        public ProcessIncomingEmail(
            ILogger<ProcessIncomingEmail> logger,
            IConfiguration config,
            ISendGridClient sendGridClient)
        {
            _logger = logger;
            _config = config;
            _sendGridClient = sendGridClient;
        }

        [FunctionName(nameof(ProcessIncomingEmail))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ProcessEmail")] HttpRequest req)
        {
            IActionResult result = null;

            try
            {
                // Parse the incoming message into a Outgoing message object
                string messageRequest = await new StreamReader(req.Body).ReadToEndAsync();

                var input = JsonConvert.DeserializeObject<OutgoingEmailMessage>(messageRequest);

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

                // Process response
                var response = await _sendGridClient.SendEmailAsync(message);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    result = new BadRequestResult();
                }
                else
                {
                    result = new OkResult();
                }               
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error. Exception thrown: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return result;
        }
    }
}
