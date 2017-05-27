namespace ChessPlatform.ViewModels.Game
{
    public class GameViewModel
    {
        public string PlayerName;
        public bool CurrentUserIsPlayer1;
        public bool CurrentUserIsPlayer2;
        public GameInfoViewModel GameInfo;

        public bool CurrentUserIsPlayer
        {
            get { return CurrentUserIsPlayer1 || CurrentUserIsPlayer2; }
        }

        public GameViewModel(Entities.User user, Entities.Game game)
        {
            PlayerName = user.Profile.Nickname;
            CurrentUserIsPlayer1 = user.Id == game.Player1?.Id;
            CurrentUserIsPlayer2 = user.Id == game.Player2?.Id;
            GameInfo = new GameInfoViewModel(game);
        }
    }
}
