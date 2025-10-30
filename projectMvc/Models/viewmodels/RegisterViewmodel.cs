using System.ComponentModel.DataAnnotations;
namespace projectMvc.Models.viewmodels
{
    public class RegisterViewmodel
    {
        [Required]
        public string? Name { get; set; }
       
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Required]
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string? Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }
        
    }
}
