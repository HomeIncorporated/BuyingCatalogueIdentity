﻿using System;
using HealthChecks.Network.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NHSD.BuyingCatalogue.Identity.Common.Constants;
using NHSD.BuyingCatalogue.Identity.Common.Settings;

namespace NHSD.BuyingCatalogue.Identity.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterHealthChecks(this IServiceCollection services, string connectionString, SmtpSettings? smtpSettings = null)
        {
            if (connectionString is null)
                throw new ArgumentNullException(nameof(connectionString));

            var healthChecksBuilder = services.AddHealthChecks();

            healthChecksBuilder.AddCheck(
                    "self",
                    () => HealthCheckResult.Healthy(),
                    new[] { HealthCheckTags.Live })
                .AddSqlServer(
                    connectionString,
                    "SELECT 1;",
                    "db",
                    HealthStatus.Unhealthy,
                    new[] { HealthCheckTags.Ready },
                    TimeSpan.FromSeconds(10));

            if (smtpSettings != null)
            {
                healthChecksBuilder.AddSmtpHealthCheck(
                    smtp =>
                    {
                        smtp.Host = smtpSettings.Host;
                        smtp.Port = smtpSettings.Port;
                        smtp.ConnectionType = SmtpConnectionType.TLS;
                    },
                    "smtp",
                    HealthStatus.Degraded,
                    new[] { HealthCheckTags.Ready },
                    TimeSpan.FromSeconds(10));
            }

            return services;
        }
    }
}
