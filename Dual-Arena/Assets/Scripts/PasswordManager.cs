using UnityEngine;
using TMPro;
using System.Collections;
public class PasswordManager : MonoBehaviour
{
    public GameObject panel;
    public TMP_InputField inputField;

    PlayerRow currentRow;

    public void Open(PlayerRow row)
    {
        panel.SetActive(true);
        inputField.text = "";
        currentRow = row;
    }


    public APIManager api;

    public void Confirm()
    {
        string entered = inputField.text;
        string username = currentRow.GetSelectedUsername();

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
            Debug.Log("Wrong password");
        }
    }
    public void Close()
    {
        panel.SetActive(false);
    }
}