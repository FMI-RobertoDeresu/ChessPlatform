using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace ChessPlatform.Entities
{
    public class User : IdentityUser
    {
        public int ExternalId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual UserProfile Profile { get; set; }
        public ICollection<User> Friends { get; protected set; }
        public virtual ICollection<Game> GamesAsPlayer1 { get; protected set; }
        public virtual ICollection<Game> GamesAsPlayer2 { get; protected set; }
        public virtual ICollection<Game> GamesAsWinner { get; protected set; }
        public virtual IEnumerable<Game> Games { get { return GamesAsPlayer1.Union(GamesAsPlayer2).Union(GamesAsWinner); } }

        public virtual ICollection<Notification> Notifications { get; protected set; }

        public User()
        {
            Profile = new UserProfile(this);
            Friends = null;
            GamesAsPlayer1 = new List<Game>();
            GamesAsPlayer2 = new List<Game>();
            GamesAsWinner = new List<Game>();
            Notifications = new List<Notification>();
        }
    }
}