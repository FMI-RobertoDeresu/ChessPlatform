using ChessPlatform.ViewModels.User;
using System;

namespace ChessPlatform.ViewModels.User
{
    public class FriendViewModel
    {
        public DateTime FriendsFrom { get; set; }
        public bool IsReuquest { get; set; }
        public bool RequestDate { get; set; }
        public UserProfileViewModel Profile { get; set; }
    }
}
