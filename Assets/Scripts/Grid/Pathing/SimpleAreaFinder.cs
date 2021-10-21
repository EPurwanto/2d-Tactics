using System;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using UnityEngine;

namespace Grid.Pathing
{
    public class SimpleAreaFinder: IAreaFinder
    {
        private class PathNode
        {
            public Vector2Int Point;
            public float Cost;
        }

        private Func<Vector2Int, Vector2Int, float> _defaultCostFunc;
        private Func<Vector2Int, IEnumerable<Vector2Int>> _neighboursFunc;

        public SimpleAreaFinder(Func<Vector2Int, Vector2Int, float> defaultCost, Func<Vector2Int, IEnumerable<Vector2Int>> neighbours)
        {
            _defaultCostFunc = defaultCost;
            _neighboursFunc = neighbours;
        }

        public IList<Vector2Int> Circle(Vector2Int center, float radius)
        {
            return Circle(center, radius, _defaultCostFunc);
        }

        public IList<Vector2Int> Circle(Vector2Int center, float radius, Func<Vector2Int, Vector2Int, float> costFn)
        {
            var openSet = new SimplePriorityQueue<PathNode>();
            openSet.Enqueue(new PathNode(){
                Point = center,
                Cost = 0,
            }, 0);
            var closedSet = new List<Vector2Int>();

            while (openSet.Count > 0)
            {
                var node = openSet.Dequeue();
                closedSet.Add(node.Point);

                foreach (var neighbour in ExpandNode(node, costFn))
                {
                    if (neighbour.Cost <= radius)
                    {
                        openSet.Enqueue(neighbour, neighbour.Cost);
                    }
                }
            }

            return closedSet.GetRange(1, closedSet.Count - 1);
        }

        private IEnumerable<PathNode> ExpandNode(PathNode node, Func<Vector2Int, Vector2Int, float> costFn)
        {
            return _neighboursFunc(node.Point).Select(neighbour => new PathNode()
            {
                Point = neighbour,
                Cost = node.Cost + costFn(node.Point, neighbour),
            });
        }
    }
}
