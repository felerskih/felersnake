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
        private readonly ITailSeeker _tailSeeker;
        private readonly string nomove = "none";

        public SsssssController(IPathFinder pathService, ITargetLocator targetLocator, IFloodFiller floodFiller, ITailSeeker tailSeeker)
        {
            _pathService = pathService;
            _targetLocator = targetLocator;
            _floodFiller = floodFiller;
            _tailSeeker = tailSeeker;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            var info = new
            {
                apiversion = "1",
                author = "trickett",
                //color = "#fc7b03",
                color = "#008080",
                head = "gamer",
                tail = "pixel"
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
            var nextMove = foodGoal != null ? _pathService.FindPath(game, foodGoal, false) :  nomove;

            //No safe food, flood fill to largest area
            if (nextMove.Equals(nomove) || !_floodFiller.CanFlood(nextMove, game)// bug in canflood
                || ! _tailSeeker.CanFindTail(game, nextMove)) 
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
