using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;


public class UserNameManager : MonoBehaviour
{

    [SerializeField] InputField [] usernameInput;
    [SerializeField] Launcher mainmanager;
    [SerializeField] GameObject NickNameWindow;

    private void Start()
    {

    }

    public void OnUserNameValueChanged (int inputField)
    {
      if (usernameInput[inputField].text.Length > 0 && usernameInput[inputField].text.Length > 4)
        {
            PhotonNetwork.NickName = usernameInput[inputField].textComponent.text;
            PlayerPrefs.SetString("username",usernameInput[inputField].text);
            PlayerPrefs.Save();
            NickNameWindow.SetActive(false);
        }
        else
        {
            mainmanager.ErrorWindow.SetActive(true);
            mainmanager.ErrorWindow.GetComponentInChildren<Text>().text = "Username is too short";
        }
    }

}
