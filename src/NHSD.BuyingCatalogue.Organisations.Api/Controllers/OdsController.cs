﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSD.BuyingCatalogue.Organisations.Api.Models;
using NHSD.BuyingCatalogue.Organisations.Api.Repositories;
using NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations;

namespace NHSD.BuyingCatalogue.Organisations.Api.Controllers
{
    [Authorize(Policy = Policy.CanAccessOrganisations)]
    [Route("api/v1/ods")]
    [ApiController]
    [Produces("application/json")]
    public sealed class OdsController : Controller
    {
        private readonly IOdsRepository _odsRepository;

        public OdsController(IOdsRepository odsRepository)
        {
            _odsRepository = odsRepository;
        }

        [HttpGet]
        [Route("{odsCode}")]
        public async Task<ActionResult> GetByOdsCodeAsync(string odsCode)
        {
            if (odsCode is null)
                throw new ArgumentNullException(nameof(odsCode));

            if (string.IsNullOrWhiteSpace(odsCode))
                return NotFound();

            var odsOrganisation = await _odsRepository.GetBuyerOrganisationByOdsCode(odsCode);

            if (odsOrganisation is null)
                return NotFound();

            if (!(odsOrganisation.IsActive && odsOrganisation.IsBuyerOrganisation))
                return new StatusCodeResult(StatusCodes.Status406NotAcceptable);

            return Ok(new OdsOrganisationViewModel
            {
                OdsCode = odsOrganisation.OdsCode,
                OrganisationName = odsOrganisation.OrganisationName,
                PrimaryRoleId = odsOrganisation.PrimaryRoleId,
                Address = odsOrganisation.Address is null ? null : new AddressViewModel
                {
                    Line1 = odsOrganisation.Address.Line1,
                    Line2 = odsOrganisation.Address.Line2,
                    Line3 = odsOrganisation.Address.Line3,
                    Line4 = odsOrganisation.Address.Line4,
                    Town = odsOrganisation.Address.Town,
                    County = odsOrganisation.Address.County,
                    Postcode = odsOrganisation.Address.Postcode,
                    Country = odsOrganisation.Address.Country,
                }
            });
        }
    }
}
