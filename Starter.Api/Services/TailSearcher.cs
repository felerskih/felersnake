using Felersnake.Global;
using Starter.Api;
using Starter.Api.Requests;

namespace Felersnake.Services
{
    public interface ITailSearcher
    {
        bool CanReachOwnTail(string nextMove, GameStatusRequest game);
    }

    public class TailSearcher : ITailSearcher
    {
        private readonly ICoordinateChecker _coordinateChecker;
        private readonly GlobalSnakeValues _global;
        private readonly IPathFinder _pathFinder;

        public TailSearcher(ICoordinateChecker coordinateChecker, GlobalSnakeValues global, IPathFinder pathFinder)
        {
            _coordinateChecker = coordinateChecker;
            _global = global;
            _pathFinder = pathFinder;
        }

        public bool CanReachOwnTail(string nextMove, GameStatusRequest game)
        {
            var tail = game.You.Body.Last();
            var nextDir = new Coordinate(0, 0);
            if (nextMove.Equals("left"))
                nextDir = _global.Left;
            else if (nextMove.Equals("right"))
                nextDir = _global.Right;
            else if (nextMove.Equals("up"))
                nextDir = _global.Up;
            else if (nextMove.Equals("down"))
                nextDir = _global.Down;

            var nextCoord = new Coordinate(game.You.Head.X + nextDir.X, game.You.Head.Y + nextDir.Y);
            return _pathFinder.FindPathToTail(game, tail, nextCoord);
        }
    }
}
