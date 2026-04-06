using UnityEngine;
using TMPro;
using System.Collections;
public class PasswordManager : MonoBehaviour
{
    public GameObject panel;
    public TMP_InputField inputField;

    public TextMeshProUGUI stat;

    PlayerRow currentRow;

    public void Open(PlayerRow row)
    {
        panel.SetActive(true);
        inputField.text = "";
        stat.text = "";
        currentRow = row;
    }


    public APIManager api;

    public void Confirm()
    {
        string entered = inputField.text;
        string username = currentRow.GetSelectedUsername();


        if (string.IsNullOrWhiteSpace(entered))
        {
            stat.text = "Enter password!";
            return;
        }   
        stat.text = "Loading...";
        StartCoroutine(LoginRoutine(username, entered));
    }

    IEnumerator LoginRoutine(string username, string password)
    {
        yield return StartCoroutine(api.Login(username, password));

        if (api.lastLoginSuccess)
        {
            currentRow.LockPlayer();
            panel.SetActive(false);
        }
        else
        {
            stat.text = "Incorrect Password Entered!";
            yield break;
        }
    }
    public void Close()
    {
        panel.SetActive(false);
    }
}