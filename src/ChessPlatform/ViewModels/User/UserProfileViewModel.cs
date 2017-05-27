using System;

namespace ChessPlatform.ViewModels.User
{
    public class UserProfileViewModel
    {
        public int? Rank { get; set; }
        public byte[] Image { get; set; }
        public string Nickname { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int GamesWon { get; set; }
        public int GamesDraw { get; set; }
        public int GamesLosses { get; set; }
        public int Points { get; set; }

        public int TotalGames
        {
            get { return GamesWon + GamesDraw + GamesLosses; }
        }

        public string ImageSrc
        {
            get
            {
                if (Image != null && Image.Length > 0)
                {
                    return String.Format("data:image/gif;base64,{0}", Convert.ToBase64String(Image));
                }

                return null;
            }
        }
    }
}
