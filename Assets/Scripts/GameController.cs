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
    private int activePlayer = 0;

    // Start is called before the first frame update
    void Start()
    {

        foreach (var player in players)
        {
            player.Init(grid);
        }
        players[activePlayer].SetActive(true);
    }

    public void NextPlayer()
    {
        players[activePlayer].SetActive(false);
        activePlayer++;
        activePlayer = activePlayer % players.Length;
        players[activePlayer].SetActive(true);

        Debug.Log($"Active Player: {players[activePlayer].gameObject.name}");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
