using ChessPlatform.ViewModels.User;

namespace ChessPlatform.ViewModels.Game
{
    public class GameInfoViewModel
    {
        public int Id { get; set; }
        public string CreatedAt { get; set; }
        public string StartedAt { get; set; }
        public string EndedAt { get; set; }
        public string Name { get; set; }
        public bool RequirePassword { get; set; }
        public UserViewModel Player1 { get; set; }
        public UserViewModel Player2 { get; set; }
        public UserViewModel Winner { get; set; }
        public int? MinimumNumberOfPoints { get; set; }
        public int? MaximumNumberOfPoints { get; set; }
        public bool AllowSpectate { get; set; }
        public int Turn { get; set; }
        public string Status { get; set; }
        public bool WithComputer { get; set; }

        public bool Started
        {
            get { return !string.IsNullOrWhiteSpace(StartedAt); }
        }

        public bool Ended
        {
            get { return !string.IsNullOrWhiteSpace(EndedAt); }
        }

        protected GameInfoViewModel() { }

        public GameInfoViewModel(Entities.Game game)
        {
            Id = game.Id;
            CreatedAt = game.CreatedAt.ToString("yyyy.MM.dd HH:mm");
            StartedAt = game.StartedAt.HasValue ? game.StartedAt.Value.ToString("yyyy.MM.dd HH:mm") : null;
            EndedAt = game.EndedAt.HasValue ? game.EndedAt.Value.ToString("yyyy.MM.dd HH:mm") : null;
            Name = game.Name;
            RequirePassword = game.Password != null;

            Player1 = new UserViewModel()
            {
                ExternalId = game.Player1 != null ? game.Player1.ExternalId : -1,
                Profile = new UserProfileViewModel()
                {
                    Nickname = game.Player1?.UserName
                }
            };

            Player2 = new UserViewModel()
            {
                ExternalId = game.Player2 != null ? game.Player2.ExternalId : -1,
                Profile = new UserProfileViewModel()
                {
                    Nickname = game.Player2?.UserName
                }
            };

            Winner = new UserViewModel()
            {
                ExternalId = game.Winner != null ? game.Winner.ExternalId : -1,
                Profile = new UserProfileViewModel()
                {
                    Nickname = game.Winner?.UserName
                }
            };

            MinimumNumberOfPoints = game.MinimumNumberOfPoints;
            MaximumNumberOfPoints = game.MaximumNumberOfPoints;
            AllowSpectate = game.AllowSpectate;
            Turn = game.Turn;
            Status = game.Status;
            WithComputer = game.WithComputer;
        }
    }
}
