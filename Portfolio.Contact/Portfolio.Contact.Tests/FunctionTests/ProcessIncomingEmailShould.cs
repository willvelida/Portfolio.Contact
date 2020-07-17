using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Portfolio.Contact.Functions;
using Portfolio.Contact.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Portfolio.Contact.Tests.FunctionTests
{
    public class ProcessIncomingEmailShould
    {
        Mock<ILogger<ProcessIncomingEmail>> _loggerMock;
        Mock<IConfiguration> _configMock;
        Mock<SendGridClient> _sendGridMock;
        Mock<HttpRequest> _httpRequestMock;

        public ProcessIncomingEmailShould()
        {
            _loggerMock = new Mock<ILogger<ProcessIncomingEmail>>();
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(x => x["RecipientEmail"]).Returns("test@test.com");
            _configMock.Setup(x => x["SendGridAPIKey"]).Returns("key");
            _sendGridMock = new Mock<SendGridClient>(MockBehavior.Strict);
            _httpRequestMock = new Mock<HttpRequest>();
        }

        [Fact]
        public async Task SendValidEmail()
        {
            // ARRANGE
            var mockedOutgoingMessageRequestBody = new OutgoingEmailMessage
            {
                SenderName = "TestName",
                SenderEmail = "testuser@mail.com",
                EmailSubject = "TestEmail",
                EmailBody = "This is a test email"
            };

            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(mockedOutgoingMessageRequestBody));
            MemoryStream stream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(stream);
            

            var sendEmail = new ProcessIncomingEmail(_loggerMock.Object, _configMock.Object, _sendGridMock.Object);

            // ACT
            var response = await sendEmail.Run(_httpRequestMock.Object);

            // ASSERT
            _sendGridMock.Verify(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void ConvertRequestToOutGoingMessagePOCO()
        {

        }

        [Fact]
        public async Task ThrowBadRequestWhenInvalid()
        {

        }

        [Fact]
        public async Task ReturnInternalServerError()
        {

        }

        [Fact]
        public async Task ReturnOkStatus()
        {

        }
    }
}
