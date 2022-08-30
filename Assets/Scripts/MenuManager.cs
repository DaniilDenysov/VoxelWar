using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public GameObject[] Window; //0-loading,1-mainmenu,2-findgame,3-creategame
    public GameObject [] AwaitingScreen;
    public Text version;
    [Header("References")]
    [SerializeField]
    public GameObject checkingForAccountUI, loginUI, registerUI, verifyEmailUI;
    [SerializeField]
    public Text verifyEmailText;

    void Start()
    {
        version.text = Application.version;
    }
    public void CloseWindow()
    {
        for (int i = 0; i < Window.Length - 1; i++)
        {
            Window[i].SetActive(false);
            Window[i].transform.parent.gameObject.SetActive(true);
        }
    }
    public void OpenWindow (int window)
    {
        for (int i = 1; i < Window.Length; i++)
        {
            if (i != window) Window[i].SetActive(false);
        }
        Window[window].SetActive(true);
    }
    public void AwaitingScreenActive(int screen)
    {
       for (int i = 0;i<AwaitingScreen.Length;i++)
        {
            if (i == screen) AwaitingScreen[i].SetActive(true);
            else AwaitingScreen[i].SetActive(false);
        }
    }
    public void BothAwaitingScreensActive()
    {
        AwaitingScreen[0].SetActive(true);
        AwaitingScreen[1].SetActive(true);
    }

    public void UpdateList()
    {
        SceneManager.LoadScene(0);
    }
    public void Quit ()
     {
         Application.Quit();
     }

}
