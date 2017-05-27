namespace ChessPlatform.WebSockets.Models
{
    public class MessageModel : BaseModel
    {
        public string Id { get; set; }
        public string From { get; set; }
        public string Content { get; set; }
        public bool Mine { get; set; }

        public MessageModel() { }

        public MessageModel(MessageModel model, bool mine) :base(model.RoomId, "message")
        {
            Id = model.Id;
            From = model.From;
            Content = model.Content;
            Mine = mine;
        }
    }
}
