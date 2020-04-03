﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NHSD.BuyingCatalogue.Identity.Api.Models;
using NHSD.BuyingCatalogue.Identity.Api.Settings;
using NHSD.BuyingCatalogue.Identity.Common.Email;

namespace NHSD.BuyingCatalogue.Identity.Api.Services
{
    /// <summary>
    /// Provides password services.
    /// </summary>
    internal sealed class PasswordService : IPasswordService
    {
        private readonly IEmailService _emailService;
        private readonly PasswordResetSettings _settings;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordService"/> class using
        /// the provided <paramref name="emailService"/> and <paramref name="settings"/>.
        /// </summary>
        /// <param name="emailService">The service to use to send e-mails.</param>
        /// <param name="settings">The configured password reset settings.</param>
        /// <param name="userManager">The Identity framework user manager.</param>
        public PasswordService(
            IEmailService emailService,
            PasswordResetSettings settings,
            UserManager<ApplicationUser> userManager)
        {
            _emailService = emailService;
            _settings = settings;
            _userManager = userManager;
        }

        /// <summary>
        /// Generates a password reset token for the user with the
        /// provided <paramref name="emailAddress"/>.
        /// </summary>
        /// <param name="emailAddress">The e-mail of the user to generate the password
        /// reset token for.</param>
        /// <returns>A <see cref="PasswordResetToken"/> if the user was found;
        /// otherwise, <see lagref="null"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="emailAddress"/> is <see langref="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="emailAddress"/> is empty or white space.</exception>
        public async Task<PasswordResetToken> GeneratePasswordResetTokenAsync(string emailAddress)
        {
            if (emailAddress is null)
                throw new ArgumentNullException(nameof(emailAddress));

            if (string.IsNullOrWhiteSpace(emailAddress))
                throw new ArgumentException($"{emailAddress} must be provided", nameof(emailAddress));

            var user = await _userManager.FindByEmailAsync(emailAddress);

            return user == null
                ? null
                : new PasswordResetToken(await _userManager.GeneratePasswordResetTokenAsync(user), user);
        }

        /// <summary>
        /// Sends a password reset e-mail to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to send the e-mail to.</param>
        /// <param name="callback">The callback URL to handle
        /// the password reset.</param>
        /// <returns>An asynchronous task context.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="user"/> is <see langref="null"/>.</exception>
        public async Task SendResetEmailAsync(ApplicationUser user, string callback)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            var message = new EmailMessage(_settings.EmailMessage, new Uri(callback))
            {
                Recipient = new EmailAddress(user.DisplayName, user.Email),
            };

            await _emailService.SendEmailAsync(message);
        }
    }
}
