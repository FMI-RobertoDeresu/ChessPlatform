namespace ChessPlatform.WebSockets.Models
{
    public class BaseModel
    {
        public string RoomId;
        public string MessageType;

        public BaseModel() { }

        public BaseModel(string roomId, string messageType)
        {
            RoomId = roomId;
            MessageType = messageType;
        }
    }
}