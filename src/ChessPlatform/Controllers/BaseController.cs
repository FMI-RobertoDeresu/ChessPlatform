using ChessPlatform.Entities;
using ChessPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChessPlatform.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        private ILogger<BaseController> _logger;

        private IUserService _userService;

        private User _user;

        protected BaseController() { }

        public string GenericErrorMessage => "An unexpected error has occured!";

        public ILogger<BaseController> Logger
            => _logger ?? (_logger = HttpContext.RequestServices.GetService<ILogger<BaseController>>());

        private IUserService UserService
            => _userService ?? (_userService = HttpContext.RequestServices.GetService<IUserService>());

        public User CurrentPlayer => _user ?? (_user = UserService.GetUserWithProfileByUsername(User.Identity.Name));
    }
}