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
using Portfolio.Contact.Repositories;
using Portfolio.Contact.Mappers;

namespace Portfolio.Contact.Functions
{
    public class ProcessIncomingEmail
    {
        private readonly ILogger<ProcessIncomingEmail> _logger;
        private readonly ISendGridMessageMapper _sendGridMessageMapper;
        private readonly ISendGridRepository _sendGridRepository;

        public ProcessIncomingEmail(
            ILogger<ProcessIncomingEmail> logger,
            ISendGridMessageMapper sendGridMessageMapper,
            ISendGridRepository sendGridRepository)
        {
            _logger = logger;
            _sendGridMessageMapper = sendGridMessageMapper;
            _sendGridRepository = sendGridRepository;
        }

        [FunctionName(nameof(ProcessIncomingEmail))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ProcessEmail")] HttpRequest req)
        {
            IActionResult result = null;

            try
            {
                string messageRequest = await new StreamReader(req.Body).ReadToEndAsync();

                var message = _sendGridMessageMapper.MapRequestToMessage(messageRequest);

                var response = await _sendGridRepository.SendEmail(message);

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
