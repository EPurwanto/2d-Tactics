using System;
using System.Collections.Generic;
using UnityEngine;

namespace Grid.Agent
{
    public enum AgentState
    {
        Uninitialised,
        Ready,
        Moving,
    }

    public class GridAgent : MonoBehaviour
    {
        public Vector2Int position;
        public float stepTimeS = 0.5f;
        private float stepTimer = 0;
        private IEnumerator<Vector2Int> _path;

        public bool showPath = true;
        public Color pathColor = Color.cyan;

        private AgentState _state = AgentState.Uninitialised;
        public AgentState State
        {
            get => _state;
            private set
            {
                var was = _state;
                _state = value;
                OnStateChange?.Invoke(was, value);
            }
        }

        /**
         * <param name="first">Previous Value</param>
         * <param name="second">New Value</param>
         */
        public event Action<AgentState, AgentState> OnStateChange;

        private GridManager _grid;

        private void Update()
        {
            if (_path != null)
            {
                stepTimer += Time.deltaTime;
                if (stepTimer >= stepTimeS)
                {
                    MoveNext();
                    stepTimer -= stepTimeS;
                }
            }
        }

        public void Init(GridManager grid)
        {
            State = AgentState.Ready;
            _grid = grid;
            transform.position = grid.GridToWorld(position);
        }

        public void FollowPath(IEnumerable<Vector2Int> path)
        {
            State = AgentState.Moving;

            if (showPath)
            {
                if (_path != null)
                {
                    // Clear tint on existing path nodes.
                    while (_path.MoveNext())
                    {
                        _grid.Get(_path.Current)?.ClearTint();
                    }
                }

                // Tint new path
                foreach (var point in path)
                {
                    _grid.Get(point)?.SetTint(pathColor);
                }
            }
            stepTimer = 0;
            _path = path.GetEnumerator();

            MoveNext();
        }

        public void MoveNext()
        {
            if (_path?.MoveNext() ?? false)
            {
                MoveTo(_path.Current);
                if (showPath)
                {
                    _grid.Get(position)?.ClearTint();
                }
            }
            else
            {
                State = AgentState.Ready;
                _path = null;
            }
        }

        public void MoveTo(Vector2Int point)
        {
            transform.position = _grid.GridToWorld(point);
            _grid.Get(position).agent = null;
            _grid.Get(point).agent = this;
            position = point;
        }
    }
}
