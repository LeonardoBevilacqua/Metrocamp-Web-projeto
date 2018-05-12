using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChooseYourGame.Models
{
    public class Commentary
    {
        public int Id { get; set; }

        [Required]    
        [StringLength(255)]
        public string CommentaryText{ get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }
    }
}