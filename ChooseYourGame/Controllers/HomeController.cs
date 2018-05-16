using Microsoft.AspNetCore.Mvc;
using ChooseYourGame.Models;
using ChooseYourGame.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ChooseYourGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly ChooseYourGameContext _contexto;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(ChooseYourGameContext contexto, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _contexto = contexto;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Main");
            }

            return RedirectToAction("LogIn");
        }
        
        public IActionResult LogIn()
        {
            if(!User.Identity.IsAuthenticated)
                return View();

            return RedirectToAction("Main");
        }
        [HttpPost]
        public  async Task<IActionResult> LogIn(LogInViewModel vm)
        {            
            if(ModelState.IsValid){
                var result = await _signInManager.PasswordSignInAsync(vm.LoginAccount, vm.LoginPassword, false, false);
                if(result.Succeeded){
                    return RedirectToAction("Main");
                }
                ModelState.AddModelError("", "Tentativa de LogIn invalido!");
            }

            return View(new AccountViewModel{Login = vm});
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(AccountViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = vm.UserName,
                    Email = vm.EMail
                };
                var result = await _userManager.CreateAsync(user, vm.Password);

                if (result.Succeeded)
                {
                    _contexto.Profiles.Add(new Profile
                    {
                        Name = vm.Name,
                        Lastname = vm.LastName,
                        UserId = await _userManager.GetUserIdAsync(user)
                    });
                    await _contexto.SaveChangesAsync();

                    await _signInManager.SignInAsync(user, false);
                    return Json(result);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return Json(new BadRequestObjectResult(ModelState));
            //return View("LogIn", vm);
        }

        [Authorize]
        public IActionResult Main()
        {
            var profile = _contexto.Profiles.
            Include(p => p.Blogs).
            Include(p => p.User).
            Where(p => p.UserId == _userManager.GetUserId(HttpContext.User)).First();
            var following = _contexto.Followers.Where(f => f.FollowerProfileId == profile.Id);
            var followers = _contexto.Followers.Where(f => f.FollowingProfileId == profile.Id);
            var blogs = _contexto.Blogs
            .Where(b =>
                following.Select(f => f.FollowingProfileId).Contains(b.ProfileId) == true ||
                b.ProfileId == profile.Id
                )
            .Include(b => b.Profile).ThenInclude(p=>p.User)
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

        public async Task<IActionResult> LogOut(){
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}