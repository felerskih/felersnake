using Microsoft.AspNetCore.Mvc;
using Starter.Api.Requests;
using Starter.Api.Responses;
using Felersnake.Services;

namespace Starter.Api.Controllers
{
    [ApiController]
    public class SsssssController : ControllerBase
    {
        private readonly IMoveService _moveService;
        private readonly ITargetService _targetService;
        private readonly string nomove = "none";

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
                //color = "#008080",
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
            if(nextMove.Equals(nomove))
            {
                goal = _targetService.DetermineNonFoodGoal(game);
                nextMove = _moveService.Move(game, goal);
            }

            var moveResp = new MoveResponse
            {
                Move = nextMove,
                Shout = "Console.Write(Debug)"
            };
            return Ok(moveResp);
        }

        [HttpPost("/end")]
        public IActionResult End(GameStatusRequest game)
        {
            return Ok();
        }
    }
}
