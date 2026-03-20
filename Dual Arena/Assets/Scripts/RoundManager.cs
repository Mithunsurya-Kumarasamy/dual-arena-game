using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class RoundManager : MonoBehaviour
{
    public MoveSelectionManager moveManager;
    public SpriteRenderer backgroundRenderer;
    public Sprite[] maps;
    public Slider player1HPBar;
    public Slider player2HPBar;

    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;

    public TextMeshProUGUI player1HPText;
    public TextMeshProUGUI player2HPText;

    public TextMeshProUGUI statusText;
    public TextMeshProUGUI roundText;

    public int player1HP = 100;
    public int player2HP = 100;

    int roundNumber = 1;

    void Start()
    {
        // Set names
        player1NameText.text = GameData.player1Name;
        player2NameText.text = GameData.player2Name;
        backgroundRenderer.sprite = maps[GameData.selectedMap];
        backgroundRenderer.transform.localScale = new Vector3(1.21f, 1.21f, 1f);
        UpdateHPUI();
    }

    void UpdateHPUI()
    {
        player1HPBar.value = player1HP;
        player2HPBar.value = player2HP;

        player1HPText.text = player1HP.ToString();
        player2HPText.text = player2HP.ToString();
    }

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

        roundText.text = "ROUND " + roundNumber;

        for (int i = 0; i < 3; i++)
        {
            yield return StartCoroutine(ResolveRound(p1Moves[i], p2Moves[i]));

            if (player1HP <= 0 || player2HP <= 0)
                break;

            yield return new WaitForSeconds(2f);
        }

        // WIN CHECK
        if (player1HP <= 0)
        {
            statusText.text = GameData.player2Name + " WINS!";
            moveManager.simulationRunning = true;
            yield break;
        }
        else if (player2HP <= 0)
        {
            statusText.text = GameData.player1Name + " WINS!";
            moveManager.simulationRunning = true;
            yield break;
        }

        // NEXT ROUND
        moveManager.player1Moves.Clear();
        moveManager.player2Moves.Clear();

        moveManager.simulationRunning = false;
        moveManager.ResetTurn();

        roundNumber++;
    }

    IEnumerator ResolveRound(int p1Move, int p2Move)
    {
        string p1 = GameData.player1Name;
        string p2 = GameData.player2Name;

        // Show moves
        statusText.text = p1 + " uses " + MoveName(p1Move);
        yield return new WaitForSeconds(1f);

        statusText.text += "\n" + p2 + " uses " + MoveName(p2Move);
        yield return new WaitForSeconds(1f);

        float roll = Random.Range(0f, 1f);

        // BOTH BLOCK
        if (p1Move == 2 && p2Move == 2)
        {
            statusText.text += "\nBoth blocked!";
            yield return new WaitForSeconds(1.5f);
            yield break;
        }

        // SAME MOVE
        if (p1Move == p2Move)
        {
            if (Random.value < 0.5f)
                DamagePlayer2(Random.Range(12, 18));
            else
                DamagePlayer1(Random.Range(12, 18));

            yield return new WaitForSeconds(1.5f);
            yield break;
        }

        // LIGHT vs HEAVY
        if (p1Move == 0 && p2Move == 1)
        {
            if (roll < 0.6f)
                DamagePlayer2(Random.Range(12, 18));
            else
                DamagePlayer1(Random.Range(25, 35));
        }

        // HEAVY vs LIGHT
        else if (p1Move == 1 && p2Move == 0)
        {
            if (roll < 0.6f)
                DamagePlayer1(Random.Range(12, 18));
            else
                DamagePlayer2(Random.Range(25, 35));
        }

        // BLOCK vs HEAVY
        else if (p1Move == 2 && p2Move == 1)
        {
            int dmg = Mathf.RoundToInt(Random.Range(25, 35) * (1 - Random.Range(0.4f, 0.65f)));
            DamagePlayer1(dmg);
        }

        else if (p1Move == 1 && p2Move == 2)
        {
            int dmg = Mathf.RoundToInt(Random.Range(25, 35) * (1 - Random.Range(0.4f, 0.65f)));
            DamagePlayer2(dmg);
        }

        // LIGHT vs BLOCK
        else if (p1Move == 0 && p2Move == 2)
        {
            int dmg = Mathf.RoundToInt(Random.Range(12, 18) * (1 - Random.Range(0.4f, 0.65f)));
            DamagePlayer2(dmg);
        }

        else if (p1Move == 2 && p2Move == 0)
        {
            int dmg = Mathf.RoundToInt(Random.Range(12, 18) * (1 - Random.Range(0.4f, 0.65f)));
            DamagePlayer1(dmg);
        }

        yield return new WaitForSeconds(1.5f);
    }

    void DamagePlayer1(int dmg)
    {
        player1HP = Mathf.Max(0, player1HP - dmg);

        statusText.text += "\n" + GameData.player1Name + " loses " + dmg + " HP!";
        UpdateHPUI();
    }

    void DamagePlayer2(int dmg)
    {
        player2HP = Mathf.Max(0, player2HP - dmg);

        statusText.text += "\n" + GameData.player2Name + " loses " + dmg + " HP!";
        UpdateHPUI();
    }
}