﻿using System;
using System.Collections.Generic;
using Moq;
using NHSD.BuyingCatalogue.Organisations.Api.Controllers;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;

namespace NHSD.BuyingCatalogue.Organisations.Api.UnitTests.TestContexts
{
    internal sealed class UsersControllerTestContext
    {
        private UsersControllerTestContext()
        {
        	CreateBuyerServiceMock = new Mock<ICreateBuyerService>();
            CreateBuyerServiceMock.Setup(x => x.CreateAsync(It.IsAny<CreateBuyerRequest>()))
                .ReturnsAsync(() => CreateBuyerResult);
        	
            RegistrationServiceMock = new Mock<IRegistrationService>();
            UsersRepositoryMock = new Mock<IUsersRepository>();

            Users = new List<ApplicationUser>();
            UsersRepositoryMock.Setup(x => x.GetUsersByOrganisationIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => Users);

            UsersRepositoryMock.Setup(x => x.CreateUserAsync(It.IsAny<ApplicationUser>()));

            Controller = new UsersController(CreateBuyerServiceMock.Object, UsersRepositoryMock.Object, RegistrationServiceMock.Object);
        }

		public Mock<ICreateBuyerService> CreateBuyerServiceMock { get; set; }

		public Result CreateBuyerResult { get; set; } = Result.Success();

        public Mock<IUsersRepository> UsersRepositoryMock { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }

        public UsersController Controller { get; set; }

        internal Mock<IRegistrationService> RegistrationServiceMock { get; set; }

        internal static UsersControllerTestContext Setup()
        {
            return new UsersControllerTestContext();
        }
    }
}
