namespace Starter.Api.Services
{
    public interface ICoordinateChecker
    {
        bool IsCoordinateSafe(Board board, Coordinate toCheck);
        bool IsCoordinateMovableToByAnotherSnake(Board board, Coordinate toCheck, string me);
    }

    public class CoordinateChecker : ICoordinateChecker
    {
        //put this osmewhere else later
        private readonly Coordinate[] directions = new Coordinate[]
           {
                new Coordinate(0, -1), // down
                new Coordinate(0, 1),  // up
                new Coordinate(-1, 0), // left
                new Coordinate(1, 0)   // right
           };

        public bool IsCoordinateSafe(Board board, Coordinate toCheck)
        {
            // Check if out of bounds
            if (toCheck.X < 0 || toCheck.X >= board.Width || toCheck.Y < 0 || toCheck.Y >= board.Height)
                return false;
            // Check if in hazards
            if (board.Hazards.ToList().Contains(toCheck))
                return false;
            // Check if colliding with any snake
            if (board.Snakes.Any(s => s.Body.ToList().Contains(toCheck)))
                return false;
            return true;
        }

        public bool IsCoordinateMovableToByAnotherSnake(Board board, Coordinate toCheck, string me)
        {
            bool notSafe = false;

            foreach(var d in directions)
            {
                var next = new Coordinate(toCheck.X + d.X, toCheck.Y + d.Y);
                if (board.Snakes.Any(s => s.Id != me && s.Head.Equals(next)))
                    notSafe = true;
            }

            return notSafe;
        }
    }
}
