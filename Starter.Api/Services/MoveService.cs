using Starter.Api.Requests;
using Starter.Api.Responses;
using System.IO;

namespace Starter.Api.Services
{
    public interface IMoveService
    {
        MoveResponse Move(GameStatusRequest game, Coordinate goal);
        Coordinate DetermineGoal(GameStatusRequest game);
    }

    public class MoveService : IMoveService
    {
        private readonly Coordinate[] directions = new Coordinate[]
            {
                new Coordinate(0, -1), // down
                new Coordinate(0, 1),  // up
                new Coordinate(-1, 0), // left
                new Coordinate(1, 0)   // right
            };
        public Coordinate DetermineGoal(GameStatusRequest game)
        {
            var myHead = game.You.Body.First(); // Head position
            var foodDistances = game.Board.Food.Select(it => new { Coordinate = it, dist = Math.Abs(it.X - myHead.X) + Math.Abs(it.Y - myHead.Y) });
            return foodDistances.OrderBy(it => it.dist).First().Coordinate;
        }

        public MoveResponse Move(GameStatusRequest game, Coordinate goal)
        {
            var myHead = game.You.Body.First(); // Head position
            var board = game.Board;

                        
            var cameFrom = SearchFrontierForGoal(myHead, board, goal);

            var path = GetPath(goal, cameFrom);
            var move = GetDirectionFromPath(path, myHead);
            return new MoveResponse
            {
                Move = move,
                Shout = $"{move}"
            };
        }

        private Dictionary<Coordinate,Coordinate?> SearchFrontierForGoal(Coordinate myHead, Board board, Coordinate goal)
        {
            var frontier = new Queue<Coordinate>();
            var cameFrom = new Dictionary<Coordinate, Coordinate?>();
            frontier.Enqueue(myHead);
            cameFrom[myHead] = null;
            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                if (current.Equals(goal))
                    break;

                foreach (var d in directions)
                {
                    var next = new Coordinate(current.X + d.X, current.Y + d.Y);

                    // Skip if out of bounds or blocked
                    if (next.X < 0 || next.X >= board.Width || next.Y < 0 || next.Y >= board.Height)
                        continue;
                    if (board.Hazards.ToList().Contains(next))
                        continue;
                    if (board.Snakes.Any(s => s.Body.Skip(1).ToList().Contains(next))) // Skip if colliding with any snake body except head
                        continue;

                    if (!cameFrom.ContainsKey(next))
                    {
                        frontier.Enqueue(next);
                        cameFrom[next] = current;
                    }
                }
            }
            return cameFrom;
        }

        private List<Coordinate> GetPath(Coordinate goal, Dictionary<Coordinate, Coordinate?> cameFrom)
        {
            var path = new List<Coordinate>();
            if (cameFrom.ContainsKey(goal))
            {
                var node = goal;
                while (node != null)
                {
                    path.Add(node);
                    node = cameFrom[node];
                }
                path.Reverse();
            }

            return path;
        }

        private string GetDirectionFromPath(List<Coordinate> path, Coordinate myHead)
        {
            if (path.Count > 1)
            {
                var next = path[1]; // the tile after head
                if (next.X > myHead.X) return "right";
                else if (next.X < myHead.X) return "left";
                else if (next.Y > myHead.Y) return "up";
                else if (next.Y < myHead.Y) return "down";
            }
            return "up";
        }
    }
}
