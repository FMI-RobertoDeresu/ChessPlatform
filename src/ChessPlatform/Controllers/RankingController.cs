using Microsoft.AspNetCore.Mvc;

namespace ChessPlatform.Controllers
{
    public class RankingController : BaseController
    {
        [Route("Leaderboard")]
        public IActionResult Players()
        {
            return View();
        }
    }
}