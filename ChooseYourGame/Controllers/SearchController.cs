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


            return View(vm);
        }
    }
}