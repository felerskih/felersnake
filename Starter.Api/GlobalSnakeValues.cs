using Starter.Api;

namespace Felersnake.Global
{
    public class GlobalSnakeValues
    {
        public readonly Coordinate[] Directions = new Coordinate[]
            {
                new Coordinate(0, -1), // down
                new Coordinate(0, 1),  // up
                new Coordinate(-1, 0), // left
                new Coordinate(1, 0)   // right
            };
    }
}
