using System;
using System.Linq;
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
    public class AuthController : Controller
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

        public IActionResult Authenticate()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Find", "Game");
            }

            return View("Auth");
        }

        [HttpPost]
        public IActionResult Register([FromBody] RegisterViewModel model)
        {
            if (model.Username.Length < 3)
            {
                ModelState.AddModelError("Username", "Username must have minimum 3 characters");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = _userService.Create(Mapper.Map<User>(model), model.Password,
                        _hotingEnvironment.WebRootPath + "\\img\\default-profile-photo.jpg");

                    if (result.Item1 == false)
                    {
                        if (!string.IsNullOrWhiteSpace(result.Item2))
                        {
                            return Json(new { registered = false, error = result.Item2 });
                        }
                    }
                    else
                    {
                        try
                        {
                            var signIn = _singInManager.PasswordSignInAsync(model.Username, model.Password, true, false);

                            signIn.Wait();
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

            return
                Json(
                    new
                    {
                        registered = false,
                        error =
                        ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.FirstOrDefault().ErrorMessage
                    });
        }

        public IActionResult Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var signIn = _singInManager.PasswordSignInAsync(model.Username, model.Password, true, false);

                try
                {
                    if (signIn.Result.Succeeded)
                    {
                        return Json(new { authenticated = true });
                    }
                    ModelState.AddModelError("", "Username or password incorrect.");
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception);
                    ModelState.AddModelError("", "An unexpected error has occurred.");
                }
            }

            return
                Json(
                    new
                    {
                        authenticated = false,
                        error = ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.FirstOrDefault().ErrorMessage
                    });
        }

        [Authorize]
        public IActionResult Logout()
        {
            var signOut = _singInManager.SignOutAsync();

            signOut.Wait();

            return RedirectToAction("Authenticate");
        }
    }
}