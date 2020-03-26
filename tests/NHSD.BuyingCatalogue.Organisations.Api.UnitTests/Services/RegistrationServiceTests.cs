﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Services;
using NHSD.BuyingCatalogue.Organisations.Api.Settings;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class RegistrationServiceTests
    {
        [Test]
        public async Task SendInitialEmailAsync_SendsEmail()
        {
            var inputMessage = new EmailMessage
            {
                Sender = new EmailAddress(),
                Recipient = new EmailAddress(),
                HtmlBody = "HTML",
                TextBody = "Text",
            };

            var settings = new RegistrationSettings { EmailMessage = inputMessage };
            var mockEmailService = new Mock<IEmailService>();

            var registrationService = new RegistrationService(mockEmailService.Object, settings);
            await registrationService.SendInitialEmailAsync(new ApplicationUser { Email = "ricardo@burton.com" });

            mockEmailService.Verify(e => e.SendEmailAsync(It.IsNotNull<EmailMessage>()), Times.Once());
        }

        [Test]
        public async Task SendInitialEmailAsync_SendsExpectedMessage()
        {
            var textBody = Guid.NewGuid().ToString();
            var inputMessage = new EmailMessage
            {
                Sender = new EmailAddress(),
                Recipient = new EmailAddress(),
                HtmlBody = "HTML",
                TextBody = textBody,
            };

            var settings = new RegistrationSettings { EmailMessage = inputMessage };

            EmailMessage actualMessage = null;

            var mockEmailService = new Mock<IEmailService>();
            mockEmailService.Setup(s => s.SendEmailAsync(It.IsNotNull<EmailMessage>()))
                .Callback<EmailMessage>(m => actualMessage = m);

            var user = new ApplicationUser
            {
                FirstName = "Uncle",
                LastName = "Bob",
                Email = "uncle@bob.com",
            };

            var registrationService = new RegistrationService(mockEmailService.Object, settings);
            await registrationService.SendInitialEmailAsync(user);

            actualMessage.TextBody.Should().Be(textBody);

            var recipient = actualMessage.Recipient;
            recipient.Should().NotBeNull();
            recipient.Name.Should().Be(user.DisplayName);
            recipient.Address.Should().Be(user.Email);
        }
    }
}
