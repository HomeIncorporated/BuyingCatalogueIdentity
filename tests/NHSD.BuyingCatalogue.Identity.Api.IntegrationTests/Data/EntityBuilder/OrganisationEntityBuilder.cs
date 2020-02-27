﻿using System;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Identity.Api.Data;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Data.Entities;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Data.EntityBuilder
{
    public sealed class OrganisationEntityBuilder
    {
        private readonly ApplicationDbContext _context;
        private readonly OrganisationEntity _organisationEntity;

        public static OrganisationEntityBuilder Create()
        {
            return new OrganisationEntityBuilder();
        }

        public OrganisationEntityBuilder()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=CatalogueUsers;MultipleActiveResultSets=true;User ID=NHSD;Password=DisruptTheMarket1!");
            _context = new ApplicationDbContext(optionsBuilder.Options);

            _organisationEntity = new OrganisationEntity()
            {
                Id = new Guid(),
                Name = "Organisation Name",
                OdsCode = "Ods Code",
                LastUpdated = DateTime.Now
            };
        }

        public void Insert()
        {
            var organisation = new Organisation(Guid.NewGuid(), _organisationEntity.Name, _organisationEntity.OdsCode);

            _context.Organisations.Add(organisation);

            // Issue: Not updating the database, wrong credentials
            _context.SaveChanges();
        }

        public OrganisationEntityBuilder WithId(Guid id)
        {
            _organisationEntity.Id = id;
            return this;
        }

        public OrganisationEntityBuilder WithName(string name)
        {
            _organisationEntity.Name = name;
            return this;
        }

        public OrganisationEntityBuilder WithOdsCode(string odsCode)
        {
            _organisationEntity.OdsCode = odsCode;
            return this;
        }

        public OrganisationEntityBuilder WithLastUpdated(DateTime lastUpdated)
        {
            _organisationEntity.LastUpdated = lastUpdated;
            return this;
        }
    }
}
