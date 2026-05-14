using System.ComponentModel.DataAnnotations;

namespace Application_Educative.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Veuillez saisir votre nom d'utilisateur")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Veuillez saisir votre mot de passe")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

