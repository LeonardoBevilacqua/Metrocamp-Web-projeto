using System.Threading.Tasks;
using ChooseYourGame.Models;
using ChooseYourGame.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
                return View(new AccountViewModel());

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
        public async Task<IActionResult> SignUp(SignUpViewModel vm)
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
                    await _userManager.AddToRoleAsync(user, "CommonUser");

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
        }

        [Authorize]
        public IActionResult Main()
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            MainViewModel vm = new MainViewModel(_contexto);
            vm.LoadProfileInfo(userId, true);

            return View(vm);
        }

        public async Task<IActionResult> LogOut(){
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}