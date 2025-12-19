using Microsoft.AspNetCore.Mvc;
using Starter.Api.Requests;
using Starter.Api.Responses;
using Felersnake.Services;

namespace Starter.Api.Controllers
{
    [ApiController]
    public class SsssssController : ControllerBase
    {
        private readonly IPathFinder _pathService;
        private readonly ITargetLocator _targetLocator;
        private readonly IFloodFiller _floodFiller;
        private readonly string nomove = "none";

        public SsssssController(IPathFinder pathService, ITargetLocator targetLocator, IFloodFiller floodFiller)
        {
            _pathService = pathService;
            _targetLocator = targetLocator;
            _floodFiller = floodFiller;
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
            //Fully safe food goal
            var foodGoal = _targetLocator.DetermineGoal(game);
            var nextMove = _pathService.FindPath(game, foodGoal, false);

            //Fully safe non-food goal
            var nonFoodGoal = foodGoal;
            if(nextMove.Equals(nomove))
            {
                nonFoodGoal = _targetLocator.DetermineNonFoodGoal(game);
                nextMove = _pathService.FindPath(game, nonFoodGoal, false);
            }

            //Immediately safe food goal
            if (nextMove.Equals(nomove))
            { 
                nextMove = _pathService.FindPath(game, foodGoal, true);
            }

            //Immediately safe non-food goal
            if (nextMove.Equals(nomove))
            {
                nextMove = _pathService.FindPath(game, nonFoodGoal, true);
            }

            //No safe food, flood fill to largest area
            if(nextMove.Equals(nomove) || !_floodFiller.CanFlood(nextMove, game)) // bug in canflood
            {
                nextMove = _floodFiller.GetBestDirection(game);
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
