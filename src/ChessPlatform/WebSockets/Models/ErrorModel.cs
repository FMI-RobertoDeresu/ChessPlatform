namespace ChessPlatform.WebSockets.Models
{
    public class ErrorModel : BaseModel
    {
        public string ErrorMessage;

        //No room id, is send on socket.
        public ErrorModel(string errorMessage) : base("", "error")
        {
            ErrorMessage = errorMessage;
        }
    }
}
