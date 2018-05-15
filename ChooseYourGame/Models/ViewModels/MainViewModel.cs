using System.Collections.Generic;

namespace ChooseYourGame.Models.ViewModels
{
    public class MainViewModel
    {
        public Profile Profile { get; set; }

        public int BlogsCount { get; set; }

        public int FollowingCount { get; set; }

        public int FollowersCount { get; set; }

        public IEnumerable<Blog> Blogs { get; set; }

        public MainViewModel(Profile profile, int blogsCount, int followingCount, int followersCount, IEnumerable<Blog> blogs)
        {
            this.Profile = profile;
            this.BlogsCount = blogsCount;
            this.FollowingCount = followingCount;
            this.FollowersCount = followersCount;
            this.Blogs = blogs;
        }
    }
}