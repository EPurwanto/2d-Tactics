using System;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

namespace Grid.Pathing
{
    public class AStarPathFinder<TGridNode>: IPathFinder
    {
        private class PathNode
        {
            public float GCost;
            public float HCost;
            public Vector2Int Point;
            public PathNode Prev;
        }

        private IGenericGrid<TGridNode> grid;

        public AStarPathFinder(IGenericGrid<TGridNode> grid)
        {
            this.grid = grid;
        }

        public float Cost(Vector2Int origin, Vector2Int destination)
        {
            if ((destination - origin).magnitude <= 1)
            {
                return 1;
            }

            return Constants.CostInfinite;
        }

        public float HeuristicCost(Vector2Int origin, Vector2Int destination)
        {
            return (destination - origin).magnitude;
        }

        public Tuple<float, IEnumerable<Vector2Int>> Path(Vector2Int origin, Vector2Int destination)
        {
            Debug.Log("Pathfinding Start");
            var counter = 0;
            var openSet = new SimplePriorityQueue<PathNode>();
            openSet.Enqueue(new PathNode()
            {
                Point = origin
            }, 0);
            // var closedSet = new SimplePriorityQueue<PathNode>();

            while (openSet.Count > 0 && counter < 200)
            {
                var next = openSet.Dequeue();
                if (next.Point == destination)
                {
                    Debug.Log("Pathfinding Success");
                    var path = ExtractPath(next);
                    return new Tuple<float, IEnumerable<Vector2Int>>(next.GCost, path);
                }

                foreach (var neighbour in ExpandNode(next, destination))
                {
                    openSet.Enqueue(neighbour, neighbour.GCost + neighbour.HCost);
                }
            }

            if (counter == 200)
            {
                throw new Exception("Step limit reached, no path found");
            }

            Debug.Log("Pathfinding Failure");
            return new Tuple<float, IEnumerable<Vector2Int>>(Constants.CostInfinite, new Vector2Int[0]);
        }

        private List<Vector2Int> ExtractPath(PathNode node)
        {
            if (node == null)
            {
                return new List<Vector2Int>();
            }

            var path = ExtractPath(node.Prev);
            path.Add(node.Point);
            return path;
        }

        private Vector2Int[] _neighbourDirections = new[]
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left,
        };
        private List<PathNode> ExpandNode(PathNode node, Vector2Int destination)
        {
            var neighbours = new List<PathNode>();

            foreach (var dir in _neighbourDirections)
            {
                var nextPoint = node.Point + dir;
                var pointExists = grid.Get(nextPoint) != null;

                if (pointExists)
                {
                    neighbours.Add(new PathNode()
                    {
                        Point = nextPoint,
                        Prev = node,
                        GCost = node.GCost + Cost(node.Point, nextPoint),
                        HCost = HeuristicCost(nextPoint, destination)
                    });
                }
            }

            return neighbours;
        }
    }
}
