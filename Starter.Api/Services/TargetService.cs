using Starter.Api.Global;
using Starter.Api.Requests;

namespace Starter.Api.Services
{
    public interface ITargetService
    {
        Coordinate DetermineGoal(GameStatusRequest game);
    }

    public class TargetService : ITargetService
    {
        private readonly ICoordinateChecker _coordinateChecker;
        private readonly GlobalSnakeValues _global;

        public TargetService(ICoordinateChecker coordinateChecker, GlobalSnakeValues global)
        {
            _coordinateChecker = coordinateChecker;
            _global = global;
        }

        public Coordinate DetermineGoal(GameStatusRequest game)
        {
            var myHead = game.You.Body.First(); // Head position
            var me = game.You.Id;

            var foodDistances = game.Board.Food.Where(it => !_coordinateChecker.IsCoordinateMovableToByAnotherSnake(game.Board, it, me))
                .Select(it => new { Coordinate = it, dist = Math.Abs(it.X - myHead.X) + Math.Abs(it.Y - myHead.Y) });

            if (foodDistances.Any())
                return foodDistances.OrderBy(it => it.dist).First().Coordinate;
            
            foreach (var d in _global.Directions)
            {
                var next = new Coordinate(myHead.X + d.X, myHead.Y + d.Y);
                if (_coordinateChecker.IsCoordinateSafe(game.Board, next, me))
                    return next;
            }
            
            foreach(var d in _global.Directions)
            {
                var next = new Coordinate(myHead.X + d.X, myHead.Y + d.Y);
                if (_coordinateChecker.IsCoordinateImmediatelySafe(game.Board, next) && !game.Board.Food.Contains(next))
                    return next;
            }

            return myHead; // No safe move found, stay in place (will die)
        }

        //Potential Strategy to use in the future;
        //private void DetermineZoneSize(Coordinate myHead, GameStatusRequest game) 
        //{
        //    var board = game.Board;
        //    if (board.Snakes.Count() > 2)
        //    {
        //        DetermineQuadrant(myHead, board);
        //    }
        //    else
        //    {

        //    }
        //}

        //private void DetermineQuadrant(Coordinate myHead, Board board)
        //{
        //    int midX = board.Width / 2;
        //    int midY = board.Height / 2;
        //    if (myHead.X <= midX && myHead.Y <= midY)
        //    {
        //        // Top-left zone
        //    }
        //    else if (myHead.X > midX && myHead.Y <= midY)
        //    {
        //        // Top-right zone
        //    }
        //    else if (myHead.X <= midX && myHead.Y > midY)
        //    {
        //        // Bottom-left zone
        //    }
        //    else
        //    {
        //        // Bottom-right zone
        //    }
        //}

        //private Coordinate PatrolQuadrant(int minX, int maxX, int minY, int maxY, Board board, Coordinate myHead)
        //{
        //    var food = board.Food.Where(it => it.X >= minX && it.X <= maxX && it.Y >= minY && it.Y <= maxY);
        //    if (food.Any())
        //    {
        //        var foodDistances = food.Select(it => new { Coordinate = it, dist = Math.Abs(it.X - myHead.X) + Math.Abs(it.Y - myHead.Y) });
        //        return foodDistances.OrderBy(it => it.dist).First().Coordinate;
        //    }
        //    else
        //    {
        //        // Patrol quadrant
        //        //Determine orientation and direction to nearest corner
        //    }
        //}
    }
}
