using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Portfolio.Contact.Functions;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Portfolio.Contact.Tests.FunctionTests
{
    public class ProcessIncomingEmailTests
    {
        Mock<ILogger<ProcessIncomingEmail>> _loggerMock;
        Mock<IConfiguration> _configMock;
        Mock<SendGridClient> _sendGridClientMock;

        private ProcessIncomingEmail _func;

        public ProcessIncomingEmailTests()
        {
            _loggerMock = new Mock<ILogger<ProcessIncomingEmail>>();
            _configMock = new Mock<IConfiguration>();
            _sendGridClientMock = new Mock<SendGridClient>();

            _func = new ProcessIncomingEmail(
                _loggerMock.Object,
                _configMock.Object,
                _sendGridClientMock.Object);
        }

        [Fact]
        public async Task ProcessIncomingEmail_happy_path()
        {

        }

        [Fact]
        public async Task ProcessIncomingEmail_bad_request()
        {

        }

        [Fact]
        public async Task ProcessIncomingEmail_exception_thrown()
        {

        }
    }
}
