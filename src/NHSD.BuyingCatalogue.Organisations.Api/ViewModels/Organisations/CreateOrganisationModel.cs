namespace NHSD.BuyingCatalogue.Organisations.Api.ViewModels.Organisations
{
    public sealed class CreateOrganisationModel
    {
        public string OrganisationName { get; set; }

        public string OdsCode { get; set; }

        public string PrimaryRoleId { get; set; }

        public AddressModel Address { get; set; }

        public bool CatalogueAgreementSigned { get; set; }
    }
}
