using System;
using System.Collections;
using System.Collections.Generic;
using Grid.Pathing;
using UnityEngine;

namespace Grid {

    public class GridManager : MonoBehaviour, IPathFinder
    {
        public Vector2Int size = new Vector2Int(3, 3);
        public Vector2 spacing = new Vector2(1, 1);

        private GenericSquareGrid<GridPoint> _grid;
        private IPathFinder _pathFinder;

        public GridPoint prefab;

        /**
         * <param name="First">The Grid position</param>
         * <param name="Second">The world space position that grid point corresponds to</param>
         */
        public event Action<Vector2Int, Vector2> OnGridClick;

        // Start is called before the first frame update
        private void Start()
        {
            _grid = new GenericSquareGrid<GridPoint>(size.x, size.y);
            _pathFinder = new AStarPathFinder<GridPoint>(Cost, HeuristicCost, Neighbours);
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    var point = new Vector2Int(x, y);
                    var item = Instantiate(prefab, GridToWorld(point), Quaternion.identity, transform);
                    _grid.Put(point, item);
                    item.OnClick += () => HandleGridClick(point);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void HandleGridClick(Vector2Int point)
        {
            OnGridClick?.Invoke(point, GridToWorld(point));
        }

        public Vector2 GridToWorld(Vector2Int position)
        {
            return transform.TransformPoint(spacing * position);
        }

        public GridPoint Get(Vector2Int point)
        {
            return _grid.Get(point);
        }

        public Tuple<float, IEnumerable<Vector2Int>> Path(Vector2Int origin, Vector2Int destination)
        {
            return _pathFinder.Path(origin, destination);
        }

        public float Cost(Vector2Int origin, Vector2Int destination)
        {
            if ((destination - origin).magnitude <= 1.1f)
            {
                var point = Get(destination);
                // Infinite cost for moving into occupied space or off edge of map
                if (point && !point.agent)
                {
                    return 1;
                }
            }

            return Constants.CostInfinite;
        }

        public float HeuristicCost(Vector2Int origin, Vector2Int destination)
        {
            var delta = (destination - origin);
            // Crow Flies
            // return delta.magnitude;

            // Manhattan
            return Math.Abs(delta.x) + Math.Abs(delta.y);
        }

        private Vector2Int[] _neighbourDirections = new[]
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left,
        };
        private IEnumerable<Vector2Int> Neighbours(Vector2Int position)
        {
            var neighbours = new List<Vector2Int>();

            foreach (var dir in _neighbourDirections)
            {
                var nextPoint = position + dir;
                var pointExists = Get(nextPoint) != null;

                if (pointExists)
                {
                    neighbours.Add(nextPoint);
                }
            }

            return neighbours;
        }
    }
}
