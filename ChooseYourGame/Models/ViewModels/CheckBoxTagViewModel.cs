using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ChooseYourGame.Models.ViewModels
{
    public class CheckBoxTagViewModel : Tag
    {
        public bool IsSelected { get; set; }

        public List<CheckBoxTagViewModel> Tags { get; set; }

        public CheckBoxTagViewModel() { }

        public CheckBoxTagViewModel(ChooseYourGameContext context)
        {
            this.Tags = new List<CheckBoxTagViewModel>();

            foreach (var tags in context.Tags)
            {
                var tag = new CheckBoxTagViewModel
                {
                    Id = tags.Id,
                    Description = tags.Description,
                    IsSelected = false
                };

                this.Tags.Add(tag);
            }
        }
    }
}