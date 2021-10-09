using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;

namespace Grid
{
    public class GenericSquareGrid<T>: IGenericGrid<T>
    {
        public T[,] Items;
        public readonly int Width;
        public readonly int Height;

        public GenericSquareGrid(int width, int height)
        {
            Width = width;
            Height = height;
            Items = new T[width,height];
        }

        public T Get(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return default;
            }

            return Items[x, y];
        }

        public T Get(Vector2Int point)
        {
            return Get(point.x, point.y);
        }


        public T Put(int x, int y, T item)
        {
            var prev = Get(x, y);
            Items[x, y] = item;
            return prev;
        }

        public T Put(Vector2Int point, T item)
        {
            return Put(point.x, point.y, item);
        }

        public IEnumerable<T> List(Vector2Int[] points)
        {
            return points.Select((point) => Get(point.x, point.y));
        }
    }
}
