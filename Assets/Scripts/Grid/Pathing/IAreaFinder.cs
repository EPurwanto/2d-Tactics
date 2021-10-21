using System;
using System.Collections.Generic;
using UnityEngine;

namespace Grid.Pathing
{
    public interface IAreaFinder
    {
        IList<Vector2Int> Circle(Vector2Int center, float radius);
        IList<Vector2Int> Circle(Vector2Int center, float radius, Func<Vector2Int, Vector2Int, float> costFn);
    }
}
