﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.Services
{
    public interface ICreateBuyerService
    {
        Task<Result> CreateAsync(CreateBuyerRequest createBuyerRequest);
    }
}
