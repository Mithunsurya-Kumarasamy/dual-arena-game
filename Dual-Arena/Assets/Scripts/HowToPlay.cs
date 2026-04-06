using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;





public class HowToPlay : MonoBehaviour
{
    [System.Serializable]
    public class RulesData
    {

        public string Title;
        public string LeftText;
        public string RightText;
    }
    public APIManager api;
    public GameObject panel;
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rrule;
    public TextMeshProUGUI rightText;
    public TextMeshProUGUI rulesText;
    public GameObject BackBtn;
    public GameObject b1;
    public GameObject b2;
    public GameObject b3;

    public void OpenPanel()
    {
        panel.SetActive(true);
        rulesText.text = "";
    }

    public void ClosePanel()
    {
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator LoadRules(string type)
    {
        panel.SetActive(true);
        b1.SetActive(false);
        b2.SetActive(false);
        b3.SetActive(false);
        BackBtn.SetActive(true);

        leftText.gameObject.SetActive(true);
        rightText.gameObject.SetActive(true);

        yield return StartCoroutine(api.GetRules(type));

        RulesData data =
            JsonUtility.FromJson<RulesData>(api.rulesJSON);

        rrule.text = data.Title;
        leftText.text = data.LeftText;
        rightText.text = data.RightText;
    }
    public void Show1v1Rules()
    {
        StartCoroutine(LoadRules("1v1"));
    }

    public void ShowTournamentRules()
    {
        StartCoroutine(LoadRules("tournament"));
    }

    public void BackGo()
    {
        rrule.text = "Rules";
        rulesText.text = "Rules";
        b1.SetActive(true);
        b2.SetActive(true);
        b3.SetActive(true);
        BackBtn.SetActive(false);
        panel.SetActive(false);

        leftText.gameObject.SetActive(false);
        rightText.gameObject.SetActive(false);

    }
}