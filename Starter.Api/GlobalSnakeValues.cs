using Starter.Api;

namespace Felersnake.Global
{
    public static class GlobalSnakeValues
    {
        public static readonly Coordinate[] Directions = new Coordinate[]
            {
                new Coordinate(0, -1), // down
                new Coordinate(0, 1),  // up
                new Coordinate(-1, 0), // left
                new Coordinate(1, 0)   // right
            };

        public static readonly Coordinate Down = Directions[0];
        public static readonly Coordinate Up = Directions[1];
        public static readonly Coordinate Left = Directions[2];
        public static readonly Coordinate Right = Directions[3];
    }
}
