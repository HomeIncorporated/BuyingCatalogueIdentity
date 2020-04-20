﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Organisations.Api.Data;
using NHSD.BuyingCatalogue.Organisations.Api.Models;

namespace NHSD.BuyingCatalogue.Organisations.Api.Repositories
{
    public sealed class OrganisationRepository : IOrganisationRepository
    {
        private readonly ApplicationDbContext _context;

        public OrganisationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Organisation>> ListOrganisationsAsync()
        {
            return await _context.Organisations.OrderBy(c=>c.Name).ToListAsync();
        }

        public async Task<Organisation> GetByIdAsync(Guid id)
        {
            return await _context.Organisations.FirstOrDefaultAsync(org => org.OrganisationId == id);
        }

        public async Task<Organisation> GetByNameAsync(string name)
        {
            return await _context.Organisations.FirstOrDefaultAsync(org => org.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task CreateOrganisationAsync(Organisation organisation)
        {
            if (organisation is null)
                throw new ArgumentNullException(nameof(organisation));

            _context.Organisations.Add(organisation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Organisation organisation)
        {
            if (organisation is null) 
                throw new ArgumentNullException(nameof(organisation));
            
            _context.Organisations.Update(organisation);
            await _context.SaveChangesAsync();
        }
    }
}
