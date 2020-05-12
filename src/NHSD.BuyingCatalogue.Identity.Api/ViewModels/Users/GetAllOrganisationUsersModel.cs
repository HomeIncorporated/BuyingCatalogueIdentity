using System.Collections.Generic;

namespace NHSD.BuyingCatalogue.Identity.Api.ViewModels.Users
{
    public sealed class GetAllOrganisationUsersModel
    {
        public IEnumerable<OrganisationUserViewModel> Users { get; set; }
    }
}
