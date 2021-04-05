using System.ComponentModel.DataAnnotations;

namespace M1.Api.Models
{
    public class DecryptRequest
    {
        [Required]
        public string CipherText { get; set; }
    }
}
