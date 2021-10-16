using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game;
using Grid;
using Grid.Agent;
using Grid.Pathing;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GridManager grid;
    public Player[] players;
    private int _activePlayer = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var player in players)
        {
            player.Init(grid);
        }
        players[_activePlayer].OnTurnStart();
    }

    public void NextPlayer()
    {
        players[_activePlayer].OnTurnEnd();
        _activePlayer++;
        _activePlayer = _activePlayer % players.Length;
        players[_activePlayer].OnTurnStart();

        Debug.Log($"Active Player: {players[_activePlayer].gameObject.name}");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
