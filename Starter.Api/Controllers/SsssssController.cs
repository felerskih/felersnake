using Microsoft.AspNetCore.Mvc;
using Starter.Api.Requests;
using Starter.Api.Services;

namespace Starter.Api.Controllers
{
    [ApiController]
    public class SsssssController : ControllerBase
    {
        private readonly IMoveService _moveService;
        private readonly ITargetService _targetService;

        public SsssssController(IMoveService moveService, ITargetService targetService)
        {
            _moveService = moveService;
            _targetService = targetService;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            var info = new
            {
                apiversion = "1",
                author = "trickett", // TODO: Your Battlesnake Username
                color = "#fc7b03", // TODO: Personalize
                head = "gamer", // TODO: Personalize
                tail = "pixel"  // TODO: Personalize
            };
            return Ok(info);
        }

        [HttpPost("/start")]
        public IActionResult Start(GameStatusRequest game)
        {
            return Ok();
        }

        [HttpPost("/move")]
        public IActionResult Move(GameStatusRequest game)
        {
            var goal = _targetService.DetermineGoal(game);
            var nextMove = _moveService.Move(game, goal);
            return Ok(nextMove);
        }

        [HttpPost("/end")]
        public IActionResult End(GameStatusRequest game)
        {
            return Ok();
        }
    }
}
