using System;
using System.Linq;
using ChooseYourGame.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult BlogManager(int? id)
        {
            if (id != null)
            {
                var blog = _contexto.Blogs
                .Include(b => b.Profile)
                .ThenInclude(p => p.User)
                .Include(b => b.BlogTag)
                .SingleOrDefault(b => b.Id == id);

                return View(blog);
            }

            return View();
        }
        [HttpPost]
        public IActionResult BlogManager(Blog vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            if (vm.Id != 0)
            {
                _contexto.Attach(vm);
                _contexto.Entry(vm).Property("Title").IsModified = true;
                _contexto.Entry(vm).Property("Description").IsModified = true;
                _contexto.Entry(vm).Property("BlogText").IsModified = true;
            }
            else
            {
                vm.CreationTime = DateTime.Now;
                vm.ProfileUserId = _contexto.Profiles
                    .Where(p => p.UserId == _userManager.GetUserId(HttpContext.User))
                    .Select(p => p.UserId)
                    .First();
                _contexto.Add(vm);
            }
            _contexto.SaveChanges();

            return RedirectToAction("MeuPerfil", "Profiles");

        }

        [AllowAnonymous]
        public IActionResult ViewBlog(int id)
        {

            var blog = _contexto.Blogs
                .Include(b => b.Profile)
                .ThenInclude(p => p.User)
                .Include(b => b.BlogTag)
                .SingleOrDefault(b => b.Id == id);

            ViewBag.UserName = _userManager.GetUserName(HttpContext.User);

            return View(blog);
        }

        public IActionResult Delete(int id)
        {

            _contexto.Blogs.Remove(new Blog { Id = id });
            _contexto.SaveChanges();

            return RedirectToAction("MeuPerfil", "Profiles");
        }
    }
}