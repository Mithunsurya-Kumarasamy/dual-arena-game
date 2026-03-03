using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void Load1v1()
    {
        SceneManager.LoadScene("DualScene");
    }

    public void LoadTournament()
    {
        SceneManager.LoadScene("TournamentScene");
    }

    public void LoadStats()
    {
        SceneManager.LoadScene("StatsScene");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }
}