using Starter.Api;

namespace Felersnake.Global
{
    public class GlobalSnakeValues
    {
        public readonly Coordinate Left = new Coordinate(-1, 0);
        public readonly Coordinate Right = new Coordinate(1, 0);
        public readonly Coordinate Up = new Coordinate(0, 1);
        public readonly Coordinate Down = new Coordinate(0, -1);

        public readonly Coordinate[] Directions = new Coordinate[]
            {
                new Coordinate(0, -1), // down
                new Coordinate(0, 1),  // up
                new Coordinate(-1, 0), // left
                new Coordinate(1, 0)   // right
            };
    }
}
