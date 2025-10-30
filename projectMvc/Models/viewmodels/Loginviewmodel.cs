using System.ComponentModel.DataAnnotations;

namespace projectMvc.Models.viewmodels
{
    public class Loginviewmodel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
