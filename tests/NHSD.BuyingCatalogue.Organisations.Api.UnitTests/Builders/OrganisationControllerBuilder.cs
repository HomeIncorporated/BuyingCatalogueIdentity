﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Identity.Common.Models;
using NHSD.BuyingCatalogue.Identity.Common.Results;
using NHSD.BuyingCatalogue.Organisations.Api.Controllers;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.Services;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders
{
    internal sealed class OrganisationControllerBuilder
    {
        private IOrganisationRepository _organisationRepository;
        private ICreateOrganisationService _createOrganisationService;
        private IServiceRecipientRepository _serviceRecipientRepository;

        private readonly ClaimsPrincipal _claimsPrincipal;

        private OrganisationControllerBuilder(Guid primaryOrganisationId)
        {
            _createOrganisationService = Mock.Of<ICreateOrganisationService>();
            _organisationRepository = Mock.Of<IOrganisationRepository>();
            _serviceRecipientRepository = Mock.Of<IServiceRecipientRepository>();

            _claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("primaryOrganisationId", primaryOrganisationId.ToString())
            },
            "mock"));
        }

        internal static OrganisationControllerBuilder Create(Guid primaryOrganisationId = default)
        {
            return new OrganisationControllerBuilder(primaryOrganisationId);
        }

        internal OrganisationControllerBuilder WithServiceRecipients(IEnumerable<ServiceRecipient> result)
        {
            var mockServiceRecipientsRepository = new Mock<IServiceRecipientRepository>();
            mockServiceRecipientsRepository.Setup(x => x.GetServiceRecipientsByParentOdsCode(It.IsAny<string>()))
                .ReturnsAsync(result);

            _serviceRecipientRepository = mockServiceRecipientsRepository.Object;
            return this;
        }

        internal OrganisationControllerBuilder WithListOrganisation(IEnumerable<Organisation> result)
        {
            var mockListOrganisation = new Mock<IOrganisationRepository>();
            mockListOrganisation.Setup(x => x.ListOrganisationsAsync()).ReturnsAsync(result);

            _organisationRepository = mockListOrganisation.Object;
            return this;
        }

        internal OrganisationControllerBuilder WithGetOrganisation(Organisation result)
        {
            var mockGetOrganisation = new Mock<IOrganisationRepository>();
            mockGetOrganisation.Setup(x => x.GetByIdAsync(result.OrganisationId)).ReturnsAsync(result);

            _organisationRepository = mockGetOrganisation.Object;
            return this;
        }

        internal OrganisationControllerBuilder WithUpdateOrganisation(Organisation organisation)
        {
            var repositoryMock = new Mock<IOrganisationRepository>();
            repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(organisation);
            repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Organisation>()));
            _organisationRepository = repositoryMock.Object;
            return this;
        }

        internal OrganisationControllerBuilder WithOrganisationRepository(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
            return this;
        }

        internal OrganisationControllerBuilder WithCreateOrganisationServiceReturningSuccess(Guid? result)
        {
            return WithCreateOrganisationServiceReturningResult(Result.Success(result));
        }

        internal OrganisationControllerBuilder WithCreateOrganisationServiceReturningFailure(string result)
        {
            return WithCreateOrganisationServiceReturningResult(Result.Failure<Guid?>(new ErrorDetails(result)));
        }

        private OrganisationControllerBuilder WithCreateOrganisationServiceReturningResult(Result<Guid?> result)
        {
            WithCreateOrganisation();
            var createOrganisationService = new Mock<ICreateOrganisationService>();
            createOrganisationService.Setup(s => s.CreateAsync(It.IsAny<CreateOrganisationRequest>()))
                .ReturnsAsync(result);
            _createOrganisationService = createOrganisationService.Object;
            return this;
        }

        private void WithCreateOrganisation()
        {
            var repositoryMock = new Mock<IOrganisationRepository>();
            repositoryMock.Setup(x => x.CreateOrganisationAsync(It.IsAny<Organisation>()));
            _organisationRepository = repositoryMock.Object;
        }

        internal OrganisationsController Build()
        {
            return new OrganisationsController(_organisationRepository, _createOrganisationService, _serviceRecipientRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = _claimsPrincipal }
                }
            };
        }
    }
}
