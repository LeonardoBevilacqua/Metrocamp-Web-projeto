using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChooseYourGame.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public int Id { get; set; }

        [Required, EmailAddress, MaxLength(255)]
        public string EMail { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}