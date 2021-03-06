﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Controllers
{

    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class OdsControllerTests
    {
        [Test]
        public async Task GetByOdsCodeAsync_OrganisationDoesNotExist_ReturnsNotFound()
        {
            using var controller = OdsControllerBuilder
                .Create()
                .WithGetByOdsCode(null)
                .Build();

            var result = await controller.GetByOdsCodeAsync(string.Empty);

            result.Should().BeEquivalentTo(new NotFoundResult());
        }

        [Test]
        public async Task GetByOdsCodeAsync_OrganisationIsNotBuyerOrganisation_ReturnsNotAccepted()
        {
            var nonBuyerOrganisation = OdsOrganisationBuilder.Create(1).Build();
            using var controller = OdsControllerBuilder
                .Create()
                .WithGetByOdsCode(nonBuyerOrganisation)
                .Build();

            var response = await controller.GetByOdsCodeAsync("dolor eternum");

            response.Should().BeOfType<StatusCodeResult>();

            response.Should().BeEquivalentTo(new StatusCodeResult(StatusCodes.Status406NotAcceptable));
        }

        [Test]
        public async Task GetByOdsCodeAsync_OrganisationExists_ReturnsActiveBuyerOrganisation()
        {
            var buyerOrganisation = OdsOrganisationBuilder.Create(1, true).Build();

            using var controller = OdsControllerBuilder
                .Create()
                .WithGetByOdsCode(buyerOrganisation)
                .Build();

            var response = await controller.GetByOdsCodeAsync(buyerOrganisation.OdsCode);

            response.Should().BeOfType<OkObjectResult>();

            var result = response as OkObjectResult;

            result.Value.Should().BeEquivalentTo(buyerOrganisation, options => options.Excluding(o => o.IsActive).Excluding(o => o.IsBuyerOrganisation));
        }

        [Test]
        public void GetByOdsCodeAsync_NullOdsCode_ThrowsArgumentNullException()
        {
            static async Task GetOrganisationByNullOdsCode()
            {
                using var controller = OdsControllerBuilder.Create().Build();
                await controller.GetByOdsCodeAsync(null);
            }
            Assert.ThrowsAsync<ArgumentNullException>(GetOrganisationByNullOdsCode);
        }

        [Test]
        public async Task GetByOdsCodeAsync_VerifyMethodIsCalledOnce_VerifiesMethod()
        {
            const string odsCode = "123";

            var odsRepositoryMock = new Mock<IOdsRepository>();
            odsRepositoryMock.Setup(x => x.GetBuyerOrganisationByOdsCodeAsync(It.IsAny<string>()))
                .ReturnsAsync((OdsOrganisation)null);

            using var controller = OdsControllerBuilder.Create()
                .WithOdsRepository(odsRepositoryMock.Object)
                .Build();

            await controller.GetByOdsCodeAsync(odsCode);

            odsRepositoryMock.Verify(x => x.GetBuyerOrganisationByOdsCodeAsync(odsCode), Times.Once);
        }
    }
}
