using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ChooseYourGame.Models.ViewModels
{
    public class MainViewModel
    {
        public Profile Profile { get; set; }

        public int BlogsCount { get; set; }

        public int FollowingCount { get; set; }

        public int FollowersCount { get; set; }

        public IEnumerable<Blog> Blogs { get; set; }

        public bool IsFollowing { get; set; }

        private IQueryable<string> _Following { get; set; }
        private IQueryable<string> _Follower { get; set; }

        private readonly ChooseYourGameContext _context;

        public MainViewModel(ChooseYourGameContext context)
        {
            this._context = context;
        }

        public void LoadProfileInfo(string userId, bool followingBlogs)
        {
            this.Profile = this._context.Profiles
            .Include(p => p.Blogs).ThenInclude(b => b.Commentaries)
            .Include(p => p.Blogs).ThenInclude(b => b.BlogTag).ThenInclude(t => t.Tag)
            .Include(p => p.User)
            .Where(p => p.UserId == userId)
            .FirstOrDefault();

            this._Following = this._context.Followers
            .Where(f => f.FollowerProfileUserId == this.Profile.UserId)
            .Select(f => f.FollowingProfileUserId);

            this._Follower = this._context.Followers
            .Where(f => f.FollowingProfileUserId == this.Profile.UserId)
            .Select(f => f.FollowerProfileUserId);

            this.BlogsCount = this.Profile.Blogs.Count();

            this.FollowingCount = _Following.Count();

            this.FollowersCount = _Follower.Count();

            if (followingBlogs)
            {
                this.Blogs = this._context.Blogs
                .Where(b => _Following.Contains(b.ProfileUserId))
                .Include(b => b.Profile).ThenInclude(p => p.User)
                .Include(b => b.Commentaries)
                .Include(b => b.BlogTag).ThenInclude(t => t.Tag);

                this.Blogs = this.Blogs.Concat(this.Profile.Blogs).OrderByDescending(b => b.CreationTime);
            }
            else
            {
                this.Blogs = this.Profile.Blogs.OrderByDescending(b => b.CreationTime);
            }
            this.Profile.Blogs = null;
        }

        public void CheckFollowing(string userId)
        {
            this.IsFollowing = _Follower.Contains(userId);
        }
    }
}