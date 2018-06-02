using System.Linq;

namespace ChooseYourGame.Models.ViewModels
{
    public class ProfilesViewModel
    {
        public Profile Profile { get; set; }
        public int FollowingCount { get; set; }

        public int FollowersCount { get; set; }

        private IQueryable<string> _Following { get; set; }
        private IQueryable<string> _Follower { get; set; }
        public ProfilesViewModel() { }
        public ProfilesViewModel(Profile profile, ChooseYourGameContext context, string userId)
        {
            this.Profile = profile;
            this._Following = context.Followers.Where(f => f.FollowerProfileUserId == profile.UserId).Select(f => f.FollowingProfileUserId);
            this._Follower = context.Followers.Where(f => f.FollowingProfileUserId == profile.UserId).Select(f => f.FollowerProfileUserId);
            
            this.FollowingCount = _Following.Count();
            this.FollowersCount = _Follower.Count();
        }
    }

}