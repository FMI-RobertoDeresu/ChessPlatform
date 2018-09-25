using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ChessPlatform.Entities;
using ChessPlatform.Logging;
using ChessPlatform.Services;
using ChessPlatform.ViewModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChessPlatform.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IUserService _userService;
        private readonly SignInManager<User> _singInManager;
        private readonly IApplicationLogger _logger;
        private readonly IHostingEnvironment _hotingEnvironment;

        public AuthController(IUserService userService, SignInManager<User> signInManager, IApplicationLogger logger,
            IHostingEnvironment hotingEnvironment)
        {
            _userService = userService;
            _singInManager = signInManager;
            _logger = logger;
            _hotingEnvironment = hotingEnvironment;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Find", "Game");

            return View("Auth");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (model.Username.Length < 3)
                ModelState.AddModelError("Username", "Username must have minimum 3 characters");

            if (ModelState.IsValid)
            {
                try
                {
                    var result = _userService.Create(Mapper.Map<User>(model), model.Password,
                        _hotingEnvironment.WebRootPath + "\\img\\default-profile-photo.jpg");

                    if (result.Item1 == false)
                    {
                        if (!string.IsNullOrWhiteSpace(result.Item2))
                            return Json(new { registered = false, error = result.Item2 });
                    }
                    else
                    {
                        try
                        {
                            await _singInManager.PasswordSignInAsync(model.Username, model.Password, true, false);
                        }
                        catch (Exception exception)
                        {
                            _logger.LogError(exception);
                        }
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception);

                    return Json(new { registered = false, error = "An error has occured!" });
                }

                return Json(new { registered = true });
            }

            var errorMessage = ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.FirstOrDefault()?.ErrorMessage;
            var response = new
            {
                registered = false,
                error =
                errorMessage
            };

            return Json(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var signIn = await _singInManager.PasswordSignInAsync(model.Username, model.Password, true, false);
                    if (signIn.Succeeded)
                        return Json(new { authenticated = true });

                    ModelState.AddModelError("", "Username or password incorrect.");
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception);
                    ModelState.AddModelError("", "An unexpected error has occurred.");
                }
            }

            var errorMessage = ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.FirstOrDefault()?.ErrorMessage;
            var response = new
            {
                authenticated = false,
                error = errorMessage
            };

            return Json(response);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _singInManager.SignOutAsync();

            return RedirectToAction("Authenticate");
        }
    }
}