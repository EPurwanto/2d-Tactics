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
            Debug.Log($"Following path {path.ToString()}");
            State = AgentState.Moving;
            _path = path.GetEnumerator();
            stepTimer = 0;
            MoveNext();
        }

        public void MoveNext()
        {
            if (_path?.MoveNext() ?? false)
            {
                Debug.Log($"Moving to next path node at {_path.Current}");
                MoveTo(_path.Current);
            }
            else
            {
                Debug.Log("No path or path had ended");
                State = AgentState.Ready;
                _path = null;
            }
        }

        public void MoveTo(Vector2Int point)
        {
            Debug.Log($"Moving to {point}");
            transform.position = _grid.GridToWorld(point);
            _grid.Get(position).agent = null;
            _grid.Get(point).agent = this;
            position = point;
        }
    }
}
