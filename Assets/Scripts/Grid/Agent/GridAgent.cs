using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Grid.Agent
{
    public enum AgentState
    {
        Uninitialised,
        Ready,
        Selected,
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
            var gridPoint = grid.Get(position);
            if (gridPoint)
            {
                gridPoint.agent = this;
            }
        }

        public void Select(bool selected)
        {
            if (selected)
            {
                State = AgentState.Selected;
            }
            else
            {
                State = AgentState.Ready;
            }
        }

        public void FollowPath(IEnumerable<Vector2Int> path)
        {
            State = AgentState.Moving;

            SetPath(path);
            stepTimer = 0;

            MoveNext();
        }

        public void SetPath(IEnumerable<Vector2Int> path)
        {
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
            _path = path.GetEnumerator();
        }

        public bool MoveNext()
        {
            if (_path?.MoveNext() ?? false)
            {
                if (MoveTo(_path.Current))
                {
                    if (showPath)
                    {
                        _grid.Get(position)?.ClearTint();
                    }
                    return true;
                }
                else
                {
                    State = AgentState.Ready;
                    SetPath(null);

                    return false;
                }
            }
            else
            {
                State = AgentState.Ready;
                SetPath(null);

                return false;
            }
        }

        public bool MoveTo(Vector2Int point)
        {
            var destination = _grid.Get(point);
            if (!destination || (destination.agent && destination.agent != this))
            {
                Debug.LogError($"Move Aborted {point}, {destination.agent}");
                return false;
            }
            _grid.Get(position).agent = null;
            destination.agent = this;
            position = point;
            transform.position = _grid.GridToWorld(point);

            return true;
        }
    }
}
