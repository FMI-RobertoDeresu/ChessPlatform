namespace ChessPlatform.WebSockets.Models
{
    public class MovelModel : BaseModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Promotion { get; set; }
    }
}
