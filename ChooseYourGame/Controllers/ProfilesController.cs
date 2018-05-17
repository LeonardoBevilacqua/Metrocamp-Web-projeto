using System.Linq;
using ChooseYourGame.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChooseYourGame.Controllers
{
    [Authorize]
    public class ProfilesController : Controller
    {
        private readonly ChooseYourGameContext _contexto;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfilesController(ChooseYourGameContext contexto, UserManager<IdentityUser> userManager)
        {
            _contexto = contexto;
            _userManager = userManager;
        }

        public IActionResult MeuPerfil()
        {
            var profile = _contexto.Profiles.
            Include(p => p.Blogs).
            Include(p => p.User).
            Where(p => p.UserId == _userManager.GetUserId(HttpContext.User)).First();
            var following = _contexto.Followers.Where(f => f.FollowerProfileId == profile.Id);
            var followers = _contexto.Followers.Where(f => f.FollowingProfileId == profile.Id);
            var blogs = _contexto.Blogs
            .Where(b => b.ProfileId == profile.Id)
            .Include(b => b.Profile).ThenInclude(p => p.User)
            .Include(b => b.Commentaries)
            .Include(b => b.BlogTag).ThenInclude(t => t.Tag)
            .OrderByDescending(b => b.CreationTime);

            var vm = new MainViewModel(
                profile, profile.Blogs.Count,
                following.Count(),
                followers.Count(),
                blogs);

            return View(vm);
        }
        public IActionResult ViewProfile(string userName)
        {
            return View();
        }
    }
}