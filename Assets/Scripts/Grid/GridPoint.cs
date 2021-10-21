using System;
using System.Collections.Generic;
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
        public Color highlightColor;
        private HashSet<uint> _highlightIds = new HashSet<uint>();

        private void Start()
        {
            _baseColor = sprite.color;
        }

        public void AddHighlightId(uint id)
        {
            if (_highlightIds.Count == 0)
            {
                sprite.color = highlightColor;
            }
            _highlightIds.Add(id);
        }

        public void RemoveHighlightId(uint id)
        {
            _highlightIds.Remove(id);
            if (_highlightIds.Count == 0)
            {
                sprite.color = _baseColor;
            }
        }

        public void HandleClick()
        {
            OnClick?.Invoke();
        }
    }
}
