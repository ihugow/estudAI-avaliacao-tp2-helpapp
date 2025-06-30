using System.ComponentModel.DataAnnotations;

namespace StockApp.Application.DTOs
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo {1} caracteres")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "A senha deve ter entre {2} e {1} caracteres")]
        public string Password { get; set; }
    }
}