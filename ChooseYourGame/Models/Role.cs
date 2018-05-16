// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;

// namespace ChooseYourGame.Models
// {
//     public class Role
//     {
//         [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//         public int Id { get; set; }

//         [Required]
//         [StringLength(100)]
//         public string Description { get; set; }

//         public virtual ICollection<UserRole> UserRoles {get; set; }
//     }
// }