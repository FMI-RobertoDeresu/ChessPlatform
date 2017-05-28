using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ChessPlatform.Entities;
using ChessPlatform.Repositories;
using ChessPlatform.Services;
using ChessPlatform.ViewModels.Game;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ChessPlatform.Controllers
{
    public class GameController : BaseController
    {
        #region Constructor and fields

        private static Process _stockfish;
        private static StreamWriter myStreamWriter;
        private static StreamReader myStreamReader;

        private readonly IGameRepository _gameRepository;
        private readonly IUserService _userService;

        public GameController(IGameRepository gameRepository, IUserService userService, IHostingEnvironment env)
        {
            if (_stockfish == null)
            {
                _stockfish = new Process
                {
                    StartInfo =
                    {
                        FileName = Path.Combine(env.ContentRootPath, "Stockfish\\stockfish7.exe"),
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true
                    }
                };

                _stockfish.Start();

                myStreamWriter = _stockfish.StandardInput;
                myStreamReader = _stockfish.StandardOutput;
            }

            _gameRepository = gameRepository;
            _userService = userService;
        }

        #endregion

        #region Actions

        public IActionResult Find()
        {
            return View();
        }

        [HttpGet]
        public IActionResult New()
        {
            return View(new NewViewModel());
        }

        [HttpPost]
        public IActionResult New(NewViewModel model)
        {
            if (ModelState.IsValid || model.WithComputer)
            {
                try
                {
                    if (model.WithComputer && string.IsNullOrEmpty(model.Name))
                    {
                        model.Name = "Computer Game";
                    }

                    var game = new Game(model.Name, CurrentPlayer, model.AllowSpectate, model.Password,
                        model.MinimumNumberOfPoints, model.MaximumNumberOfPoints, model.WithComputer);

                    _gameRepository.Add(game);
                    _gameRepository.SaveChanges();

                    if (model.WithComputer)
                    {
                        Play(new PlayRequestViewModel { Id = game.Id, WithComputer = true, Password = game.Password });
                    }

                    return RedirectToAction("Game", new { id = game.Id });
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception);
                    ModelState.AddModelError("", GenericErrorMessage);

                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        [Route("Game")]
        public IActionResult Game(int id)
        {
            try
            {
                var game = _gameRepository.Get(id);

                if (CurrentPlayer.Id != game.Player1.Id && CurrentPlayer.Id != game.Player2?.Id && !game.AllowSpectate)
                {
                    return View("Unauthorized");
                }

                return View(new GameViewModel(CurrentPlayer, game));
            }
            catch (Exception exception)
            {
                Logger.LogError(exception);
            }

            //change this
            return null;
        }

        #endregion

        #region JSON 

        [HttpGet]
        public JsonResult GetGame(int id)
        {
            try
            {
                return Json(new GameViewModel(CurrentPlayer, _gameRepository.Get(id)));
            }
            catch (Exception exception)
            {
                Logger.LogError(exception);
            }

            return Json(new
            {
                Error = true,
                ErrorMessage = GenericErrorMessage
            });
        }

        [HttpGet]
        public JsonResult GetGames()
        {
            var games = _gameRepository.GetGames()
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
            var gamesModels = games.Select(game => new GameViewModel(CurrentPlayer, game))
                .ToList();

            try
            {
                return Json(new
                {
                    Games = gamesModels,
                    Error = false
                });
            }
            catch (Exception exception)
            {
                Logger.LogError(exception);
            }

            return Json(new { Error = true, ErrorMessage = GenericErrorMessage });
        }

        [HttpPost]
        public JsonResult Play([FromBody] PlayRequestViewModel model)
        {
            try
            {
                var game = _gameRepository.Get(model.Id);

                if (!(CurrentPlayer.Id == game.Player1.Id || CurrentPlayer.Id == game.Player2?.Id) || model.WithComputer)
                {
                    if (model.WithComputer == false)
                    {
                        if (game.StartedAt != null)
                        {
                            return Json(new { Error = true, ErrorMessage = "Game has been started!" });
                        }

                        if (game.MinimumNumberOfPoints.HasValue && CurrentPlayer.Profile.Points < game.MinimumNumberOfPoints.Value)
                        {
                            return Json(new { Error = true, ErrorMessage = "You are too weak for this game!" });
                        }

                        if (game.MaximumNumberOfPoints.HasValue && CurrentPlayer.Profile.Points > game.MaximumNumberOfPoints.Value)
                        {
                            return Json(new { Error = true, ErrorMessage = "You are too good for this game!" });
                        }

                        if (game.Password != model.Password)
                        {
                            return Json(new { Error = true, ErrorMessage = "Invalid password" });
                        }
                    }

                    game.StartGame(!model.WithComputer ? CurrentPlayer : null);

                    _gameRepository.Update(game);
                    _gameRepository.SaveChanges();
                }

                return Json(new { Error = false });
            }
            catch (Exception exception)
            {
                Logger.LogError(exception);
            }

            return Json(new { Error = true, ErrorMessage = GenericErrorMessage });
        }

        [HttpGet]
        public JsonResult GetBestMove(string fen)
        {
            lock (_stockfish)
            {
                Thread.Sleep(500);

                string mfrom, mto;

                myStreamWriter.Flush();
                myStreamWriter.WriteLine("position fen " + fen);
                myStreamWriter.WriteLine("go depth 1");

                var outputText = "h";

                while (outputText[0] != 'b')
                {
                    outputText = myStreamReader.ReadLine();
                }

                mfrom = mto = outputText;
                mfrom = mfrom.Remove(0, 9);
                mfrom = mfrom.Remove(2, mfrom.Length - 2);
                mto = mto.Remove(0, 11);
                mto = mto.Remove(2, mto.Length - 2);

                return Json(new { from = mfrom, to = mto });
            }
        }

        [HttpPost]
        public JsonResult MakeMove(MoveViewModel move)
        {
            try
            {
                var game = _gameRepository.Get(move.GameId);

                if (game.StartedAt.HasValue && !game.EndedAt.HasValue)
                {
                    if (CurrentPlayer.Id != game.Player1.Id && CurrentPlayer.Id != game.Player2?.Id)
                    {
                        return new JsonResult(new { Error = true, ErrorMessage = "You do not have permission to make moves." });
                    }

                    game.AddMove(move.From, move.To, move.Promotion, move.BoardStatus);

                    if (move.EndGame)
                    {
                        game.EndGame(move.IsCheckmate);

                        _userService.Update(game.Player1);
                        _userService.Update(game.Player2);
                    }

                    _gameRepository.Update(game);
                    _gameRepository.SaveChanges();

                    return Json(new { Error = false });
                }

                return Json(new { Error = true, ErrorMessage = "Game is not in progress." });
            }
            catch (Exception exception)
            {
                Logger.LogError(exception);
            }

            return Json(new { Error = true, ErrorMessage = GenericErrorMessage });
        }

        #endregion
    }
}