using System.ComponentModel.DataAnnotations;

namespace GloboconSoftwareWeb.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }
}
