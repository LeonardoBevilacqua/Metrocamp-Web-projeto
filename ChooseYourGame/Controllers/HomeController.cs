using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ChooseYourGame.Models;
using ChooseYourGame.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            if (!User.Identity.IsAuthenticated)
                return View(new AccountViewModel());

            return RedirectToAction("Main");
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(LogInViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(vm.LoginAccount, vm.LoginPassword, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Main");
                }
                ModelState.AddModelError("", "Tentativa de LogIn invalido!");
            }

            return View(new AccountViewModel { Login = vm });
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

        public JsonResult RequestNewPassword(string emailRecover)
        {
            var result = new SortedList();
            result.Add("result", false);
            result.Add("class", "danger");
            result.Add("message", "Erro ao tentar enviar E-Mail de recuperação.<br/>Tente novamente mais tarde.");

            IdentityUser user = _userManager.FindByEmailAsync(emailRecover).Result;

            if (user == null)
            {
                result["class"] = "warning";
                result["message"] = "E-Mail inválido!<br/>Tente novamente.";
                return Json(result);
            }

            var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;

            var resetLink = Url.Action("ResetPassword", "Home", new { token = token }, protocol: HttpContext.Request.Scheme);

            // Email function
            SmtpClient client = new SmtpClient("smtp.live.com");

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("cygnoreplay@hotmail.com");
            mailMessage.To.Add(user.Email);
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = "Este é o E-Mail para recuperação de senha.<br/><a href='" + resetLink
                + "'>Clique aqui</a> ou acesse o link abaixo para recuperar.<br/>" + resetLink;
            mailMessage.Subject = "CYG - Recuperar senha";
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("cygnoreplay@hotmail.com", "CygpqQP97");
            client.EnableSsl = true;

            try
            {
                client.Send(mailMessage);
                result["result"] = true;
                result["class"] = "success";
                result["message"] = "E-Mail enviado com sucesso.<br/>Acesse sua caixa de entrada.";
            }
            catch (System.Exception)
            {
                result["class"] = "danger";
                result["message"] = "Erro ao tentar enviar E-Mail de recuperação.<br/>Tente novamente mais tarde.";
            }

            return Json(result);
        }

        public IActionResult ResetPassword(string token)
        {
            if(token == null){
                return RedirectToAction("LogIn");
            }
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(IFormCollection form)
        {
            IdentityUser user = _userManager.FindByEmailAsync(form["email"]).Result;
            IdentityResult result = _userManager.ResetPasswordAsync
                (user, form["token"], form["password"]).Result;

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Main");
            }

            ViewBag.Message = "Erro ao tentar alterar a Senha.";

            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}