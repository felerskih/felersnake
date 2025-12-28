using Starter.Api;
using Starter.Api.Requests;
using System.ComponentModel.DataAnnotations;

namespace Felersnake.Services
{
    public interface IFloodFiller
    {
        string GetBestDirection(GameStatusRequest game);
        bool CanFlood(string direction, GameStatusRequest game);
    }

    //flood fill from each desired move to see if there is enough space to go that way
    public class FloodFiller : IFloodFiller
    {
        private readonly ICoordinateChecker _coordinateChecker;

        public FloodFiller(ICoordinateChecker coordinateChecker)
        {
            _coordinateChecker = coordinateChecker;
        }

        public string GetBestDirection(GameStatusRequest game)
        {
            var head = game.You.Head;

            //refactor to use global values
            var directions = new Dictionary<string, Coordinate>
            {
                { "up",    new Coordinate(head.X, head.Y + 1) },
                { "down",  new Coordinate(head.X, head.Y - 1) },
                { "left",  new Coordinate(head.X - 1, head.Y) },
                { "right", new Coordinate(head.X + 1, head.Y) }
            };

            string bestMove = null;
            int bestSpace = 0;

            foreach (var kv in directions)
            {
                string move = kv.Key;
                Coordinate start = kv.Value;

                if (!_coordinateChecker.IsCoordinateSafe(game.Board, start, game.You, true))
                    continue;

                int space = FloodFillSafe(start, game);

                if (space > bestSpace)
                {
                    bestSpace = space;
                    bestMove = move;
                }
            }

            if(bestMove == null)
            {
                foreach (var kv in directions)
                {
                    string move = kv.Key;
                    Coordinate start = kv.Value;

                    if (!_coordinateChecker.IsCoordinateImmediatelySafe(game.Board, start, true))
                        continue;

                    int space = FloodFillImmediatelySafe(start, game);

                    if (space > bestSpace)
                    {
                        bestSpace = space;
                        bestMove = move;
                    }
                }
            }

            return bestMove ?? "up"; // fallback
        }

       
        public bool CanFlood(string direction, GameStatusRequest game)
        {
            var myHead = game.You.Head;
            var myLength = game.You.Body.Count();

            var directions = new Dictionary<string, Coordinate>
            {
                { "up",    new Coordinate(0,1) },
                { "down",  new Coordinate(0,-1) },
                { "left",  new Coordinate(-1,0) },
                { "right", new Coordinate(1,0) }
            };

            if (!directions.ContainsKey(direction))
                return false;

            var next = new Coordinate(myHead.X + directions[direction].X, myHead.Y + directions[direction].Y);
            return FloodFillSafe(next, game) > myLength ? true : FloodFillImmediatelySafe(next, game) > myLength ;
        }

        private int FloodFillSafe(Coordinate start, GameStatusRequest game)
        {
            var width = game.Board.Width;
            var height = game.Board.Height;
            var visited = new HashSet<Coordinate>();
            var queue = new Queue<Coordinate>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var point = queue.Dequeue();

                var neighbors = new List<Coordinate>
                {
                    new Coordinate(point.X + 1, point.Y),
                    new Coordinate(point.X - 1, point.Y),
                    new Coordinate(point.X, point.Y + 1),
                    new Coordinate(point.X, point.Y - 1)
                };

                foreach (var n in neighbors)
                {
                    if (!visited.Contains(n) &&
                        _coordinateChecker.IsCoordinateSafe(game.Board, n, game.You, true))
                    {
                        visited.Add(n);
                        queue.Enqueue(n);
                    }
                }
            }

            return visited.Count;
        }

        private int FloodFillImmediatelySafe(Coordinate start, GameStatusRequest game)
        {
            var width = game.Board.Width;
            var height = game.Board.Height;
            var visited = new HashSet<Coordinate>();
            var queue = new Queue<Coordinate>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var point = queue.Dequeue();

                var neighbors = new List<Coordinate>
                {
                    new Coordinate(point.X + 1, point.Y),
                    new Coordinate(point.X - 1, point.Y),
                    new Coordinate(point.X, point.Y + 1),
                    new Coordinate(point.X, point.Y - 1)
                };

                foreach (var n in neighbors)
                {
                    if (!visited.Contains(n) &&
                        _coordinateChecker.IsCoordinateImmediatelySafe(game.Board, n, true))
                    {
                        visited.Add(n);
                        queue.Enqueue(n);
                    }
                }
            }

            return visited.Count;
        }
    }
}
