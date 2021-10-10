using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Grid;
using Grid.Agent;
using Grid.Pathing;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GridManager grid;

    public GridAgent[] characters;
    private GridAgent selectedCharacter;

    // Start is called before the first frame update
    void Start()
    {
        if (grid)
        {
            grid.OnGridClick += HandleGridClick;
        }

        foreach (var agent in characters)
        {
            agent.Init(grid);
        }
    }

    private void HandleGridClick(Vector2Int gridPoint, Vector2 worldPoint)
    {
        var agent = grid.Get(gridPoint)?.agent;
        if (agent)
        {
            if (selectedCharacter == agent)
            {
                selectedCharacter.Select(false);
                selectedCharacter = null;
            }
            else
            {
                selectedCharacter?.Select(false);
                selectedCharacter = agent;
                selectedCharacter.Select(true);
            }
        }
        else if (selectedCharacter)
        {
            var (cost, path) = grid.Path(selectedCharacter.position, gridPoint);
            selectedCharacter.FollowPath(path);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
