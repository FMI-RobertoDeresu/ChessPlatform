using System;

namespace ChessPlatform.Entities
{
    public class Notification
    {
        public long Id { get; protected set; }
        public string UserId { get; protected set; }

        public DateTime CreatedAt { get; set; }
        public string Content { get; protected set; }

        public virtual User User { get; protected set; }

        protected Notification() { }

        public Notification(User user, string content)
        {
            User = user;
            Content = content;
        }
    }
}
