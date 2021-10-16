using System;
using System.Collections.Generic;
using UnityEngine;

namespace Grid.Pathing
{
    public interface IPathFinder
    {
        public Tuple<float, List<Vector2Int>> Path(Vector2Int origin, Vector2Int destination);
    }

    public static class Constants
    {
        public const float CostInfinite = 9999;
    }
}
