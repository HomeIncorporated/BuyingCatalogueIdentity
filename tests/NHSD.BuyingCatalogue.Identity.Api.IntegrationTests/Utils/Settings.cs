﻿using Microsoft.Extensions.Configuration;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils
{
    public sealed class Settings
    {
        public string AdminConnectionString { get; }

        public string ConnectionString { get; }

        public string OrganisationApiBaseUrl { get; }

        public SmtpServer Smtp { get; }

        public Settings(IConfigurationRoot config)
        {
            AdminConnectionString = config.GetConnectionString("CatalogueUsersAdmin");
            ConnectionString = config.GetConnectionString("CatalogueUsers");
            OrganisationApiBaseUrl = config.GetValue<string>("OrganisationApiBaseUrl");
            Smtp = new SmtpServer(config.GetSection("SmtpServer"));
        }

        public sealed class SmtpServer
        {
            public SmtpServer(IConfigurationSection config)
            {
                Host = config.GetValue<string>("Host");
                Port = config.GetValue<ushort>("Port");
                ApiBaseUrl = config.GetValue<string>("ApiBaseUrl");
            }

            public string Host { get; }

            public ushort Port { get; }

            public string ApiBaseUrl { get; }
        }
    }
}
