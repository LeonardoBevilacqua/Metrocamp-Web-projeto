using System.Linq;
using ChooseYourGame.Models;
using ChooseYourGame.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            string userId = _userManager.GetUserId(HttpContext.User);
            MainViewModel vm = new MainViewModel(_contexto);
            vm.LoadProfileInfo(userId, false);

            return View(vm);
        }
        public IActionResult ViewProfile(string id)
        {
            string userId = _contexto.Users
            .Where(u => u.UserName == id)
            .Select(u => u.Id).First();
            MainViewModel vm = new MainViewModel(_contexto);
            vm.LoadProfileInfo(userId, false);
            vm.CheckFollowing(_userManager.GetUserId(HttpContext.User));

            return View(vm);
        }

        public IActionResult Follow(string id)
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            string profileUserId = _contexto.Users.Where(u => u.UserName == id).Select(u => u.Id).First();

            var follow = new Follower
            {
                FollowerProfileId = _contexto.Profiles.Where(p => p.UserId == userId).Select(p => p.Id).First(),
                FollowingProfileId = _contexto.Profiles.Where(p => p.UserId == profileUserId).Select(p => p.Id).First()
            };

            _contexto.Followers.Add(follow);
            _contexto.SaveChanges();

            return RedirectToAction("ViewProfile", new { id = id });
        }
        public IActionResult Unfollow(string id)
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            string profileUserId = _contexto.Users.Where(u => u.UserName == id).Select(u => u.Id).First();

            var follow = new Follower
            {
                Id = _contexto.Followers
            .Where(f =>
                f.FollowerProfileId == _contexto.Profiles.Where(p => p.UserId == userId).Select(p => p.Id).First() &&
                f.FollowingProfileId == _contexto.Profiles.Where(p => p.UserId == profileUserId).Select(p => p.Id).First())
            .Select(f => f.Id).First()
            };

            _contexto.Followers.Remove(follow);
            _contexto.SaveChanges();

            return RedirectToAction("ViewProfile", new
            {
                id = id
            });
        }
    }
}