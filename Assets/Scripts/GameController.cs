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

    public GridAgent character;

    // Start is called before the first frame update
    void Start()
    {
        if (grid)
        {
            grid.OnGridClick += HandleGridClick;
        }

        if (character)
        {
            character.Init(grid);
        }
    }

    private void HandleGridClick(Vector2Int gridPoint, Vector2 worldPoint)
    {
        var (cost, path) = grid.Path(character.position, gridPoint);
        character.FollowPath(path);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
