using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grid {

    public class GridManager : MonoBehaviour
    {
        public Vector2Int size = new Vector2Int(3, 3);
        public Vector2 spacing = new Vector2(1, 1);

        private GenericSquareGrid<GameObject> _grid;

        public GameObject prefab;

        // Start is called before the first frame update
        void Start()
        {
            _grid = new GenericSquareGrid<GameObject>(size.x, size.y);
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    var point = new Vector2Int(x, y);
                    var item = Instantiate(prefab, GridToWorld(point), Quaternion.identity);
                    _grid.Put(point, item);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private Vector2 GridToWorld(Vector2Int position)
        {
            return spacing * position;
        }
    }
}
