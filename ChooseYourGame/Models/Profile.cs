using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChooseYourGame.Models
{
    public class Profile
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        public string Picture { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(75)]
        public string Lastname { get; set; }

        public string Background { get; set; }

        public User User { get; set; }

    }
}