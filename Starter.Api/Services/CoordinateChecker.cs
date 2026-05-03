using Felersnake.Global;
using Starter.Api;

namespace Felersnake.Services
{
    public interface ICoordinateChecker
    {
        bool IsCoordinateSafe(Board board, Coordinate toCheck, Snake me, bool floodCheck = false);
        bool IsCoordinateMovableToByAnotherSnake(Board board, Coordinate toCheck, Snake me, bool floodCheck);
        bool IsCoordinateImmediatelySafe(Board board, Coordinate toCheck, Snake me, bool floodCheck = false, bool isTailCheck = false);
    }

    public class CoordinateChecker : ICoordinateChecker
    {
        public bool IsCoordinateSafe(Board board, Coordinate toCheck, Snake me, bool floodCheck = false)
        {
            if(!IsCoordinateImmediatelySafe(board, toCheck, me, floodCheck))
                return false;
            if (IsCoordinateMovableToByAnotherSnake(board, toCheck, me, floodCheck))
                return false;
            return true;
        }

        public bool IsCoordinateMovableToByAnotherSnake(Board board, Coordinate toCheck, Snake me, bool floodCheck)
        {
            bool notSafe = false;

            foreach(var d in GlobalSnakeValues.Directions)
            {
                var next = new Coordinate(toCheck.X + d.X, toCheck.Y + d.Y);
                if (board.Snakes.Any(s => s.Id != me.Id && s.Head.Equals(next) && (s.Length >= me.Length || !floodCheck)))
                    notSafe = true;
            }

            return notSafe;
        }

        public bool IsCoordinateImmediatelySafe(Board board, Coordinate toCheck, Snake me, bool floodCheck = false, bool isTailCheck = false)
        {
            // Check if out of bounds
            if (toCheck.X < 0 || toCheck.X >= board.Width || toCheck.Y < 0 || toCheck.Y >= board.Height)
                return false;
            // Check if in hazards
            if (board.Hazards.ToList().Contains(toCheck))
                return false;
            // Check if colliding with any snake, can't collide with head if you are larger, can't collide with tail as it moves next turn unless eating food...
            if (board.Snakes.Any(s => s.Body.ToList().Contains(toCheck) && (!(s.Id != me.Id && s.Head.Equals(toCheck) && s.Length >= me.Length)
                    || (!s.Body.Last().Equals(toCheck) || (s.Health == 100 && !isTailCheck && !s.Id.Equals(me.Id)) || floodCheck))))
                return false;
            return true;
        }
    }
}
