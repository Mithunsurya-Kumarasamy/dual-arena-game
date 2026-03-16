using UnityEngine;

public class RoundSimulator : MonoBehaviour
{
    public int player1HP = 100;
    public int player2HP = 100;

    public void ResolveRound(int p1Move, int p2Move)
    {
        Debug.Log("Resolving round");

        float roll = Random.Range(0f,1f);

        if (p1Move == p2Move)
        {
            Debug.Log("Same move clash!");
            return;
        }

        if(p1Move == 0 && p2Move == 1) // Light vs Heavy
        {
            if(roll < 0.6f)
                DamagePlayer2();
            else
                DamagePlayer1();
        }
    }

    void DamagePlayer1()
    {
        int dmg = Random.Range(8,13);
        player1HP -= dmg;

        Debug.Log("Player1 took " + dmg + " damage");
    }

    void DamagePlayer2()
    {
        int dmg = Random.Range(8,13);
        player2HP -= dmg;

        Debug.Log("Player2 took " + dmg + " damage");
    }
}