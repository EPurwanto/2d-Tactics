using System.Collections.Generic;
using UnityEngine;

namespace Grid.Pathing
{
    public interface IAreaFinder
    {
        IList<Vector2Int> Circle(Vector2Int center, float radius);
    }
}
