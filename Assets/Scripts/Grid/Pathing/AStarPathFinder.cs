using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private Func<Vector2Int, Vector2Int, float> _defaultCostFn;
        private Func<Vector2Int, Vector2Int, float> _hCostFn;
        private Func<Vector2Int, IEnumerable<Vector2Int>> _neighboursFn;

        public AStarPathFinder(Func<Vector2Int, Vector2Int, float> defaultCostFn, Func<Vector2Int, Vector2Int, float> hCost, Func<Vector2Int, IEnumerable<Vector2Int>> neighbours)
        {
            _defaultCostFn = defaultCostFn;
            _hCostFn = hCost;
            _neighboursFn = neighbours;
        }

        public (float, List<Vector2Int>) Path(Vector2Int origin, Vector2Int destination)
        {
            return Path(origin, destination, _defaultCostFn);
        }
        public (float, List<Vector2Int>) Path(Vector2Int origin, Vector2Int destination, Func<Vector2Int, Vector2Int, float> costFn)
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
                var node = openSet.Dequeue();
                if (node.Point == destination)
                {
                    Debug.Log("Pathfinding Success");
                    var path = ExtractPath(node);
                    return (node.GCost, path.GetRange(1, path.Count - 1));
                }

                foreach (var neighbour in ExpandNode(node, destination, costFn))
                {
                    openSet.Enqueue(neighbour, neighbour.GCost + neighbour.HCost);
                }
            }

            if (counter == 200)
            {
                throw new Exception("Step limit reached, no path found");
            }

            Debug.Log("Pathfinding Failure");
            return (Constants.CostInfinite, new List<Vector2Int>(0));
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

        private IEnumerable<PathNode> ExpandNode(PathNode node, Vector2Int destination, Func<Vector2Int, Vector2Int, float> costFn)
        {
            return _neighboursFn(node.Point).Select(neighbour => new PathNode()
            {
                Point = neighbour,
                Prev = node,
                GCost = node.GCost + costFn(node.Point, neighbour),
                HCost = _hCostFn(neighbour, destination)
            });
        }
    }
}
