using System.ComponentModel.DataAnnotations;

namespace M1.Api.Models
{
    public class EncryptRequest
    {
        [Required]
        public string PlainText { get; set; }
    }
}
