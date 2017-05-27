using System;
using System.Threading.Tasks;
using ChessPlatform.Entities;
using ChessPlatform.Repositories;
using ChessPlatform.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ChessPlatform.Controllers
{
    public class TestController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHostingEnvironment _hotingEnvironment;
        private readonly SignInManager<User> _singInManager;
        private readonly IGameRepository _gameRepository;
        private int? testGameId;

        public TestController(IUserService userService, IHostingEnvironment hotingEnvironment, SignInManager<User> singInManager,
            IGameRepository gameRepository)
        {
            _userService = userService;
            _hotingEnvironment = hotingEnvironment;
            _singInManager = singInManager;
            _gameRepository = gameRepository;
        }

        [Route("Test")]
        public JsonResult Test()
        {
            return Json(new
            {
                Test1 = new { Action = "User register", Result = TestUserRegister() },
                Test2 = new { Action = "User log in", Result = TestUserLogIn() },
                Test3 = new { Action = "Create game", Result = TestCreateGame() },
                Test4 = new { Action = "Get best move", Result = TestGetBestMove() }
            });
        }

        public JsonResult TestUserRegister()
        {
            try
            {
                var newUser = new User
                {
                    UserName = "TEST",
                    Email = "TEST@TEST.com"
                };

                newUser.Profile = new UserProfile(newUser)
                {
                    Nickname = "NICKNAMETEST",
                    FirstName = "FIRSTNAMETEST",
                    LastName = "LASTNAMETEST"
                };

                var result = _userService.Create(newUser, "123456",
                    _hotingEnvironment.WebRootPath + "\\img\\default-profile-photo.jpg");

                if (result.Item1 == false)
                {
                    if (!string.IsNullOrWhiteSpace(result.Item2))
                    {
                        return Json(new
                        {
                            registered = false,
                            error = result.Item2
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                return Json(new { Status = "Error", ErrorMessage = exception.Message });
            }

            return Json(new { Status = "OK", ErrorMessage = string.Empty });
        }

        public JsonResult TestUserLogIn()
        {
            try
            {
                Task<SignInResult> signIn = _singInManager.PasswordSignInAsync("TEST", "123456", false, false);

                if (signIn.Result.Succeeded)
                {
                    _singInManager.PasswordSignInAsync("roberto", "qweqwe", true, false).Wait();

                    try
                    {
                        DeleteTestUser();
                    }
                    catch (Exception) { }
                    ;

                    return Json(new { Status = "OK", ErrorMessage = string.Empty });
                }
                try
                {
                    DeleteTestUser();
                }
                catch (Exception) { }

                return Json(new { Status = "!OK", ErrorMessage = "User not found" });
            }
            catch (Exception exception)
            {
                try
                {
                    DeleteTestUser();
                }
                catch (Exception) { }

                return Json(new { Status = "Error", ErrorMessage = exception.Message });
            }
        }

        public JsonResult TestCreateGame()
        {
            try
            {
                var game = new Game("TESTGAME", _userService.GetUserByUsername("TEST"), false, "", null, null, false);

                _gameRepository.Add(game);
                _gameRepository.SaveChanges();

                testGameId = game.Id;
            }
            catch (Exception exception)
            {
                return Json(new { Status = "Error", ErrorMessage = exception.Message });
            }

            try
            {
                DeleteTestGame();
            }
            catch (Exception) { }

            return Json(new { Status = "OK", ErrorMessage = string.Empty });
        }

        public JsonResult TestGetBestMove()
        {
            try
            {
                var bestMove =
                    new GameController(_gameRepository, _userService, _hotingEnvironment).GetBestMove(
                        "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

                if (string.IsNullOrEmpty(bestMove.ToString()))
                {
                    return Json(new { Status = "!OK", ErrorMessage = "No move returned" });
                }
                return Json(new { Status = "OK", ErrorMessage = string.Empty });
            }
            catch (Exception exception)
            {
                return Json(new { Status = "Error", ErrorMessage = exception.Message });
            }
        }

        public void DeleteTestUser()
        {
            try
            {
                _userService.Delete(_userService.GetUserByUsername("TEST"));
            }
            catch (Exception) { }
        }

        public void DeleteTestGame()
        {
            try
            {
                if (testGameId.HasValue)
                {
                    _gameRepository.Delete(_gameRepository.Get(testGameId.Value));
                    _gameRepository.SaveChanges();
                }
            }
            catch (Exception) { }
        }
    }
}