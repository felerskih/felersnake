using Felersnake.Global;
using Starter.Api;
using Starter.Api.Requests;

namespace Felersnake.Services
{
    public interface IPathFinder
    {
        string FindPath(GameStatusRequest game, Coordinate goal, bool FallbackToImmediate);
        bool FindPathToTail(GameStatusRequest game, Coordinate tail, Coordinate start);
    }

    public class PathFinder : IPathFinder
    {
        private readonly ICoordinateChecker _coordinateChecker;
        private readonly GlobalSnakeValues _global;
        
        public PathFinder(ICoordinateChecker coordinateChecker, GlobalSnakeValues global)
        {
            _coordinateChecker = coordinateChecker;
            _global = global;
        }


        public string FindPath(GameStatusRequest game, Coordinate goal, bool FallbackToImmediate)
        {
            var myHead = game.You.Body.First(); // Head position
            var me = game.You;
            var board = game.Board;


            var cameFrom = SearchFrontierForSafeGoal(myHead, board, goal, me, false);
            var path = GetPath(goal, cameFrom);
            if(path.Count == 0 && FallbackToImmediate)
            {
                //Couldn't find safe path to goal, try for immediately safe path
                cameFrom = SearchFrontierForImmediatelySafeGoal(myHead, board, goal, false, me.Id);
                path = GetPath(goal, cameFrom);
            }

            return GetDirectionFromPath(path, myHead);
        }

        public bool FindPathToTail(GameStatusRequest game, Coordinate tail, Coordinate start)
        {
            //tail is part of snake so not snake so need to ignore self it all tail checks
            var me = game.You;
            var board = game.Board;
            var cameFrom = SearchFrontierForSafeGoal(start, board, tail, me, true);
            var path = GetPath(tail, cameFrom);
            if (path.Count == 0)
            {
                //Couldn't find safe path to goal, try for immediately safe path
                cameFrom = SearchFrontierForImmediatelySafeGoal(start, board, tail, true, me.Id);
                path = GetPath(tail, cameFrom);
            }

            return path.Count > 0;
        }

        private Dictionary<Coordinate, Coordinate?> SearchFrontierForSafeGoal(Coordinate start, Board board, Coordinate goal, Snake me, bool isTailCheck)
        {
            var frontier = new Queue<Coordinate>();
            var cameFrom = new Dictionary<Coordinate, Coordinate?>();
            frontier.Enqueue(start);
            cameFrom[start] = null;
            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                if (current.Equals(goal))
                    break;

                foreach (var d in _global.Directions)
                {
                    var next = new Coordinate(current.X + d.X, current.Y + d.Y);

                    if (_coordinateChecker.IsCoordinateSafe(board, next, me, isTailCheck) && !cameFrom.ContainsKey(next))
                    {
                        frontier.Enqueue(next);
                        cameFrom[next] = current;
                    }
                }
            }
            return cameFrom;
        }

        private Dictionary<Coordinate, Coordinate?> SearchFrontierForImmediatelySafeGoal(Coordinate start, Board board, Coordinate goal, bool isTailCheck, string myId)
        {
            var frontier = new Queue<Coordinate>();
            var cameFrom = new Dictionary<Coordinate, Coordinate?>();
            frontier.Enqueue(start);
            cameFrom[start] = null;
            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                if (current.Equals(goal))
                    break;
                foreach (var d in _global.Directions)
                {
                    var next = new Coordinate(current.X + d.X, current.Y + d.Y);
                    if (_coordinateChecker.IsCoordinateImmediatelySafe(board, next, isTailCheck, myId) && !cameFrom.ContainsKey(next))
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
            }//Else, we couldn't get to goal, need to pick new goal to play for time
            return "none";
        }
    }
}
