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
        public string FollowerProfileUserId { get; set; }

        public Profile FollowerProfile { get; set; }

        [Required]
        [ForeignKey(nameof(Profile))]
        public string FollowingProfileUserId { get; set; }

        public Profile FollowingProfile { get; set; }
    }
}