﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.OrganisationUsers;

namespace NHSD.BuyingCatalogue.Organisations.Api.Controllers
{
    [ApiController]
    [Route("api/v1/Organisations/{organisationId}/Users")]
    [Produces("application/json")]
    public sealed class UsersController : Controller
    {
        private static readonly IList<OrganisationUserViewModel> _users = new List<OrganisationUserViewModel>
        {
            new OrganisationUserViewModel
            {
                UserId = "1234-56789",
                Name = "John Smith",
                PhoneNumber = "01234567890",
                EmailAddress = "a.b@c.com",
                IsDisabled = false,
                OrganisationId = new Guid("FFE7CB2F-9494-4CC7-A348-420D502956D9")
            },
            new OrganisationUserViewModel
            {
                UserId = "9876-54321",
                Name = "Benny Hill",
                PhoneNumber = "09876543210",
                EmailAddress = "g.b@z.com",
                IsDisabled = true,
                OrganisationId = new Guid("FFE7CB2F-9494-4CC7-A348-420D502956D9")
            }
        };

        [HttpGet]
        public ActionResult GetUsersById(Guid organisationId)
        {
            return Ok(new GetAllOrganisationUsersViewModel
            {
                Users = _users.Where(user => organisationId.Equals(user.OrganisationId))
            });
        }

        [HttpPost]
        public ActionResult CreateUser(Guid organisationId, OrganisationUserViewModel userViewModel)
        {
            userViewModel.OrganisationId = organisationId;

            _users.Add(userViewModel);

            return Ok();
        }
    }
}
