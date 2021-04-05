using System;

namespace M1.Api.Models
{
    public class AzureAdApiOption
    {
        public string Instance { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string Audience { get; set; }
        public string AllowedScope { get; set; }
    }
}
