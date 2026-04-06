using UnityEngine.SceneManagement;
using UnityEngine;
using Unity;
using System.Collections;
public class AboutManager : MonoBehaviour
{

    public GameObject g1;
    public GameObject g2;
    public GameObject g3;
    public GameObject g4;
    public GameObject panel;

    public void cm1()
    {
        StartCoroutine(m1());
    }

    public void cm2()
    {
        StartCoroutine(m2());
    }

    public void cm3()
    {
        StartCoroutine(m3());
    }

    public void cm4()
    {
        StartCoroutine(m4());
    }
    public IEnumerator m1()
    {
        panel.SetActive(true);
        g1.SetActive(true);
        yield return new WaitForSeconds(2f);
        panel.SetActive(false);
        g1.SetActive(false);
    }

    public IEnumerator m2()
    {
        panel.SetActive(true);
        g2.SetActive(true);
        yield return new WaitForSeconds(2f);
        panel.SetActive(false);
        g2.SetActive(false);
    }
    public IEnumerator m3()
    {
        panel.SetActive(true);
        g3.SetActive(true);
        yield return new WaitForSeconds(2f);
        panel.SetActive(false);
        g3.SetActive(false);
    }
    public IEnumerator m4()
    {
        panel.SetActive(true);
        g4.SetActive(true);
        yield return new WaitForSeconds(2f);
        panel.SetActive(false);
        g4.SetActive(false);
    }

    public void back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
