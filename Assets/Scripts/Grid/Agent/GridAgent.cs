using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Game;
using Grid.Pathing;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Grid.Agent
{
    public enum AgentState
    {
        Uninitialised,
        Dead,
        Idle,
        Ready,
        Moving,
    }

    public class GridAgent : MonoBehaviour
    {
        // Stats
        public Vector2Int position;
        public int maxMovement = 4;
        public int remainingMovement = 0;
        public int maxHealth = 10;
        public int currentHealth = 10;
        public int attackDamage = 1;
        public int attackRange = 1;

        // Movement
        public float stepTimeS = 0.5f;
        private float _stepTimer = 0;
        private IList<Vector2Int> _path;
        public LineRenderer lineRenderer;

        private bool _hasHighlight;
        private uint? _reachableAreaHighlightId;

        // State
        private bool _selected;
        private AgentState _state = AgentState.Uninitialised;
        private Player _controller;

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
            if (!_grid || !_controller)
            {
                State = AgentState.Uninitialised;
            }
             else if (currentHealth <= 0)
            {
                State = AgentState.Dead;
            }
            else if (_path != null)
            {
                State =  AgentState.Moving;
            }
            else if (Selected)
            {
                State =  AgentState.Ready;
            }
            else
            {
                State =  AgentState.Idle;
            }
        }

        public void Init(GridManager grid, Player controller)
        {
            _controller = controller;
            _grid = grid;
            transform.position = grid.GridToWorld(position);
            var gridPoint = grid.Get(position);
            if (gridPoint)
            {
                gridPoint.agent = this;
            }

            currentHealth = maxHealth;
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

        public float MovementCost(Vector2Int origin, Vector2Int destination)
        {
            if (destination == origin)
            {
                return 0;
            }
            var diff = destination - origin;
            if (Math.Abs(diff.x) + Math.Abs(diff.y) == 1)
            {
                var point = _grid.Get(destination);
                // Infinite cost for moving into occupied space or off edge of map
                if (point && !point.agent)
                {
                    return 1;
                }
            }

            return Constants.CostInfinite;
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
                var reachable = _grid.Circle(position, remainingMovement, MovementCost);
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

        public void Attack()
        {
            var range = _grid.Circle(position, attackRange);

            Debug.Log($"{gameObject.name} Range: {range.Count}");

            foreach (var point in range.Select(_grid.Get))
            {
                var target = point.agent;
                Debug.Log($"{gameObject.name} Target: {target}");
                if (target && target._controller != _controller && target.State != AgentState.Dead)
                {
                    Debug.Log($"{gameObject.name} Attack: {attackDamage}-> {target.gameObject.name}");
                    target.ReceiveDamage(attackDamage);
                }
            }
        }

        public void ReceiveDamage(int damage)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Debug.Log("Agent received fatal damage");
            }

            UpdateState();
        }

        #region Event Handlers

        public void HandleTurnStart()
        {
            remainingMovement = maxMovement;
        }

        public void HandleTurnEnd()
        {
            Selected = false;
            Attack();
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
