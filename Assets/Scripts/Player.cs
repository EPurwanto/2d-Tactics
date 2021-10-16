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

        private GridAgent _selectedCharacter;
        public GridAgent SelectedCharacter
        {
            get => _selectedCharacter;
            set
            {
                if (_selectedCharacter == value)
                {
                    _selectedCharacter?.Select(false);
                    _selectedCharacter = null;
                }
                else
                {
                    _selectedCharacter?.Select(false);
                    _selectedCharacter = value;
                    _selectedCharacter?.Select(true);
                }
            }
        }

        private bool _active;

        public void Init(GridManager grid)
        {
            _grid = grid;
            _grid.OnGridClick += HandleGridClick;
            foreach (var agent in characters)
            {
                agent.Init(grid);
            }
        }

        public void OnTurnStart()
        {
            _active = true;
            foreach (var agent in characters)
            {
                agent.OnTurnStart();
            }
        }

        public void OnTurnEnd()
        {
            _active = false;
            SelectedCharacter = null;
            foreach (var agent in characters)
            {
                agent.OnTurnEnd();
            }
        }

        private void HandleGridClick(Vector2Int gridPoint, Vector2 worldPoint)
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
    }

}
