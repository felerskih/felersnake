using Felersnake.Global;
using Starter.Api;
using Starter.Api.Requests;

namespace Felersnake.Services
{
    public interface IPathFinder
    {
        string FindPath(GameStatusRequest game, Coordinate goal, bool FallbackToImmediate, Coordinate? start = null);
        bool IsCoordinateMovableToByAnotherSnakeIn2Turns(GameStatusRequest game, Coordinate toCheck, Snake me);
    }

    public class PathFinder : IPathFinder
    {
        private readonly ICoordinateChecker _coordinateChecker;
        
        public PathFinder(ICoordinateChecker coordinateChecker)
        {
            _coordinateChecker = coordinateChecker;
        }


        public string FindPath(GameStatusRequest game, Coordinate goal, bool FallbackToImmediate, Coordinate? start = null)
        {
            if (start == null)
                start = game.You.Body.First(); // Head position
            var me = game.You;
            var board = game.Board;


            var cameFrom = SearchFrontierForSafeGoal(start, game, goal, me);
            var path = GetPath(goal, cameFrom);
            if(path.Count == 0 && FallbackToImmediate)
            {
                //Couldn't find safe path to goal, try for immediately safe path
                cameFrom = SearchFrontierForImmediatelySafeGoal(start, board, goal);
                path = GetPath(goal, cameFrom);
            }

            return GetDirectionFromPath(path, start);
        }

        public bool IsCoordinateMovableToByAnotherSnakeIn2Turns(GameStatusRequest game, Coordinate toCheck, Snake me)
        {
            var shit = game.Board.Snakes.Any(s => s.Id != me.Id && GetPathCount(game, toCheck, false, s.Head) <= 2);
            return shit;
        }

        private int GetPathCount(GameStatusRequest game, Coordinate goal, bool FallbackToImmediate, Coordinate start)
        {
            var me = game.You;
            var board = game.Board;
            var cameFrom = SearchFrontierForSafeGoal(start, game, goal, me);
            var path = GetPath(goal, cameFrom);
            return path.Count();
        }

        private Dictionary<Coordinate, Coordinate?> SearchFrontierForSafeGoal(Coordinate myHead, GameStatusRequest game, Coordinate goal, Snake me)
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

                foreach (var d in GlobalSnakeValues.Directions)
                {
                    var next = new Coordinate(current.X + d.X, current.Y + d.Y);

                    if (_coordinateChecker.IsCoordinateSafe(game.Board, next, me) //&& !IsCoordinateMovableToByAnotherSnakeIn2Turns(game, next, me)
                            && !cameFrom.ContainsKey(next))
                    {
                        frontier.Enqueue(next);
                        cameFrom[next] = current;
                    }
                }
            }
            return cameFrom;
        }

        private Dictionary<Coordinate, Coordinate?> SearchFrontierForImmediatelySafeGoal(Coordinate myHead, Board board, Coordinate goal)
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
                foreach (var d in GlobalSnakeValues.Directions)
                {
                    var next = new Coordinate(current.X + d.X, current.Y + d.Y);
                    if (_coordinateChecker.IsCoordinateImmediatelySafe(board, next) && !cameFrom.ContainsKey(next))
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
