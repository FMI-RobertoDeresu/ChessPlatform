namespace ChessPlatform.ViewModels.Auth
{
    public class RegisterViewModel
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        protected RegisterViewModel() { }
    }
}
