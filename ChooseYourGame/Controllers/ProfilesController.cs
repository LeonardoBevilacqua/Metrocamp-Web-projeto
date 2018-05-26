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

        // public IActionResult MeuPerfil()
        // {
        //     string userId = _userManager.GetUserId(HttpContext.User);
        //     MainViewModel vm = new MainViewModel(_contexto);
        //     vm.LoadProfileInfo(userId, false);

        //     return View(vm);
        // }
        [AllowAnonymous]
        public IActionResult ViewProfile(string id)
        {
            string loggedUserId,userId;
            bool isCurrentUser;
            if (id == null)
            {
                return NotFound();
            }

            loggedUserId = _userManager.GetUserId(HttpContext.User);
            userId = userId = _contexto.Users
                    .Where(u => u.UserName == id)
                    .Select(u => u.Id).FirstOrDefault();
            
            if (userId == null)
            {
                return NotFound();
            }
            
            isCurrentUser = loggedUserId == userId;                


            MainViewModel vm = new MainViewModel(_contexto);
            vm.LoadProfileInfo(userId, false);

            if (!isCurrentUser)
            {
                vm.CheckFollowing(_userManager.GetUserId(HttpContext.User));                
            }

            return !isCurrentUser ? View(vm) : View("MeuPerfil", vm);
        }

        public IActionResult Follow(string id)
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            string profileUserId = _contexto.Users.Where(u => u.UserName == id).Select(u => u.Id).FirstOrDefault();

            var follow = new Follower
            {
                FollowerProfileUserId = _contexto.Profiles.Where(p => p.UserId == userId).Select(p => p.UserId).FirstOrDefault(),
                FollowingProfileUserId = _contexto.Profiles.Where(p => p.UserId == profileUserId).Select(p => p.UserId).FirstOrDefault()
            };

            _contexto.Followers.Add(follow);
            _contexto.SaveChanges();

            return RedirectToAction("ViewProfile", new { id = id });
        }
        public IActionResult Unfollow(string id)
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            string profileUserId = _contexto.Users.Where(u => u.UserName == id).Select(u => u.Id).FirstOrDefault();

            var follow = new Follower
            {
                Id = _contexto.Followers
            .Where(f =>
                f.FollowerProfileUserId == _contexto.Profiles.Where(p => p.UserId == userId).Select(p => p.UserId).FirstOrDefault() &&
                f.FollowingProfileUserId == _contexto.Profiles.Where(p => p.UserId == profileUserId).Select(p => p.UserId).FirstOrDefault())
            .Select(f => f.Id).FirstOrDefault()
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