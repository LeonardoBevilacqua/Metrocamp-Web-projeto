using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChooseYourGame.Models
{
    public class Follower
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Profile))]
        public int FollowerProfileId { get; set; }

        public Profile FollowerProfile { get; set; }

        [Required]
        [ForeignKey(nameof(Profile))]
        public int FollowingProfileId { get; set; }

        public Profile FollowingProfile { get; set; }
    }
}