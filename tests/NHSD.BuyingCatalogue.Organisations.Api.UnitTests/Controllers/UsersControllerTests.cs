﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.BuyingCatalogue.Identity.Common.Models;
using NHSD.BuyingCatalogue.Identity.Common.Models.Results;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Builders;
using NHSD.BuyingCatalogue.Organisations.Api.UnitTests.TestContexts;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Messages;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Users;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public sealed class UsersControllerTests
    {
        [Test]
        public async Task GetUsersByOrganisationId_ReturnsOkObjectResult()
        {
            var context = UsersControllerTestContext.Setup();

            using var controller = context.Controller;

            var result = await controller.GetUsersByOrganisationId(Guid.Empty);
            result.Should().BeOfType<OkObjectResult>();
            (result as OkObjectResult).Value.Should().BeOfType<GetAllOrganisationUsersViewModel>();
        }

        [Test]
        public async Task GetUsersByOrganisationId_NoUsers_ReturnsEmptyList()
        {
            var context = UsersControllerTestContext.Setup();

            using var controller = context.Controller;

            var result = await controller.GetUsersByOrganisationId(Guid.Empty) as OkObjectResult;
            var users = result.Value as GetAllOrganisationUsersViewModel;
            users.Users.Should().BeEmpty();
        }

        [Test]
        public async Task GetUsersByOrganisationId_ValidId_ReturnsUsers()
        {
            var users = new List<(ApplicationUser RepoUser, OrganisationUserViewModel Expected)>
            {
                CreateApplicationUserTestData(false),
                CreateApplicationUserTestData(true),
                CreateApplicationUserTestData(false)
            };

            var context = UsersControllerTestContext.Setup();
            context.Users = users.Select(x => x.RepoUser);

            using var controller = context.Controller;

            var result = await controller.GetUsersByOrganisationId(Guid.Empty) as OkObjectResult;
            var viewModel = result.Value as GetAllOrganisationUsersViewModel;

            viewModel.Users.Should().BeEquivalentTo(users.Select(x => x.Expected));
        }

        [Test]
        public async Task GetUsersByOrganisationId_UserRepository_GetUsersByOrganisationIdAsync_CalledOnce()
        {
            var context = UsersControllerTestContext.Setup();

            using var controller = context.Controller;

            await controller.GetUsersByOrganisationId(Guid.Empty);

            context.UsersRepositoryMock.Verify(x => x.GetUsersByOrganisationIdAsync(Guid.Empty), Times.Once);
        }

        [Test]
        public async Task CreateBuyerAsync_CreateBuyerSuccessfulResult_ReturnsUserId()
        {
            const string newUserId = "New Test User Id";

            var context = UsersControllerTestContext.Setup();
            context.CreateBuyerResult = Result.Success(newUserId);

            using var controller = context.Controller;

            var response = await controller.CreateBuyerAsync(Guid.Empty, new CreateBuyerRequestViewModel());
            
            response.Should().BeOfType<ActionResult<CreateBuyerResponseViewModel>>();
            var actual = response.Result;

            actual.Should().BeEquivalentTo(new OkObjectResult(new CreateBuyerResponseViewModel { UserId = newUserId }));
        }

        [Test]
        public async Task CreateBuyerAsync_CreateBuyerService_CreateAsync_CalledOnce()
        {
            var context = UsersControllerTestContext.Setup();

            using var controller = context.Controller;

            var organisationId = Guid.NewGuid();
            var createUserRequestViewModel = new CreateBuyerRequestViewModel
            {
                FirstName = "Bob",
                LastName = "Smith",
                PhoneNumber = "98654321",
                EmailAddress = "bob@smith.com"
            };

            await controller.CreateBuyerAsync(organisationId, createUserRequestViewModel);

            var expected = CreateBuyerRequestBuilder
                .Create()
                .WithFirstName(createUserRequestViewModel.FirstName)
                .WithLastName(createUserRequestViewModel.LastName)
                .WithPhoneNumber(createUserRequestViewModel.PhoneNumber)
                .WithEmailAddress(createUserRequestViewModel.EmailAddress)
                .WithPrimaryOrganisationId(organisationId)
                .Build();

            context.CreateBuyerServiceMock.Verify(x => x.CreateAsync(expected), Times.Once);
        }

        [Test]
        public async Task CreateBuyerAsync_CreateBuyerFailureResult_ReturnsBadRequest()
        {
            var errors = new List<ErrorMessage> { new ErrorMessage("TestErrorId", "TestField") };

            var context = UsersControllerTestContext.Setup();
            context.CreateBuyerResult = Result.Failure<string>(errors);

            var organisationId = Guid.NewGuid();
            var createUserRequestViewModel = new CreateBuyerRequestViewModel();

            var response = await context.Controller.CreateBuyerAsync(organisationId, createUserRequestViewModel);

            response.Should().BeOfType<ActionResult<CreateBuyerResponseViewModel>>();
            var actual = response.Result;

            var expectedErrors = new List<ErrorMessageViewModel> { new ErrorMessageViewModel { Id = "TestErrorId", Field = "TestField" } };
            var expected = new BadRequestObjectResult(new CreateBuyerResponseViewModel { Errors = expectedErrors});
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void CreateBuyerAsync_NullApplicationUser_ThrowsException()
        {
            var context = UsersControllerTestContext.Setup();

            async Task<ActionResult<CreateBuyerResponseViewModel>> CreateUser()
            {
                using var controller = context.Controller;
                return await controller.CreateBuyerAsync(Guid.Empty, null);
            }

            Assert.ThrowsAsync<ArgumentNullException>(CreateUser);
        }

        private static (ApplicationUser RepoUser, OrganisationUserViewModel ExpectedUser) CreateApplicationUserTestData(bool disabled)
        {
            var repositoryApplicationUser = ApplicationUserBuilder
                .Create()
                .WithFirstName(Guid.NewGuid().ToString())
                .WithLastName(Guid.NewGuid().ToString())
                .WithPhoneNumber(Guid.NewGuid().ToString())
                .WithEmailAddress(Guid.NewGuid().ToString())
                .WithDisabled(disabled)
                .BuildBuyer();

            return (
                RepoUser: repositoryApplicationUser,
                ExpectedUser: new OrganisationUserViewModel
                {
                    UserId = repositoryApplicationUser.Id,
                    EmailAddress = repositoryApplicationUser.Email,
                    PhoneNumber = repositoryApplicationUser.PhoneNumber,
                    FirstName = repositoryApplicationUser.FirstName,
                    LastName = repositoryApplicationUser.LastName,
                    IsDisabled = repositoryApplicationUser.Disabled
                }
            );
        }

        [Test]
        public async Task GetUserById_WithExistingUserId_ReturnsTheUser()
        {
            var context = UsersControllerTestContext.Setup();
            context.User = ApplicationUserBuilder.Create().BuildBuyer();

            var expected = new GetUser
            {
                Name = context.User.FirstName + " " + context.User.LastName,
                PhoneNumber = context.User.PhoneNumber,
                EmailAddress = context.User.Email,
                Disabled = context.User.Disabled,
                PrimaryOrganisationId = context.User.PrimaryOrganisationId
            };

            using var controller = context.Controller;

            var result = await controller.GetUserByIdAsync(context.User.Id);
            result.Result.Should().BeOfType<OkObjectResult>();
            var objectResult = result.Result as OkObjectResult;
            objectResult.Value.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetUserById_NoExistingUserId_ReturnsNotFound()
        {
            var context = UsersControllerTestContext.Setup();

            using var controller = context.Controller;

            var result = await controller.GetUserByIdAsync(string.Empty);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetUserById_UserRepository_GetUserByIdAsync_CalledOnce()
        {
            var context = UsersControllerTestContext.Setup();

            using var controller = context.Controller;

            await controller.GetUserByIdAsync(string.Empty);

            context.UsersRepositoryMock.Verify(x => x.GetUserByIdAsync(String.Empty), Times.Once);
        }

        [Test]
        public async Task EnableUserAsync_GetUserByIdAndEnableThem_ReturnsOk()
        {
            var context = UsersControllerTestContext.Setup();

            using var controller = context.Controller;

            context.User = ApplicationUserBuilder.Create().WithDisabled(true).BuildBuyer();

            var result = await controller.EnableUserAsync(context.User.Id);
            result.Should().BeOfType<NoContentResult>();

            context.User.Disabled.Should().BeFalse();
        }

        [Test]
        public async Task EnableUserAsync_UserIsNull_ReturnsNotFound()
        {
            var context = UsersControllerTestContext.Setup();

            using var controller = context.Controller;

            var result = await controller.EnableUserAsync("unknown");
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task EnableUserAsync_UserRepository_UpdateAsync_CalledOnce()
        {
            var context = UsersControllerTestContext.Setup();
            using var controller = context.Controller;

            context.User = ApplicationUserBuilder.Create().WithDisabled(true).BuildBuyer();

            await controller.EnableUserAsync(context.User.Id);

            context.UsersRepositoryMock.Verify(x => x.GetUserByIdAsync(context.User.Id), Times.Once);
            context.UsersRepositoryMock.Verify(x => x.UpdateAsync(context.User), Times.Once);
        }

        [Test]
        public async Task DisableUserAsync_GetUserByIdAndDisableThem_ReturnsOk()
        {
            var context = UsersControllerTestContext.Setup();

            using var controller = context.Controller;

            context.User = ApplicationUserBuilder.Create().WithDisabled(false).BuildBuyer();

            var result = await controller.DisableUserAsync(context.User.Id);
            result.Should().BeOfType<NoContentResult>();

            context.User.Disabled.Should().BeTrue();
        }

        [Test]
        public async Task DisableUserAsync_UserIsNull_ReturnsNotFound()
        {
            var context = UsersControllerTestContext.Setup();

            using var controller = context.Controller;

            var result = await controller.DisableUserAsync("unknown");
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task DisableUser_UserRepository_UpdateAsync_CalledOnce()
        {
            var context = UsersControllerTestContext.Setup();
            using var controller = context.Controller;

            context.User = ApplicationUserBuilder.Create().WithDisabled(false).BuildBuyer();

            await controller.DisableUserAsync(context.User.Id);

            context.UsersRepositoryMock.Verify(x => x.GetUserByIdAsync(context.User.Id), Times.Once);
            context.UsersRepositoryMock.Verify(x => x.UpdateAsync(context.User), Times.Once);
        }
    }
}
