using System;
using System.Linq;
using ChooseYourGame.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChooseYourGame.Controllers
{
    [Authorize]
    public class BlogsController : Controller
    {
        private readonly ChooseYourGameContext _contexto;
        private readonly UserManager<IdentityUser> _userManager;

        public BlogsController(ChooseYourGameContext contexto, UserManager<IdentityUser> userManager)
        {
            _contexto = contexto;
            _userManager = userManager;
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Blog vm)
        {
            if (ModelState.IsValid)
            {
                vm.CreationTime = DateTime.Now;
                vm.ProfileId = _contexto.Profiles
                    .Where(p => p.UserId == _userManager.GetUserId(HttpContext.User))
                    .Select(p => p.Id)
                    .First();

                _contexto.Add(vm);
                _contexto.SaveChanges();
                // Alterar futuramente para meu perfil
                return RedirectToAction("Index", "Home");
            }

            return View(vm);
        }
    }
}