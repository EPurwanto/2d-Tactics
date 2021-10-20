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
    private event Action<Player> OnActivePlayerChange;

    public GridManager grid;
    public Player[] players;
    private int _activePlayer;

    private int ActivePlayer
    {
        get => _activePlayer;
        set
        {
            _activePlayer = value % players.Length;
            OnActivePlayerChange(players[_activePlayer]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var player in players)
        {
            player.Init(grid);
            OnActivePlayerChange += player.HandleActivePlayerChange;
        }

        ActivePlayer = 0;
    }

    public void NextPlayer()
    {
        ActivePlayer++;
        OnActivePlayerChange(players[ActivePlayer]);

        Debug.Log($"Active Player: {players[_activePlayer].gameObject.name}");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
