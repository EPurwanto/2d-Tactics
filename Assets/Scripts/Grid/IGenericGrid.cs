using System;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public interface IGenericGrid<T>
    {
        public T Get(int x, int y);
        public T Get(Vector2Int point);

        public T Put(int x, int y, T item);
        public T Put(Vector2Int point, T item);

        public IEnumerable<T> List(Vector2Int[] points);
    }
}
