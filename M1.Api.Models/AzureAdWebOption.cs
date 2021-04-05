namespace M1.Api.Models
{
    public class AzureAdWebOption
    {
        public string Instance { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ReturnUrlParameter { get; set; }
    }
}
