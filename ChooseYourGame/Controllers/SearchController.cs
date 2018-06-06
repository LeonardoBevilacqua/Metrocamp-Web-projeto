using System.Collections.Generic;
using System.Linq;
using ChooseYourGame.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChooseYourGame.Controllers
{
    public class SearchController : Controller
    {
        private readonly ChooseYourGameContext _contexto;

        private readonly UserManager<IdentityUser> _userManager;

        public SearchController(ChooseYourGameContext contexto, UserManager<IdentityUser> userManager)
        {
            _contexto = contexto;
            _userManager = userManager;
        }

        public IActionResult Index(string id)
        {
            SearchViewModel vm = new SearchViewModel();
            if (id != null)
            {
                vm.Blogs = _contexto.Blogs
                    .Include(b => b.BlogTag)
                    .Include(b => b.Profile).ThenInclude(p => p.User)
                    .Where(b => b.BlogTag
                        .Select(
                            bt => bt.Tag.Description.ToLower()).Contains(id.ToLower())
                        || b.Title.ToLower().Contains(id.ToLower())
                    );

                vm.Profiles = _contexto.Profiles
                    .Include(p => p.User)
                    .Where(p => p.User.UserName.ToLower().Contains(id.ToLower())
                        || p.Name.ToLower().Contains(id.ToLower())
                        || p.Lastname.ToLower().Contains(id.ToLower())
                    );

                ViewBag.BuscaId = id;
            }

            return View(vm);
        }

        public IActionResult Following(string id)
        {
            ViewBag.Title = id + " esta seguindo";
            var userId = _contexto.Users.Where(u => u.UserName == id).Select(u => u.Id).FirstOrDefault();
            ViewBag.userId = _userManager.GetUserId(HttpContext.User);
            ViewBag.user = id;
            var following = _contexto.Followers.Where(f => f.FollowerProfileUserId == userId).Select(f => f.FollowingProfileUserId);
            var profiles = _contexto.Profiles.Include(p => p.User).Where(p => following.Contains(p.UserId));

            List<ProfilesViewModel> profilesList = new List<ProfilesViewModel>();

            foreach (var profile in profiles)
            {
                profilesList.Add(
                    new ProfilesViewModel(profile, _contexto, userId)
                );
            }

            return View("UsersList", profilesList);
        }

        public IActionResult Followers(string id)
        {
            ViewBag.Title = "Seguidores de " + id;
            var userId = _contexto.Users.Where(u => u.UserName == id).Select(u => u.Id).FirstOrDefault();
            var currentUserId = _userManager.GetUserId(HttpContext.User);
            ViewBag.userId = currentUserId;
            ViewBag.user = id;
            var followers = _contexto.Followers.Where(f => f.FollowingProfileUserId == userId).Select(f => f.FollowerProfileUserId);
            var profiles = _contexto.Profiles.Include(p => p.User).Where(p => followers.Contains(p.UserId));

            List<ProfilesViewModel> profilesList = new List<ProfilesViewModel>();

            foreach (var profile in profiles)
            {
                profilesList.Add(
                    new ProfilesViewModel(profile, _contexto, currentUserId)
                );
            }

            return View("UsersList", profilesList);
        }
    }
}