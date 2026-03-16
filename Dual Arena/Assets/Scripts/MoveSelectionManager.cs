using UnityEngine;
using System.Collections.Generic;

public class MoveSelectionManager : MonoBehaviour
{
    public List<int> player1Moves = new List<int>();
    public List<int> player2Moves = new List<int>();
    public bool simulationRunning = false;
    int currentPlayer = 1;

    public void SelectLight()
    {
        RegisterMove(0);
    }

    public void SelectHeavy()
    {
        RegisterMove(1);
    }

    public void SelectBlock()
    {
        RegisterMove(2);
    }
    
    string MoveName(int move)
    {
        if (move == 0) return "Light";
        if (move == 1) return "Heavy";
        if (move == 2) return "Block";
        return "?";
    }
    void RegisterMove(int move)
    {
        if (simulationRunning)
            return;

        if (player1Moves.Count == 3 && player2Moves.Count == 3)
            return;

        if (currentPlayer == 1)
        {
            player1Moves.Add(move);
            Debug.Log("P1 Move: " + MoveName(move));

            if (player1Moves.Count == 3)
            {
                currentPlayer = 2;
                Debug.Log("Player 2 turn");
            }
        }
        else
        {
            player2Moves.Add(move);
            Debug.Log("P2 Move: " + MoveName(move));

            if (player2Moves.Count == 3)
            {
                Debug.Log("Both players finished selecting!");

                simulationRunning = true;

                FindFirstObjectByType<RoundManager>().StartSimulation();
            }
        }
    }

    public void ResetTurn()
    {
        currentPlayer = 1;
        Debug.Log("Round reset. Player 1 choose moves.");
    }

}