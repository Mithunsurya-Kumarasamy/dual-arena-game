using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


public class APIManager : MonoBehaviour
{
    public bool lastLoginSuccess;
    public bool lastRegisterSuccess;
    
    string baseURL = "http://10.1.232.184";

    public IEnumerator Login(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(baseURL + "/auth/login", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string response = www.downloadHandler.text;
            Debug.Log("Response: " + response);

            lastLoginSuccess = response.Contains("true");
        }
        else
        {
            Debug.Log("Error: " + www.error);
            lastLoginSuccess = false;
        }
    }
    public IEnumerator Register(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(baseURL + "/auth/register", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            lastRegisterSuccess = www.downloadHandler.text.Contains("true");
        }
        else
        {
            lastRegisterSuccess = false;
        }
    }
}