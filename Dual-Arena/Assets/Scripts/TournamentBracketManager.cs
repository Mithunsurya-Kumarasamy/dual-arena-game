using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TournamentBracketManager : MonoBehaviour
{
    public TextMeshProUGUI lbracketText;
    public TextMeshProUGUI rbracketText;
    public TextMeshProUGUI bracketText;
    public TextMeshProUGUI matchInfoText;

    public GameObject nextButton;
    public GameObject mmButton;

    public APIManager api;

    IEnumerator Start()
    {
        yield return StartCoroutine(api.GetTournamentMatches(GameData.currentTournamentID));

        if (api.fetchedMatches == null || api.fetchedMatches.Count == 0)
        {
            Debug.LogError("❌ No matches found in DB");
            yield break;
        }

        // rebuild GameData
        GameData.tournamentMatches.Clear();

        // 🔥 find latest round number
        int latestRound = -1;

        foreach (var m in api.fetchedMatches)
        {
            if (m.RoundNumber > latestRound)
                latestRound = m.RoundNumber;
        }
        GameData.currentRound = latestRound;

        // 🔥 only load matches from latest round
        foreach (var m in api.fetchedMatches)
        {
            if (m.RoundNumber == latestRound)
            {
                MatchData match = new MatchData();
                match.player1 = m.Player1;
                match.player2 = m.Player2;
                match.winner = m.Winner;

                GameData.tournamentMatches.Add(match);
            }
        }

        // rebuild player list from matches
        HashSet<string> players = new HashSet<string>();

        foreach (var m in GameData.tournamentMatches)
        {
            players.Add(m.player1);
            players.Add(m.player2);
        }

        GameData.tournamentPlayers = new List<string>(players);
        // 🔥 RESUME LOGIC

        GameData.tournamentMatchIndex = 0;

        // find first unplayed match
        for (int i = 0; i < GameData.tournamentMatches.Count; i++)
        {
            if (string.IsNullOrEmpty(GameData.tournamentMatches[i].winner))
            {
                GameData.tournamentMatchIndex = i;
                break;
            }

            // if all matches completed
            GameData.tournamentMatchIndex = GameData.tournamentMatches.Count;
        }

        // ✅ CHECK IF ROUND COMPLETED
        // ✅ CHECK IF ROUND COMPLETED
        if (GameData.tournamentMatchIndex >= GameData.tournamentMatches.Count)
        {
            bool nextRoundExists = false;

            foreach (var m in api.fetchedMatches)
            {
                if (m.RoundNumber == GameData.currentRound - 1)
                {
                    nextRoundExists = true;
                    break;
                }
            }

            if (!nextRoundExists)
            {
                Debug.Log("🔥 Generating next round (LOAD)");
                GenerateNextRound();
            }
            else
            {
                Debug.Log("✅ Next round already exists → loading it");

                GameData.currentRound--;

                GameData.tournamentMatches.Clear();

                foreach (var m in api.fetchedMatches)
                {
                    if (m.RoundNumber == GameData.currentRound)
                    {
                        MatchData match = new MatchData();
                        match.player1 = m.Player1;
                        match.player2 = m.Player2;
                        match.winner = m.Winner;

                        GameData.tournamentMatches.Add(match);
                    }
                }
            }

            GameData.tournamentMatchIndex = 0;
        }
        // ✅ CHECK IF TOURNAMENT FINISHED
        if (GameData.tournamentMatches.Count == 1 &&
            !string.IsNullOrEmpty(GameData.tournamentMatches[0].winner))
        {
            GameData.tournamentFinished = true;
            GameData.tournamentWinner = GameData.tournamentMatches[0].winner;

            nextButton.SetActive(false);

            bracketText.text = "Tournament Over!";
            matchInfoText.text = "CHAMPION: " + GameData.tournamentWinner;

            yield break; // 🚨 IMPORTANT
        }

        // 🔥 AFTER ALL LOGIC → UI
        DisplayBracket();
        UpdateCurrentMatch();
    }
    void HandleWinnerReturn()
    {
        if (GameData.tournamentFinished) return; // 🛑 STOP HERE

        if (string.IsNullOrEmpty(GameData.lastWinner)) return;

        RecordWinner(GameData.lastWinner);

        GameData.lastWinner = "";
    }
    public void RecordWinner(string winner)
    {
        var match = GameData.tournamentMatches[GameData.tournamentMatchIndex];

        match.winner = winner;

        Debug.Log("✅ Winner set for: " + match.player1 + " vs " + match.player2);

        GameData.tournamentMatchIndex++;

        if (AllMatchesCompleted())
        {
            GenerateNextRound();
            GameData.tournamentMatchIndex = 0;
        }

        DisplayBracket();
        UpdateCurrentMatch();

        Debug.Log("Match Index: " + GameData.tournamentMatchIndex);
        Debug.Log("Total Matches: " + GameData.tournamentMatches.Count);
        GameData.winnerProcessed = true;
    }
    void GenerateNextRound()
    {
        List<string> winners = new List<string>();

        foreach (var match in GameData.tournamentMatches)
        {
            winners.Add(match.winner);
        }

        // 🏆 FINAL WINNER
        if (winners.Count == 1)
        {
            Debug.Log("TOURNAMENT WINNER: " + winners[0]);

            GameData.tournamentWinner = winners[0]; // ✅ STORE CORRECTLY
            GameData.tournamentFinished = true;

            return;
        }
        GameData.tournamentMatches.Clear();

        for (int i = 0; i < winners.Count; i += 2)
        {
            MatchData m = new MatchData();
            m.player1 = winners[i];
            m.player2 = winners[i + 1];

            GameData.tournamentMatches.Add(m);
        }
        GameData.currentRound--;
        StartCoroutine(SendNextRoundToDB());

        GameData.tournamentPlayers = winners;
    }

    IEnumerator SendNextRoundToDB()
    {
        List<PlayerSelectionManager.MatchSend> matchList = new List<PlayerSelectionManager.MatchSend>();

        for (int i = 0; i < GameData.tournamentMatches.Count; i++)
        {
            var m = GameData.tournamentMatches[i];

            PlayerSelectionManager.MatchSend data = new PlayerSelectionManager.MatchSend();
            data.player1 = m.player1;
            data.player2 = m.player2;
            data.roundNumber = GameData.currentRound; // or track round separately
            data.matchOrder = i;

            matchList.Add(data);
        }

        PlayerSelectionManager.MatchWrapper wrapper = new PlayerSelectionManager.MatchWrapper();
        wrapper.matches = matchList;

        string json = JsonUtility.ToJson(wrapper);

        WWWForm form = new WWWForm();
        form.AddField("tournamentId", GameData.currentTournamentID);
        form.AddField("matches", json);

        UnityWebRequest www = UnityWebRequest.Post(
            "http://localhost:3000/tournament/createMatches", form);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
            Debug.Log("✅ Next round saved");
        else
            Debug.LogError("❌ Failed to save next round");
    }
    string GetRoundName(int roundNumber)
    {
        if (roundNumber == 4) return "Round of 16";
        if (roundNumber == 3) return "Quarter Final";
        if (roundNumber == 2) return "Semi Final";
        if (roundNumber == 1) return "Final";

        return "Match";
    }

    bool AllMatchesCompleted()
    {
        foreach (var match in GameData.tournamentMatches)
        {
            Debug.Log("Checking winner: " + match.winner);

            if (string.IsNullOrEmpty(match.winner))
                return false;
        }
        return true;
    }
    void DisplayBracket()
    {
        int total = GameData.tournamentPlayers.Count;
        string roundName = GetRoundName(GameData.currentRound);

        // Clear all
        lbracketText.text = "";
        rbracketText.text = "";
        bracketText.text = roundName + "\n\n";

        int matchCount = GameData.tournamentMatches.Count;

        // FINAL → center only
        if (matchCount == 1)
        {
            var match = GameData.tournamentMatches[0];

            bracketText.text +=
                roundName + "\n" +
                match.player1 + " vs " + match.player2;

            return;
        }

        // SPLIT LEFT & RIGHT
        int half = matchCount / 2;


        lbracketText.text = roundName + "\n\n" + lbracketText.text;
        for (int i = 0; i < matchCount; i++)
        {
            var match = GameData.tournamentMatches[i];

            string p1 = GetColoredName(match.player1, match.winner);
            string p2 = GetColoredName(match.player2, match.winner);

            string matchText =
                "M" + (i + 1) + "\n" +
                p1 + " vs " + p2 + "\n\n";
            if (i == GameData.tournamentMatchIndex)
            {
                matchText = "<b><color=yellow>" + matchText + "</color></b>";
            }

            if (i < half)
                lbracketText.text += matchText;
            else
                rbracketText.text += matchText;
        }
    }
    void UpdateCurrentMatch()
    {
        int total = GameData.tournamentPlayers.Count;
        string roundName = GetRoundName(GameData.currentRound);

        if (GameData.tournamentMatches == null || GameData.tournamentMatches.Count == 0)
        {
            Debug.LogError("❌ No matches available");
            return;
        }

        if (GameData.tournamentMatchIndex >= GameData.tournamentMatches.Count)
        {
            Debug.LogError("❌ Index out of range: " + GameData.tournamentMatchIndex);
            return;
        }

        var match = GameData.tournamentMatches[GameData.tournamentMatchIndex];

        matchInfoText.text =
            "Next Game:\n" + roundName + " - Match " + (GameData.tournamentMatchIndex + 1) + "\n" +
            match.player1 + " vs " + match.player2;
    }
    string GetColoredName(string player, string winner)
    {
        if (string.IsNullOrEmpty(winner))
            return player; // not played yet

        if (player == winner)
            return "<color=green>" + player + "</color>";

        return "<color=red>" + player + "</color>";
    }

    public void mm()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void StartMatch()
    {
        var match = GameData.tournamentMatches[GameData.tournamentMatchIndex];

        GameData.player1Name = match.player1;
        GameData.player2Name = match.player2;
        SceneManager.LoadScene("MapSelection");
    }
}