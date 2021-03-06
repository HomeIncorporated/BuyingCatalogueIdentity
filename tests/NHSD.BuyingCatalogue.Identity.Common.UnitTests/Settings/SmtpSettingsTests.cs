﻿using FluentAssertions;
using NHSD.BuyingCatalogue.Identity.Common.Settings;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Identity.Common.UnitTests.Settings
{
    [TestFixture]
    internal sealed class SmtpSettingsTests
    {
        [Test]
        public void SmtpSettings_AuthenticationSettings_IsInitialized()
        {
            var settings = new SmtpSettings();

            settings.Authentication.Should().NotBeNull();
        }
    }
}
