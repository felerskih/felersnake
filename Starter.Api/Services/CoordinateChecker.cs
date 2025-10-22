using Felersnake.Global;
using Starter.Api;

namespace Felersnake.Services
{
    public interface ICoordinateChecker
    {
        bool IsCoordinateSafe(Board board, Coordinate toCheck, Snake me);
        bool IsCoordinateMovableToByAnotherSnake(Board board, Coordinate toCheck, Snake me);
        bool IsCoordinateImmediatelySafe(Board board, Coordinate toCheck);
    }

    public class CoordinateChecker : ICoordinateChecker
    {
        private readonly GlobalSnakeValues _global;

        public CoordinateChecker(GlobalSnakeValues global)
        {
            _global = global;
        }

        public bool IsCoordinateSafe(Board board, Coordinate toCheck, Snake me)
        {
            if(!IsCoordinateImmediatelySafe(board, toCheck))
                return false;
            if (IsCoordinateMovableToByAnotherSnake(board, toCheck, me))
                return false;
            return true;
        }

        public bool IsCoordinateMovableToByAnotherSnake(Board board, Coordinate toCheck, Snake me)
        {
            bool notSafe = false;

            foreach(var d in _global.Directions)
            {
                var next = new Coordinate(toCheck.X + d.X, toCheck.Y + d.Y);
                if (board.Snakes.Any(s => s.Id != me.Id && s.Head.Equals(next) && s.Length >= me.Length))
                    notSafe = true;
            }

            return notSafe;
        }

        public bool IsCoordinateImmediatelySafe(Board board, Coordinate toCheck)
        {
            // Check if out of bounds
            if (toCheck.X < 0 || toCheck.X >= board.Width || toCheck.Y < 0 || toCheck.Y >= board.Height)
                return false;
            // Check if in hazards
            if (board.Hazards.ToList().Contains(toCheck))
                return false;
            // Check if colliding with any snake, can't collide with tail as it moves next turn unless eating food...
            if (board.Snakes.Any(s => s.Body.ToList().Contains(toCheck) && (!s.Body.Last().Equals(toCheck) || s.Health == 100)))
                return false;
            return true;
        }

    }
}
