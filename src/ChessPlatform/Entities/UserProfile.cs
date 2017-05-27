using System;

namespace ChessPlatform.Entities
{
    public class UserProfile
    {
        public string Id { get; protected set; }

        public int Rank { get; set; }
        public byte[] Image { get; set; }
        public string Nickname { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GamesWon { get; set; }
        public int GamesDraw { get; set; }
        public int GamesLosses { get; set; }

        public virtual User User { get; protected set; }

        ///// <summary>
        ///// This must be temporary.
        ///// </summary>
        public int Points
        {
            get { return 700 + GamesWon * 100 - GamesLosses * 30; }
            set { value = 700 + GamesWon * 100 - GamesLosses * 30; }
        }

        protected UserProfile() { }

        public UserProfile(User user)
        {
            User = user;
        }
    }
}
