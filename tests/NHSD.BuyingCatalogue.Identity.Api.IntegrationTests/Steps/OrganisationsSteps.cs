﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel.Client;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps.Common;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Utils;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.EntityBuilder;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Steps
{
    [Binding]
    public sealed class OrganisationsSteps
    {
        private readonly ScenarioContext _context;
        private readonly Response _response;
        private readonly Settings _settings;

        private readonly string _organisationUrl;

        private const string AccessTokenKey = "AccessToken";
        private const string OrganisationMapDictionary = "OrganisationMapDictionary";

        public OrganisationsSteps(ScenarioContext context, Response response, Settings settings)
        {
            _context = context;
            _response = response;
            _settings = settings;

            _organisationUrl = _settings.OrganisationApiBaseUrl + "/api/v1/Organisations";
        }

        [Given(@"Organisations exist")]
        public async Task GivenOrganisationsExist(Table table)
        {
            IDictionary<string, Guid> organisationDictionary = new Dictionary<string, Guid>();

            foreach (var organisationTableItem in table.CreateSet<OrganisationTable>())
            {
                var organisation = OrganisationEntityBuilder
                    .Create()
                    .WithName(organisationTableItem.Name)
                    .WithOdsCode(organisationTableItem.OdsCode)
                    .Build();

                await organisation.InsertAsync(_settings.ConnectionString);
                organisationDictionary.Add(organisation.Name, organisation.Id);
            }

            _context[OrganisationMapDictionary] = organisationDictionary;
        }

        [When(@"a GET request is made for the Organisations section")]
        public async Task WhenAGETRequestIsMadeForTheOrganisationsSection()
        {
            string bearerToken = _context.Get(AccessTokenKey, "");

            using var client = new HttpClient();
            client.SetBearerToken(bearerToken);

            _response.Result = await client.GetAsync(new Uri(_organisationUrl));
        }

        [Then(@"the Organisations list is returned with the following values")]
        public async Task ThenTheOrganisationsListIsReturnedWithTheFollowingValues(Table table)
        {
            var expectedOrganisations = table.CreateSet<OrganisationTable>().ToList();

            var organisations = (await _response.ReadBody()).SelectToken("organisations").Select(t => new
            {
                Name = t.SelectToken("name").ToString(), OdsCode = t.SelectToken("odsCode").ToString()
            });

            organisations.Should().BeEquivalentTo(expectedOrganisations, options => options.WithStrictOrdering());
        }

        [When(@"a GET request is made for an organisation with name (.*)")]
        public async Task WhenAGETRequestIsMadeForAnOrganisationWithNameOrganisation(string organisationName)
        {
            var allOrganisations = _context.Get<IDictionary<string, Guid>>(OrganisationMapDictionary);

            var organisationId = Guid.Empty.ToString();
            if (allOrganisations.ContainsKey(organisationName))
            {
                organisationId = allOrganisations?[organisationName].ToString();
            }

            using var client = new HttpClient();
            client.SetBearerToken(_context.Get(AccessTokenKey, ""));
            _response.Result = await client.GetAsync(new Uri($"{_organisationUrl}/{organisationId}"));
        }

        [Given(@"an Organisation with name (.*) does not exist")]
        public async Task GivenAnOrganisationWithNameOrganisationDoesNotExist(string organisationName)
        {
            var organisations = await OrganisationEntity.GetByNameAsync(_settings.ConnectionString, organisationName);
            organisations.Should().BeNull();
        }

        private class OrganisationTable
        {
            public string Name { get; set; }

            public string OdsCode { get; set; }
        }
    }
}
