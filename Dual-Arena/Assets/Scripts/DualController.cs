using UnityEngine;

public class DualController : MonoBehaviour
{
    public GameObject player1Shield;
    public GameObject player2Shield;

    public void Player1Block()
    {
        player1Shield.SetActive(true);
        Invoke("HideP1Shield", 0.5f);
    }

    void HideP1Shield()
    {
        player1Shield.SetActive(false);
    }

    public void Player2Block()
    {
        player2Shield.SetActive(true);
        Invoke("HideP2Shield", 0.5f);
    }

    void HideP2Shield()
    {
        player2Shield.SetActive(false);
    }
}