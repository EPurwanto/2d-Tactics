using System;
using Grid.Agent;
using UnityEngine;

namespace Grid
{
    public class GridPoint: MonoBehaviour
    {
        public event Action OnClick;

        public GridAgent agent;
        public SpriteRenderer sprite;

        private Color _baseColor;

        private void Start()
        {
            _baseColor = sprite.color;
        }

        public void SetTint(Color color)
        {
            sprite.color = color;
        }

        public void ClearTint()
        {
            sprite.color = _baseColor;
        }

        public void HandleClick()
        {
            OnClick?.Invoke();
        }
    }
}
