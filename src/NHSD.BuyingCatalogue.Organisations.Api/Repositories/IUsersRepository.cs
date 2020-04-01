﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.Repositories
{
    public interface IUsersRepository
    {
        Task<IEnumerable<ApplicationUser>> GetUsersByOrganisationIdAsync(Guid organisationId);

        Task CreateUserAsync(ApplicationUser user);

        Task<ApplicationUser> GetUserById(string userId);
    }
}
