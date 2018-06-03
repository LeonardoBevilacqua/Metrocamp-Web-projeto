using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ChooseYourGame.Models.ViewModels
{
    public class ConfigViewModel
    {
        [Required, StringLength(50)]
        public string Name { get; set; }

        [StringLength(75)]
        public string Lastname { get; set; }

        [Required, EmailAddress]
        public string EMail { get; set; }

        public IFormFile Picture { get; set; }
        public IFormFile Background { get; set; }

        [Required, DataType("password")]
        public string CurrentPassword { get; set; }

        [DataType("password")]
        public string NewPassword { get; set; }

        [DataType("password")]
        public string NewPassword_Confirmation { get; set; }
    }
}