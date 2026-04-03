using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LoadTournamentManager : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public APIManager api;

    List<int> tournamentIDs = new List<int>();

    void Start()
    {
        StartCoroutine(LoadTournaments());
    }

    IEnumerator LoadTournaments()
    {
        yield return StartCoroutine(api.GetTournaments());

        dropdown.ClearOptions();
        tournamentIDs.Clear();

        List<string> options = new List<string>();

        foreach (var t in api.fetchedTournaments)
        {
            options.Add(t.TournamentName);
            tournamentIDs.Add(t.TournamentID);
        }

        dropdown.AddOptions(options);
    }

    public void GoNext()
    {
        int index = dropdown.value;

        GameData.currentTournamentID = tournamentIDs[index];

        Debug.Log("Selected Tournament ID: " + GameData.currentTournamentID);

        SceneManager.LoadScene("TournamentMatchScene");
    }

    public void Back()
    {
        SceneManager.LoadScene("TournamentSelect");
    }
}