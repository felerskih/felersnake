using Felersnake.Global;
using Starter.Api;
using Starter.Api.Requests;

namespace Felersnake.Services
{
    public interface ITargetLocator
    {
        Coordinate DetermineGoal(GameStatusRequest game);
    }

    public class TargetLocator : ITargetLocator
    {
        private readonly IPathFinder _pathFinder;

        public TargetLocator(IPathFinder pathFinder)
        {
            _pathFinder = pathFinder;
        }

        public Coordinate? DetermineGoal(GameStatusRequest game)
        {
            var myHead = game.You.Body.First(); // Head position
            var me = game.You;

            
            var foodDistances = game.Board.Food.Where(it => !_pathFinder.IsCoordinateMovableToByAnotherSnakeIn2Turns(game, it, me))
                .Select(it => new { Coordinate = it, dist = Math.Abs(it.X - myHead.X) + Math.Abs(it.Y - myHead.Y) });

            if (foodDistances.Any())
                return foodDistances.OrderBy(it => it.dist).First().Coordinate;

            return null;
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
