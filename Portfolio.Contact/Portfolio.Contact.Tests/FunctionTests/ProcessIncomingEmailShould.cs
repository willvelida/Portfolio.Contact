using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Portfolio.Contact.Functions;
using Portfolio.Contact.Models;
using Portfolio.Contact.Repositories;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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
        Mock<ISendGridRepository> _sendGridRepoMock;
        Mock<HttpRequest> _httpRequestMock;

        private ProcessIncomingEmail _func;

        public ProcessIncomingEmailShould()
        {
            _loggerMock = new Mock<ILogger<ProcessIncomingEmail>>();
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(x => x["RecipientEmail"]).Returns("test@test.com");
            _configMock.Setup(x => x["SendGridAPIKey"]).Returns("key");
            _sendGridRepoMock = new Mock<ISendGridRepository>();
            _httpRequestMock = new Mock<HttpRequest>();

            _func = new ProcessIncomingEmail(_loggerMock.Object, _configMock.Object, _sendGridRepoMock.Object);
        }       

        [Fact]
        public async Task ReturnOkResultWhenSuccessful()
        {
            // ARRANGE
            var mockedOutgoingMessageRequestBody = new OutgoingEmailMessage
            {
                SenderName = "TestName",
                SenderEmail = "testuser@mail.com",
                EmailSubject = "TestEmail",
                EmailBody = "This is a test email"
            };

            var mockedResponse = new Response(System.Net.HttpStatusCode.OK, It.IsAny<HttpContent>(), It.IsAny<HttpResponseHeaders>());

            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(mockedOutgoingMessageRequestBody));
            MemoryStream stream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(stream);
            _sendGridRepoMock.Setup(x => x.SendEmail(It.IsAny<SendGridMessage>()))
                .ReturnsAsync(mockedResponse);

            // ACT
            var response = await _func.Run(_httpRequestMock.Object);

            // ASSERT
            Assert.Equal(typeof(OkResult), response.GetType());
        }

        [Fact]
        public async Task ThrowBadRequestWhenInvalid()
        {
            // ARRANGE
            var mockedOutgoingMessageRequestBody = new OutgoingEmailMessage
            {
                SenderName = "TestName",
                SenderEmail = null,
                EmailSubject = "TestEmail",
                EmailBody = "This is a test email"
            };

            var mockedResponse = new Response(System.Net.HttpStatusCode.BadRequest, It.IsAny<HttpContent>(), It.IsAny<HttpResponseHeaders>());

            byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(mockedOutgoingMessageRequestBody));
            MemoryStream stream = new MemoryStream(byteArray);
            _httpRequestMock.Setup(r => r.Body).Returns(stream);
            _sendGridRepoMock.Setup(x => x.SendEmail(It.IsAny<SendGridMessage>()))
                .ReturnsAsync(mockedResponse);

            // ACT
            var response = await _func.Run(_httpRequestMock.Object);

            // ASSERT
            Assert.Equal(typeof(BadRequestResult), response.GetType());
        }

        [Fact]
        public async Task ReturnInternalServerError()
        {
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
            _sendGridRepoMock.Setup(x => x.SendEmail(It.IsAny<SendGridMessage>()))
                .ThrowsAsync(It.IsAny<Exception>());

            // ACT
            var response = await _func.Run(_httpRequestMock.Object);

            // ASSERT
            Assert.Equal(typeof(StatusCodeResult), response.GetType());
            var responseStatusCode = (StatusCodeResult)response;
            Assert.Equal(500, responseStatusCode.StatusCode);
        }
    }
}
