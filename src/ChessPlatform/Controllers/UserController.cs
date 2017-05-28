using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using ChessPlatform.Services;
using ChessPlatform.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace ChessPlatform.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("/Profile/{id}")]
        public IActionResult Profile(int id)
        {
            return View(id);
        }

        [HttpGet]
        public JsonResult GetPlayer(int externalId)
        {
            try
            {
                return Json(new { Player = Mapper.Map<UserViewModel>(_userService.GetUserWithProfileByExternalId(externalId)) });
            }
            catch (Exception exception)
            {
                Logger.LogError(exception);
            }

            return Json(new { Error = true, ErrorMessage = GenericErrorMessage });
        }

        [HttpGet]
        public JsonResult GetCurrentPlayer()
        {
            try
            {
                return Json(new { Player = Mapper.Map<UserViewModel>(CurrentPlayer) });
            }
            catch (Exception exception)
            {
                Logger.LogError(exception);
            }

            return Json(new { Error = true, ErrorMessage = GenericErrorMessage });
        }

        [HttpGet]
        public JsonResult GetPlayers()
        {
            try
            {
                return Json(new
                {
                    Players = Mapper.Map<IEnumerable<UserViewModel>>(_userService.GetAllWithProfileAndRank()),
                    Error = false
                });
            }
            catch (Exception exception)
            {
                Logger.LogError(exception);
            }

            return Json(new { Error = true, ErrorMessage = GenericErrorMessage });
        }

        /// <summary>
        ///     Image must be resized.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateProfilePhoto()
        {
            try
            {
                if (Request.Form.Files.Count == 1 && Request.Form.Files[0].Length > 0)
                {
                    using (var imageStream = Request.Form.Files[0].OpenReadStream())
                    {
                        using (var stream = new MemoryStream())
                        {
                            int read;
                            var buffer = new byte[160 * 1024];

                            while ((read = imageStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                stream.Write(buffer, 0, read);
                            }

                            CurrentPlayer.Profile.Image = stream.ToArray();
                        }
                    }

                    _userService.Update(CurrentPlayer);
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception);

                return Json(new { error = true, errorMessage = GenericErrorMessage });
            }

            return Json(new { Error = false });
        }
    }
}