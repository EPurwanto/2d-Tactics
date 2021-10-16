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

        private Func<Vector2Int, Vector2Int, float> _costFunc;
        private Func<Vector2Int, Vector2Int, float> _hCostFunc;
        private Func<Vector2Int, IEnumerable<Vector2Int>> _neighboursFunc;

        public AStarPathFinder(Func<Vector2Int, Vector2Int, float> cost, Func<Vector2Int, Vector2Int, float> hCost, Func<Vector2Int, IEnumerable<Vector2Int>> neighbours)
        {
            _costFunc = cost;
            _hCostFunc = hCost;
            _neighboursFunc = neighbours;
        }

        public Tuple<float, List<Vector2Int>> Path(Vector2Int origin, Vector2Int destination)
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
                    return new Tuple<float, List<Vector2Int>>(node.GCost, path.GetRange(1, path.Count - 1));
                }

                foreach (var neighbour in ExpandNode(node, destination))
                {
                    openSet.Enqueue(neighbour, neighbour.GCost + neighbour.HCost);
                }
            }

            if (counter == 200)
            {
                throw new Exception("Step limit reached, no path found");
            }

            Debug.Log("Pathfinding Failure");
            return new Tuple<float, List<Vector2Int>>(Constants.CostInfinite, new List<Vector2Int>(0));
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

        private IEnumerable<PathNode> ExpandNode(PathNode node, Vector2Int destination)
        {
            return _neighboursFunc(node.Point).Select(neighbour => new PathNode()
            {
                Point = neighbour,
                Prev = node,
                GCost = node.GCost + _costFunc(node.Point, neighbour),
                HCost = _hCostFunc(neighbour, destination)
            });
        }
    }
}
