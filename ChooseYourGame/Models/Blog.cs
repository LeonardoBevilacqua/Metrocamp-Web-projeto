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

        [Required(ErrorMessage="O campo Título é obrigatório")]
        [StringLength(100, ErrorMessage="Tamanho máximo de 100 caracteres")]
        public string Title { get; set; }

        [Required(ErrorMessage="O campo Assunto do blog é obrigatório")]
        public string Description { get; set; }

        [Required(ErrorMessage="O campo Blog é obrigatório")]
        public string BlogText { get; set; }

        public DateTime CreationTime { get; set; }

        public string ProfileUserId { get; set; }
        public Profile Profile { get; set; }

        public List<BlogTag> BlogTag { get; set; }

        public List<Commentary> Commentaries { get; set; }
    }
}