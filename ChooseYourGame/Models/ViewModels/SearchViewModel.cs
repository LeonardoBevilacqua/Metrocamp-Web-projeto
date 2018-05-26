using System.Linq;

namespace ChooseYourGame.Models.ViewModels
{
    public class SearchViewModel
    {
        public IQueryable<Blog> Blogs { get; set; }
        public IQueryable<Profile> Profiles { get; set; }
    }
}