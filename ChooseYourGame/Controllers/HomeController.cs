using Microsoft.AspNetCore.Mvc;
using ChooseYourGame.Models;
using ChooseYourGame.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ChooseYourGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly ChooseYourGameContext _contexto;

        public HomeController(ChooseYourGameContext contexto)
        {
            _contexto = contexto;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Main()
        {
            var profile = _contexto.Profiles.Include(p => p.Blogs).First();
            var following = _contexto.Followers.Where(f => f.FollowerProfileUserId == profile.UserId);
            var followers = _contexto.Followers.Where(f => f.FollowingProfileUserId == profile.UserId);

            //var test =   following.Where(f => f.FollowingProfileUserId == 2).ToString();
            var test = following.Select(f=> f.FollowingProfileUserId).Contains(2) == true;

            var blogs = _contexto.Blogs
            .Where(b =>
                following.Select(f=> f.FollowingProfileUserId).Contains(b.ProfileId) == true ||
                b.ProfileId == profile.UserId
                )
            .Include(b => b.Profile)
            .Include(b => b.Commentaries)
            .Include(b => b.BlogTag).ThenInclude(t => t.Tag)
            .OrderByDescending(b => b.CreationTime);

            MainViewModel mainData = new MainViewModel(
                profile,
                profile.Blogs.Count,
                following.Count(),
                followers.Count(),
                blogs);

            return View(mainData);
        }
    }
}