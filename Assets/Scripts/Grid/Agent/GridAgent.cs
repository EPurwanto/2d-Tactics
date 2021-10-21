using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private float _stepTimer = 0;
        public int maxMovement = 4;
        public int remainingMovement = 0;
        private IList<Vector2Int> _path;
        private bool _hasHighlight;
        private uint? _reachableAreaHighlightId;

        public LineRenderer lineRenderer;
        public Color rangeColour = Color.green;

        private bool _selected;
        private AgentState _state = AgentState.Uninitialised;

        #region PropertyAccessors

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                UpdateHighlights();
                UpdateState();
            }
        }

        public AgentState State
        {
            get => _state;
            private set
            {
                var was = _state;
                if (value != was)
                {
                    _state = value;
                    Debug.Log($"{gameObject.name} OnStateChange {was}-{value}");
                    OnStateChange?.Invoke(was, value);
                }
            }
        }

        #endregion

        /**
         * <param name="first">Previous Value</param>
         * <param name="second">New Value</param>
         */
        public event Action<AgentState, AgentState> OnStateChange;

        private GridManager _grid;

        private void Update()
        {
            if (_path != null && remainingMovement > 0)
            {
                _stepTimer += Time.deltaTime;
                if (_stepTimer >= stepTimeS)
                {
                    MoveNext();
                    _stepTimer -= stepTimeS;
                }
            }
        }

        private void UpdateState()
        {
            if (!_grid)
            {
                State = AgentState.Uninitialised;
            }
            else if (_path != null)
            {
                State =  AgentState.Moving;
            }
            else if (Selected)
            {
                State =  AgentState.Selected;
            }
            else
            {
                State =  AgentState.Ready;
            }
        }

        public void Init(GridManager grid)
        {
            _grid = grid;
            transform.position = grid.GridToWorld(position);
            var gridPoint = grid.Get(position);
            if (gridPoint)
            {
                gridPoint.agent = this;
            }
            UpdateState();
        }

        public void SetPath(IList<Vector2Int> path)
        {
            _path = path;
            _stepTimer = 0;
            UpdatePathIndicator();
            UpdateState();
        }

        public void MoveNext()
        {
            if (remainingMovement > 0 && _path != null)
            {
                var destination = _path[0];
                _path.RemoveAt(0);
                MoveTo(destination);

                if (_path.Count == 0)
                {
                    SetPath(null);
                }
            }
        }

        public void MoveTo(Vector2Int point)
        {
            var destination = _grid.Get(point);
            if (!destination || (destination.agent && destination.agent != this))
            {
                Debug.LogError($"Move Aborted {point}, {destination.agent}");
                return;
            }
            _grid.Get(position).agent = null;
            destination.agent = this;
            position = point;
            transform.position = _grid.GridToWorld(point);
            remainingMovement -= 1;

            UpdateHighlights();
            UpdatePathIndicator();
            Debug.Log($"Remaining Movement {remainingMovement} {point}");
        }

        private void UpdateHighlights()
        {
            if (_reachableAreaHighlightId.HasValue)
            {
                _grid.ReleaseHighlightZone(_reachableAreaHighlightId.Value);
                _reachableAreaHighlightId = null;
            }

            if (_selected)
            {
                var reachable = _grid.Circle(position, remainingMovement);
                _reachableAreaHighlightId = _grid.AddHighlightZone(reachable);
            }
        }

        private void UpdatePathIndicator()
        {
            if (_path != null)
            {
                // Update lineRenderer
                lineRenderer.positionCount = _path.Count + 1;
                var pathPositions = _path.Select(_grid.GridToWorld).ToList();
                pathPositions.Insert(0, transform.position);
                lineRenderer.SetPositions(pathPositions.ToArray());
            }
            else
            {
                lineRenderer.positionCount = 0;
                lineRenderer.SetPositions(new Vector3[0]);
            }
        }

        #region Event Handlers

        public void HandleTurnStart()
        {
            remainingMovement = maxMovement;
        }

        public void HandleTurnEnd()
        {
            Selected = false;
            UpdateState();
        }

        public void HandlePlayerSelectionChange(GridAgent agent)
        {
            if (agent == this)
            {
                Selected = true;
            }
            else if (Selected)
            {
                Selected = false;
            }
        }

        #endregion
    }
}
