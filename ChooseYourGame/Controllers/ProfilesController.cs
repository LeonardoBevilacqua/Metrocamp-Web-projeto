using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ChooseYourGame.Models;
using ChooseYourGame.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChooseYourGame.Controllers
{
    [Authorize]
    public class ProfilesController : Controller
    {
        private readonly ChooseYourGameContext _contexto;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly IHostingEnvironment _hostingEnvironment;

        public ProfilesController(ChooseYourGameContext contexto, UserManager<IdentityUser> userManager, IHostingEnvironment hostingEnvironment)
        {
            _contexto = contexto;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        [AllowAnonymous]
        public IActionResult ViewProfile(string id)
        {
            string loggedUserId, userId;
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

            if (null != _contexto.Followers.Where(f => f.FollowerProfileUserId == userId && f.FollowingProfileUserId == profileUserId).FirstOrDefault())
            {
                return RedirectToAction("ViewProfile", new { id = id });
            }

            var follow = new Follower
            {
                FollowerProfileUserId = userId,
                FollowingProfileUserId = profileUserId
            };

            _contexto.Followers.Add(follow);
            _contexto.SaveChanges();

            return RedirectToAction("ViewProfile", new { id = id });
        }
        public IActionResult Unfollow(string id)
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            string profileUserId = _contexto.Users.Where(u => u.UserName == id).Select(u => u.Id).FirstOrDefault();
            int followersId = _contexto.Followers.Where(f => f.FollowerProfileUserId == userId && f.FollowingProfileUserId == profileUserId).Select(f => f.Id).FirstOrDefault();
            if (followersId == 0)
            {
                return RedirectToAction("ViewProfile", new { id = id });
            }


            _contexto.Followers.Remove(new Follower { Id = followersId });
            _contexto.SaveChanges();

            return RedirectToAction("ViewProfile", new { id = id });
        }

        public IActionResult Config()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var profile = _contexto.Profiles.Find(userId);

            var vm = new ConfigViewModel
            {
                Name = profile.Name,
                Lastname = profile.Lastname,
                EMail = _contexto.Users.Where(u => u.Id == userId).Select(u => u.Email).FirstOrDefault()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Config(ConfigViewModel vm)
        {
            string newpicture = null;
            string newbackground = null;
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _contexto.Users.Find(userId);
            var profile = _contexto.Profiles.Find(userId);
            var result = await _userManager.CheckPasswordAsync(user,
             vm.CurrentPassword);

            if (!ModelState.IsValid || !result)
            {
                return View(vm);
            }
            string picture;
            string background;

            // find current image to delete if another is send
            if (vm.Picture != null)
            {
                picture = _contexto.Profiles.Find(userId).Picture;
                if (picture != null)
                {
                    System.IO.File.Delete(picture);
                }
                // Image Management
                picture = ContentDispositionHeaderValue.Parse(vm.Picture.ContentDisposition).FileName.Trim('"');
                picture = this.EnsureCorrectFilename(picture);

                newpicture = Guid.NewGuid().ToString() + Path.GetExtension(picture);
                profile.Picture = newpicture;
            }
            if (vm.Background != null)
            {
                background = _contexto.Profiles.Find(userId).Background;
                if (background != null)
                {
                    System.IO.File.Delete(background);
                }
                // Image Management
                background = ContentDispositionHeaderValue.Parse(vm.Background.ContentDisposition).FileName.Trim('"');
                background = this.EnsureCorrectFilename(background);

                newbackground = Guid.NewGuid().ToString() + Path.GetExtension(background);
                profile.Background = newbackground;
            }


            profile.Name = vm.Name;
            profile.Lastname = vm.Lastname;
            _contexto.Update(profile);
            _contexto.SaveChanges();

            if (user.Email != vm.EMail)
            {
                await _userManager.SetEmailAsync(user, vm.EMail);
            }
            if (vm.NewPassword != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, vm.NewPassword);
            }

            if (vm.Picture != null)
            {
                using (FileStream output = System.IO.File.Create(this.GetPathAndFilename(newpicture, "avatar")))
                {
                    await vm.Picture.CopyToAsync(output);
                }
            }
            if (vm.Background != null)
            {
                using (FileStream output = System.IO.File.Create(this.GetPathAndFilename(newbackground, "background")))
                {
                    await vm.Background.CopyToAsync(output);
                }
            }

            return RedirectToAction("Main", "Home");
        }

        public async Task<IActionResult> DeleteAccount()
        {
            var userId = _userManager.GetUserId(HttpContext.User);            

            var result = await _userManager.DeleteAsync(await _userManager.FindByIdAsync(userId));

            if (result.Succeeded)
            {
                return RedirectToAction("LogOut","Home");                
            }

            return View();

            // var commentaries = _contexto.Commentaries.Where(c => c.ProfileUserId == userId);
            // _contexto.RemoveRange(commentaries);

            // var profile = _contexto.Profiles
            //     .Include(p => p.Blogs)
            //         .ThenInclude(b => b.BlogTag)
            //     .Include(p => p.Blogs)
            //         .ThenInclude(b => b.Commentaries)
            //     .Include(p=>p.User)
            //     .Where(p => p.UserId == userId).First();

            // _contexto.Remove(profile);
            // _contexto.SaveChanges();
            

        }

        // File functions
        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }

        private string GetPathAndFilename(string filename, string folderName)
        {
            return this._hostingEnvironment.WebRootPath + "/uploads/" + folderName + "/" + filename;
        }
    }
}
