using System;

namespace ChessPlatform.ViewModels.User
{
    public class NotificationViewModel
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
    }
}
