using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using Portfolio.Contact.Mappers;
using Portfolio.Contact.Models;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Portfolio.Contact.Tests.MapperTests
{
    public class SendGridMessageMapperShould
    {
        Mock<IConfiguration> _configMock;

        private SendGridMessageMapper _mapperMock;

        public SendGridMessageMapperShould()
        {
            _configMock = new Mock<IConfiguration>();
            _configMock.Setup(x => x["RecipientEmail"]).Returns("test@test.com");

            _mapperMock = new SendGridMessageMapper(_configMock.Object);
        }

        [Theory]
        [InlineData("Will Velida", "willvelida@test.com", "test subject", "test body")]
        [InlineData("Steve Smith", "stevesmith@test.com", "Hi there!", "test body")]
        [InlineData("Claire Jones", "clairejones@test.com", "What's up?!", "test body")]
        [InlineData("Daisy Harris", "daisyharris@test.com", "Requesting service", "test body")]
        public void MapRequestCorrectly(string senderName, string senderEmail, string emailSubject, string emailBody)
        {
            // ARRANGE
            var testInput = new OutgoingEmailMessage
            {
                SenderName = senderName,
                SenderEmail = senderEmail,
                EmailSubject = emailSubject,
                EmailBody = emailBody
            };

            string testRequest = JsonConvert.SerializeObject(testInput);

            // ACT
            var response = _mapperMock.MapRequestToMessage(testRequest);

            // ASSERT
            Assert.Equal(new EmailAddress(testInput.SenderEmail, testInput.SenderName), response.From);
            Assert.Equal(testInput.EmailSubject, response.Subject);
            Assert.Equal(testInput.EmailBody, response.PlainTextContent);
            Assert.Equal(testInput.EmailBody, response.HtmlContent);
        }
    }
}
