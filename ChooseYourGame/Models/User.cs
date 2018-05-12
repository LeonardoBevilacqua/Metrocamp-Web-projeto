using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChooseYourGame.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string EMail { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        
        [Required]
        public int AccessId { get; set; }

        public Access Access { get; set; }
    }
}