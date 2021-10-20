using System;
using System.Linq;
using Grid;
using Grid.Agent;
using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour
    {
        public GridAgent[] characters;
        private GridManager _grid;

        private event Action<GridAgent> OnSelectedCharacterChange;
        private event Action OnTurnStart;
        private event Action OnTurnEnd;

        private GridAgent _selectedCharacter;
        public GridAgent SelectedCharacter
        {
            get => _selectedCharacter;
            set
            {
                if (_selectedCharacter == value)
                {
                    _selectedCharacter = null;
                }
                else
                {
                    _selectedCharacter = value;
                }
                OnSelectedCharacterChange(_selectedCharacter);
            }
        }

        private bool _active;

        public void Init(GridManager grid)
        {
            _grid = grid;
            _grid.OnGridClick += HandleGridClick;

            OnTurnStart += HandleTurnStart;
            OnTurnEnd += HandleTurnEnd;

            foreach (var agent in characters)
            {
                agent.Init(grid);
                OnTurnStart += agent.HandleTurnStart;
                OnTurnEnd += agent.HandleTurnEnd;
                OnSelectedCharacterChange += agent.HandlePlayerSelectionChange;
            }
        }


        #region Event Handlers

        public void HandleTurnStart()
        {

        }

        public void HandleTurnEnd()
        {
            SelectedCharacter = null;
        }

        public void HandleActivePlayerChange(Player player)
        {
            if (player == this)
            {
                _active = true;
                OnTurnStart();
            }
            else if (_active)
            {
                _active = false;
                OnTurnEnd();
            }
        }

        public void HandleGridClick(Vector2Int gridPoint, Vector2 worldPoint)
        {
            if (_active)
            {
                var agent = _grid.Get(gridPoint)?.agent;
                if (agent)
                {
                    if (characters.Any(c => c == agent))
                    {
                        SelectedCharacter = agent;
                    }
                }
                else if (SelectedCharacter)
                {
                    var (cost, path) = _grid.Path(SelectedCharacter.position, gridPoint);
                    SelectedCharacter.SetPath(path);
                }
            }
        }

        #endregion
    }

}
