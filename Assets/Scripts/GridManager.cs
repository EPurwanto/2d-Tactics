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
        void Start()
        {
            _grid = new GenericSquareGrid<GridPoint>(size.x, size.y);
            _pathFinder = new AStarPathFinder<GridPoint>(_grid);
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
            OnGridClick(point, GridToWorld(point));
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
    }
}
