using ChessPlatform.ViewModels.Game;
using System.Collections.Generic;

namespace ChessPlatform.ViewModels.User
{
    public class UserViewModel
    {
        public int ExternalId { get; set; }
        public string Email { get; set; }
        public UserProfileViewModel Profile { get; set; }
        public IEnumerable<GameInfoViewModel> Games { get; set; }
        public IEnumerable<UserProfileViewModel> Friends { get; set; }
        public IEnumerable<NotificationViewModel> Notifications { get; set; }
    }
}