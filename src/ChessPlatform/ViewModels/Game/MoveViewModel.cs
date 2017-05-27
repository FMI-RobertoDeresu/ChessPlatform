namespace ChessPlatform.ViewModels.Game
{
    public class MoveViewModel
    {
        public int GameId { get; set; }

        public string BoardStatus { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Promotion { get; set; }

        public bool EndGame { get; set; }
        public bool IsCheckmate { get; set; }
    }
}