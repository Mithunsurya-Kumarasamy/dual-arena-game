using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TournamentSetupManager : MonoBehaviour
{
    public TMP_Dropdown playerCountDropdown;
    public TMP_InputField tournName;

    public TextMeshProUGUI stat;
    public void OnNext()
    {

        if (string.IsNullOrWhiteSpace(tournName.text))
        {
            stat.text = "Enter tournament name!";
            return;
        }

        stat.text = "";
        int count = GetSelectedCount();

        GameData.tournamentPlayerCount = count;
        GameData.tournamentName = tournName.text;

        SceneManager.LoadScene("PlayerSelectionScene");
    }

    int GetSelectedCount()
    {
        switch (playerCountDropdown.value)
        {
            case 0: return 4;
            case 1: return 8;
            case 2: return 16;
        }
        return 4;
    }

    public void onNew()
    {
        SceneManager.LoadScene("TournamentScene");
    }

    public void onLoad()
    {
        SceneManager.LoadScene("LoadTournamentScene");
    }
    public void bak()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void bak1()
    {
        SceneManager.LoadScene("TournamentSelect");
    }
}