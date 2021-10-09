using System;
using Grid.Agent;
using UnityEngine;

namespace Grid
{
    public class GridPoint: MonoBehaviour
    {
        public event Action OnClick;

        public GridAgent agent;

        public void HandleClick()
        {
            OnClick?.Invoke();
        }
    }
}
