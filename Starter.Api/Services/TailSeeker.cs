using Felersnake.Global;
using Starter.Api;
using Starter.Api.Requests;

namespace Felersnake.Services
{
    public interface ITailSeeker
    {
        string? FindTail(GameStatusRequest game);
    }

    public class TailSeeker : ITailSeeker
    {
        private readonly IPathFinder _pathFinder;

        public TailSeeker(IPathFinder pathFinder)
        {
            _pathFinder = pathFinder;
        }

        //It might be worth while to implement another rule to check for food that can path to head and tail
        public string? FindTail(GameStatusRequest game)
        {
            var tail = game.You.Body.Last();

            foreach (var dir in GlobalSnakeValues.Directions)
            {
                var coordNextToTail = new Coordinate(tail.X + dir.X, tail.Y + dir.Y);
                var head = game.You.Head;
                if (coordNextToTail.X == head.X && coordNextToTail.Y == head.Y)
                {
                    //You're about to eat your tail, find something else
                    if (game.You.Health == 100)
                    {
                        Console.WriteLine($"You're about to eat yourself definitely on ${game.Turn}");
                        return null;
                    }

                    var inverseDir = new Coordinate(dir.X * -1, dir.Y * -1);

                    if (inverseDir.Y == -1)
                        return "down";
                    if (inverseDir.Y == 1)
                        return "up";
                    if (inverseDir.X == -1)
                        return "left";
                    if (inverseDir.X == 1)
                        return "right";

                }

                var dirToMove = _pathFinder.FindPath(game, coordNextToTail, true);
                if (dirToMove != null && !string.Equals(dirToMove, "none", StringComparison.OrdinalIgnoreCase))
                {
                    Coordinate temp = new Coordinate(0,0);
                    if (dirToMove == "down")
                        temp = GlobalSnakeValues.Down;
                    if (dirToMove == "up")
                        temp = GlobalSnakeValues.Up;
                    if (dirToMove == "left")
                        temp = GlobalSnakeValues.Left;
                    if (dirToMove == "right")
                        temp = GlobalSnakeValues.Right;

                    var nextCoord = new Coordinate(head.X + temp.X, head.Y + temp.Y);
                    if (coordNextToTail.X == head.X && coordNextToTail.Y == head.Y)
                    {
                        Console.WriteLine($"You're about to eat yourself possibly on ${game.Turn}");
                        return null;
                    }
                    return dirToMove;
                }
            }
            return null;
        }
    }
}