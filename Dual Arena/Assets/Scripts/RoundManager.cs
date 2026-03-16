using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoundManager : MonoBehaviour
{
    public MoveSelectionManager moveManager;

    public int player1HP = 100;
    public int player2HP = 100;

    public void StartSimulation()
    {
        StartCoroutine(SimulateRounds());
    }
    string MoveName(int move)
    {
        if (move == 0) return "Light";
        if (move == 1) return "Heavy";
        if (move == 2) return "Block";
        return "Unknown";
    }
    IEnumerator SimulateRounds()
    {
        List<int> p1Moves = moveManager.player1Moves;
        List<int> p2Moves = moveManager.player2Moves;

        for (int i = 0; i < 3; i++)
        {
            Debug.Log("Round " + (i + 1) +
            " | P1: " + MoveName(p1Moves[i]) +
            " vs P2: " + MoveName(p2Moves[i]));

            ResolveRound(p1Moves[i], p2Moves[i]);

            if (player1HP <= 0 || player2HP <= 0)
            {
                Debug.Log("Fight ended early!");
                break;
            }

            yield return new WaitForSeconds(3f);
        }
        if (player1HP <= 0)
        {
            Debug.Log("PLAYER 2 WINS!");
        }
        else if (player2HP <= 0)
        {
            Debug.Log("PLAYER 1 WINS!");
        }
        Debug.Log("Simulation Finished");

        moveManager.player1Moves.Clear();
        moveManager.player2Moves.Clear();

        moveManager.simulationRunning = false;

        moveManager.ResetTurn();
    }

    void ResolveRound(int p1Move, int p2Move)
    {
        float roll = Random.Range(0f, 1f);

        if (p1Move == 2 && p2Move == 2)
        {
            Debug.Log("Both players blocked. Nothing happens.");
            return;
        }

        if (p1Move == p2Move)
        {
            float roll1 = Random.Range(0f, 1f);

            if (roll1 < 0.5f)
                DamagePlayer2(Random.Range(12, 18));
            else
                DamagePlayer1(Random.Range(12, 18));

            return;
        }

        // LIGHT vs HEAVY
        if (p1Move == 0 && p2Move == 1)
        {
            if (roll < 0.6f)
                DamagePlayer2(Random.Range(12, 18));
            else
                DamagePlayer1(Random.Range(25, 35));
        }

        else if (p1Move == 2 && p2Move == 1)
        {
            int dmg = Random.Range(25, 35);
            float reduction = Random.Range(0.40f, 0.65f);
            dmg = Mathf.RoundToInt(dmg * (1 - reduction));

            DamagePlayer1(dmg);
        }

        else if (p1Move == 1 && p2Move == 2)
        {
            int dmg = Random.Range(25, 35);
            float reduction = Random.Range(0.40f, 0.65f);
            dmg = Mathf.RoundToInt(dmg * (1 - reduction));

            DamagePlayer2(dmg);
        }

        // HEAVY vs LIGHT
        else if (p1Move == 1 && p2Move == 0)
        {
            if (roll < 0.6f)
                DamagePlayer1(Random.Range(12, 18));
            else
                DamagePlayer2(Random.Range(25, 35));
        }

        // LIGHT vs BLOCK
        else if (p1Move == 0 && p2Move == 2)
        {
            int dmg = Random.Range(12, 18);
            float reduction = Random.Range(0.40f, 0.65f);
            dmg = Mathf.RoundToInt(dmg * (1 - reduction));

            DamagePlayer2(dmg);
        }

        // BLOCK vs LIGHT
        else if (p1Move == 2 && p2Move == 0)
        {
            int dmg = Random.Range(12, 18);
            float reduction = Random.Range(0.40f, 0.65f);
            dmg = Mathf.RoundToInt(dmg * (1 - reduction));

            DamagePlayer1(dmg);
        }
    }

    void DamagePlayer1(int dmg)
    {
        player1HP -= dmg;

        if (player1HP < 0)
            player1HP = 0;

        Debug.Log("Player1 took " + dmg + " damage.");
        Debug.Log("HP Status → P1: " + player1HP + " | P2: " + player2HP);
    }
    void DamagePlayer2(int dmg)
    {
        player2HP -= dmg;

        if (player2HP < 0)
            player2HP = 0;

        Debug.Log("Player2 took " + dmg + " damage.");
        Debug.Log("HP Status → P1: " + player1HP + " | P2: " + player2HP);
    }
}