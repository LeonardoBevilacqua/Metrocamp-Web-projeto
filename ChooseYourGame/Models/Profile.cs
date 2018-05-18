using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ChooseYourGame.Models
{
    public class Profile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Picture { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(75)]
        public string Lastname { get; set; }

        public string Background { get; set; }
        
        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public IEnumerable<Blog> Blogs { get; set; }

    }
}