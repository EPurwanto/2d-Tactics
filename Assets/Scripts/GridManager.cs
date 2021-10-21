using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grid.Pathing;
using UnityEngine;

namespace Grid {

    public class GridManager : MonoBehaviour, IPathFinder, IAreaFinder
    {
        public Vector2Int size = new Vector2Int(3, 3);
        public Vector2 spacing = new Vector2(1, 1);

        private GenericSquareGrid<GridPoint> _grid;
        private IPathFinder _pathFinder;
        private IAreaFinder _areaFinder;

        public GridPoint prefab;

        private uint _nextHighlightKey = 0;
        private Dictionary<uint, IList<Vector2Int>> _highlights = new Dictionary<uint, IList<Vector2Int>>();

        /**
         * <param name="First">The Grid position</param>
         * <param name="Second">The world space position that grid point corresponds to</param>
         */
        public event Action<Vector2Int, Vector2> OnGridClick;

        public void Init()
        {
            _grid = new GenericSquareGrid<GridPoint>(size.x, size.y);
            _pathFinder = new AStarPathFinder<GridPoint>(Cost, HeuristicCost, Neighbours);
            _areaFinder = new SimpleAreaFinder(Cost, Neighbours);
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

        private void HandleGridClick(Vector2Int point)
        {
            OnGridClick?.Invoke(point, GridToWorld(point));
        }

        #region Grid Functions

        public Vector3 GridToWorld(Vector2Int position)
        {
            return transform.TransformPoint(spacing * position);
        }

        public GridPoint Get(Vector2Int point)
        {
            return _grid.Get(point);
        }

        public uint AddHighlightZone(IList<Vector2Int> points)
        {
            while (_highlights.ContainsKey(_nextHighlightKey))
            {
                _nextHighlightKey++;
            }

            _highlights[_nextHighlightKey] = points;
            foreach (var point in points)
            {
                var gridPoint = Get(point);
                gridPoint.AddHighlightId(_nextHighlightKey);
            }
            return _nextHighlightKey++;
        }

        public void ReleaseHighlightZone(uint id)
        {
            var points = _highlights[id];
            foreach (var point in points)
            {
                var gridPoint = Get(point);
                gridPoint.RemoveHighlightId(id);
            }
            _highlights.Remove(id);
        }

        #endregion

        #region IPathFinder

        public Tuple<float, List<Vector2Int>> Path(Vector2Int origin, Vector2Int destination)
        {
            return _pathFinder.Path(origin, destination);
        }

        #endregion

        #region IAreaFinder

        public IList<Vector2Int> Circle(Vector2Int center, float radius)
        {
            return _areaFinder.Circle(center, radius);
        }

        #endregion

        #region Pathfinding Functions


        public float Cost(Vector2Int origin, Vector2Int destination)
        {
            if (destination == origin)
            {
                return 0;
            }
            var diff = destination - origin;
            if (Math.Abs(diff.x) + Math.Abs(diff.y) == 1)
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

        #endregion
    }
}
