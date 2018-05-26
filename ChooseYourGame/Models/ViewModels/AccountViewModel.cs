using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ChooseYourGame.Models.ViewModels
{
    public class AccountViewModel
    {
        public LogInViewModel Login { get; set; }
        public SignUpViewModel SignUp { get; set; }
    }

    public class SignUpViewModel
    {
        [Required(ErrorMessage = "O Campo Nome é obrigatório."), Display(Name = "Nome")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O Campo Sobrenome é obrigatório."), Display(Name = "Sobrenome")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "O Campo E-Mail é obrigatório."), EmailAddress(ErrorMessage = "O E-Mail não é valido.")]
        public string EMail { get; set; }

        [Required(ErrorMessage = "O Campo Nome de Usuário é obrigatório.")]
        public string UserName { get; set; }

        [
            Required(ErrorMessage = "O Campo Senha é obrigatório."),
            MinLength(6, ErrorMessage = "Senha deve ter o tamanho minimo de 6 caracteres."),
            MaxLength(50, ErrorMessage = "Senha deve ter o tamanho maximo de 50 caracteres."),
            DataType(DataType.Password)
        ]
        public string Password { get; set; }

        [
            Required(ErrorMessage = "O Campo Confirmar Senha é obrigatório."),
            DataType(DataType.Password),
            Compare("Password", ErrorMessage = "As senhas não são iguais.")
        ]
        public string Password_Confirmation { get; set; }
    }

    public class LogInViewModel
    {
        [Required]
        public string LoginAccount { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }
    }
}