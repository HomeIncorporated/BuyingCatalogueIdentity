﻿using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    public sealed class DatabaseSteps
    {
        private readonly Settings _settings;

        public DatabaseSteps(Settings settings)
        {
            _settings = settings;
        }

        [Given(@"the call to the database to set the field will fail")]
        public async Task GivenTheCallToTheDatabaseToSetTheFieldWillFail()
        {
            await Database.DropUser(_settings.AdminConnectionString);
        }
    }
}
