using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerSelectionManager : MonoBehaviour
{
    public Transform leftColumn;
    public Transform rightColumn;
    public GameObject playerRowPrefab;
    public GameObject registerPanel;

    public List<string> selectedPlayers = new List<string>();
    public PasswordManager passwordManager;
    public int totalPlayers;
    private int lockedPlayers = 0;

    public Button nextButton;
    [System.Serializable]
    public class MatchSend
    {
        public string player1;
        public string player2;
        public int roundNumber;
        public int matchOrder;
    }

    [System.Serializable]
    public class MatchWrapper
    {
        public List<MatchSend> matches;
    }
    void Start()
    {


        int count = GameData.tournamentPlayerCount;
        if (count == 0) count = 4;

        totalPlayers = count;

        GeneratePlayerRows(count);

        StartCoroutine(LoadUsers()); // ✅ AFTER rows created

        nextButton.interactable = false;
    }

    IEnumerator LoadUsers()
    {
        yield return StartCoroutine(passwordManager.api.GetUsers());

        Debug.Log("Users loaded: " + passwordManager.api.fetchedUsers.Count);

        PopulateDropdowns(passwordManager.api.fetchedUsers);
    }
    void PopulateDropdowns(List<string> users)
    {
        foreach (Transform col in new[] { leftColumn, rightColumn })
        {
            foreach (Transform row in col)
            {
                PlayerRow r = row.GetComponentInChildren<PlayerRow>();
                r.dropdown.ClearOptions();
                r.dropdown.AddOptions(users);
            }
        }
    }
    public void OnNextPressed()
    {
        GameData.tournamentPlayers = new List<string>(selectedPlayers);

        StartCoroutine(StartTournamentFlow()); // ✅ THIS WAS MISSING
    }
    
    IEnumerator StartTournamentFlow()
    {
        GameData.currentRound = 1;

        // 🔥 CREATE TOURNAMENT FIRST
        yield return StartCoroutine(
            passwordManager.api.CreateTournament(
                GameData.tournamentName,   // you must store this earlier
                GameData.tournamentPlayerCount
            )
        );

        GameData.currentTournamentID = passwordManager.api.createdTournamentID;

        Debug.Log("🎯 Tournament ID: " + GameData.currentTournamentID);

        // THEN generate matches
        GenerateBracket();

        GameData.tournamentMatchIndex = 0;

        yield return StartCoroutine(SendTournamentMatches());

        SceneManager.LoadScene("TournamentMatchScene");
    }
    IEnumerator SendTournamentMatches()
    {
        List<MatchSend> matchList = new List<MatchSend>();

        for (int i = 0; i < GameData.tournamentMatches.Count; i++)
        {
            var m = GameData.tournamentMatches[i];

            MatchSend data = new MatchSend();
            data.player1 = m.player1;
            data.player2 = m.player2;
            data.roundNumber = GameData.currentRound;
            data.matchOrder = i;

            matchList.Add(data);
        }

        MatchWrapper wrapper = new MatchWrapper();
        wrapper.matches = matchList;

        string json = JsonUtility.ToJson(wrapper);

        WWWForm form = new WWWForm();
        form.AddField("tournamentId", GameData.currentTournamentID);
        form.AddField("matches", json);

        UnityWebRequest www = UnityWebRequest.Post(
            "http://localhost:3000/tournament/createMatches", form);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Matches saved to DB");
        }
        else
        {
            Debug.LogError("❌ Failed to save matches: " + www.error);
        }
    }
    void GenerateBracket()
    {
        GameData.tournamentMatches.Clear();

        List<string> players = new List<string>(selectedPlayers);

        // 🔀 shuffle players
        for (int i = 0; i < players.Count; i++)
        {
            string temp = players[i];
            int rand = Random.Range(i, players.Count);
            players[i] = players[rand];
            players[rand] = temp;
        }

        // 🎯 create matches
        for (int i = 0; i < players.Count; i += 2)
        {
            MatchData match = new MatchData();
            match.player1 = players[i];
            match.player2 = players[i + 1];

            GameData.tournamentMatches.Add(match);
        }

        foreach (var m in GameData.tournamentMatches)
        {
            Debug.Log(m.player1 + " vs " + m.player2);
        }
    }

    public void AddPlayer(string username)
    {
        if (!selectedPlayers.Contains(username))
        {
            selectedPlayers.Add(username);
        }
    }

    public void OpenRegister()
    {
        registerPanel.SetActive(true);
    }
    IEnumerator RefreshDelay()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(LoadUsers());
    }
    public void CloseRegister()
    {
        registerPanel.SetActive(false);
    }
    public void RefreshUsers()
    {
        StartCoroutine(LoadUsers());
    }
    public void GoBack()
    {
        SceneManager.LoadScene("TournamentScene"); // or previous scene
    }
    public void OnPlayerLocked()
    {
        lockedPlayers++;

        if (lockedPlayers >= totalPlayers)
        {
            nextButton.interactable = true; // ✅ enable next
        }
    }

    void GeneratePlayerRows(int count)
    {
        // clear old
        foreach (Transform child in leftColumn) Destroy(child.gameObject);
        foreach (Transform child in rightColumn) Destroy(child.gameObject);

        int half = count / 2;

        for (int i = 0; i < count; i++)
        {
            Transform parent = (i < half) ? leftColumn : rightColumn;

            GameObject rowObj = Instantiate(playerRowPrefab, parent);

            PlayerRow row = rowObj.GetComponentInChildren<PlayerRow>();
            row.SetPlayerNumber(i + 1);

            // TEMP usernames
            row.passwordManager = passwordManager;
            row.dropdown.ClearOptions();
            row.manager = this;
            row.dropdown.ClearOptions(); // leave empty

        }
    }
}