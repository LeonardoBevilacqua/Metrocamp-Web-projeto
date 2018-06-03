using System;
using System.Collections.Generic;
using System.Linq;
using ChooseYourGame.Models;
using ChooseYourGame.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            var tags = new CheckBoxTagViewModel(_contexto).Tags;

            if (id != null)
            {
                Blog blog = _contexto.Blogs
                .Include(b => b.BlogTag)
                .SingleOrDefault(b => b.Id == id && b.ProfileUserId == _userManager.GetUserId(HttpContext.User));

                if (blog == null)
                {
                    return NotFound();
                }

                foreach (var tag in tags)
                {
                    if (blog.BlogTag.Select(bt => bt.TagId).Contains(tag.Id))
                    {
                        tag.IsSelected = true;
                    }
                }

                ViewBag.Tags = tags;
                return View(blog);
            }

            ViewBag.Tags = tags;
            return View();
        }

        [HttpPost]
        public IActionResult BlogManager(int? id, Blog vm, IFormCollection form)
        {
            var tags = new CheckBoxTagViewModel(_contexto).Tags;
            if (ModelState.ErrorCount > 0)
            {
                ViewBag.Tags = tags;
                return View(vm);
            }

            var checkBoxTags = form["CheckBoxTags"];

            if (id == null)
            {
                vm.CreationTime = DateTime.Now;
                vm.ProfileUserId = _contexto.Profiles
                    .Where(p => p.UserId == _userManager.GetUserId(HttpContext.User))
                    .Select(p => p.UserId)
                    .FirstOrDefault();

                _contexto.Add(vm);
                _contexto.SaveChanges();
            }
            else
            {
                _contexto.Attach(vm);
                _contexto.Entry(vm).Property("Title").IsModified = true;
                _contexto.Entry(vm).Property("Description").IsModified = true;
                _contexto.Entry(vm).Property("BlogText").IsModified = true;

                vm.BlogTag = _contexto.Blogs
                            .Where(b => b.Id == vm.Id)
                            .Select(b => b.BlogTag).FirstOrDefault();

                vm.BlogTag.RemoveAll(b => b.BlogId == vm.Id);
                _contexto.SaveChanges();
            }

            List<BlogTag> blogTag = new List<BlogTag>();
            foreach (var tag in checkBoxTags)
            {
                blogTag.Add(new BlogTag
                {
                    TagId = int.Parse(tag),
                    BlogId = vm.Id
                });
            }

            vm.BlogTag = blogTag;
            _contexto.SaveChanges();

            return RedirectToAction("ViewProfile", "Profiles", new { id = _userManager.GetUserName(HttpContext.User) });
        }

        [AllowAnonymous]
        public IActionResult ViewBlog(int id)
        {

            var blog = _contexto.Blogs
                .Include(b => b.Profile)
                    .ThenInclude(p => p.User)
                .Include(b => b.BlogTag).ThenInclude(bt => bt.Tag)
                .Include(b => b.Commentaries)
                    .ThenInclude(c => c.Profile).ThenInclude(p => p.User)
                .SingleOrDefault(b => b.Id == id);

            blog.Commentaries.Reverse();

            ViewBag.UserName = _userManager.GetUserName(HttpContext.User);
            ViewBag.BlogId = blog.Id;

            return View(blog);
        }

        public IActionResult Comment(int blogId, string commentary)
        {
            var url = Url.Action(nameof(ViewBlog), new { id = blogId }) + "#commentaries";
            var newCommentary = new Commentary
            {
                ProfileUserId = _userManager.GetUserId(HttpContext.User),
                BlogId = blogId,
                CommentaryText = commentary
            };
            _contexto.Add(newCommentary);
            _contexto.SaveChanges();

            return Redirect(url);
        }

        public IActionResult Delete(int id)
        {
            var blog = _contexto.Blogs.Find(id);
            var userId = _userManager.GetUserId(HttpContext.User);

            if (blog.ProfileUserId == userId)
            {
                _contexto.Blogs.Remove(blog);
                _contexto.SaveChanges();
            }

            return RedirectToAction("ViewProfile", "Profiles", new { id = _userManager.GetUserName(HttpContext.User) });
        }
    }
}