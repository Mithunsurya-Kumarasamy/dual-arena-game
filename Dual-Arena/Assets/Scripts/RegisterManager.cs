using UnityEngine;
using TMPro;
using System.Collections;
public class RegisterManager : MonoBehaviour
{
    public GameObject panel;

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public TextMeshProUGUI feedbackText;
    public PlayerSelectionManager playerSelectionManager;

    public void Open()
    {
        panel.SetActive(true);
        feedbackText.text = "";
        usernameInput.text = "";
        passwordInput.text = "";
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    public APIManager api;

    public void Register()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Enter username & password";
            return;
        }

        StartCoroutine(RegisterRoutine(username, password));
    }

    IEnumerator RegisterRoutine(string username, string password)
    {
        yield return StartCoroutine(api.Register(username, password));

        if (api.lastRegisterSuccess)
        {
            feedbackText.text = "Registered successfully!";
            Invoke(nameof(Close), 1.5f);
             playerSelectionManager.RefreshUsers();
        }
        else
        {
            feedbackText.text = "Registration failed";
        }
    }
}