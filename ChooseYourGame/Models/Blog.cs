using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChooseYourGame.Models
{
    public class Blog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string BlogText { get; set; }

        public DateTime CreationTime { get; set; }

        public string ProfileUserId { get; set; }
        public Profile Profile { get; set; }

        public List<BlogTag> BlogTag { get; set; }

        public List<Commentary> Commentaries { get; set; }
    }
}