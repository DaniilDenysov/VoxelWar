using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    private static LobbyManager instance;

   

    [Header("UI References")]
    [SerializeField]
    private GameObject profileUI;
    [SerializeField]
    private GameObject changePfpUI;
    [SerializeField]
    private GameObject changeEmailUI;
    [SerializeField]
    private GameObject changePasswordUI;
    [SerializeField]
    private GameObject reverifyUI;
    [SerializeField]
    private GameObject resetPasswordConfirmUI;
    [SerializeField]
    private GameObject actionSuccessPanelUI;
    [SerializeField]
    private GameObject deleteUserConfirmUI;
    [Space(5f)]

    [Header("Basic Info References")]
    [SerializeField]
    private Text usernameText;
    [SerializeField]
    private Text emailText;
    [SerializeField]
    private string token;
    [Space(5f)]

    [Header("Profile Picture References")]
    [SerializeField]
    private Image profilePicture;
    [SerializeField]
    private InputField profilePictureLink;
    [SerializeField]
    private Text outputText;
    [Space(5f)]

    [Header("Change Email References")]
    [SerializeField]
    private InputField changeEmailEmailInputField;
    [Space(5f)]

    [Header("Change Password References")]
    [SerializeField]
    private InputField changePasswordInputField;
    [SerializeField]
    private InputField changePasswordConfirmInputField;
    [Space(5f)]

    [Header("Reverify References")]
    [SerializeField]
    private InputField reverifyEmailInputField;
    [SerializeField]
    private InputField reverifyPasswordInputField;
    [Space(5)]

    [Header("Action Success Panel References")]
    [SerializeField]
    private Text actionSuccessText;

    private void Start()
    {
    /*    if (DataManager.instance.user != null)
        {
          LoadProfile();
        }*/
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this);
        }
    }

    public void LoadProfile ()
    {
        if (DataManager.instance.user != null)
        {
            System.Uri photoUrl = DataManager.instance.user.PhotoUrl;
            string name = DataManager.instance.user.DisplayName;
            string email = DataManager.instance.user.Email;

            StartCoroutine(LoadImage(photoUrl.ToString()));
            usernameText.text = name;
            emailText.text = email;

        }
    }

    private IEnumerator LoadImage (string _photoUri)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(_photoUri);
        yield return request.SendWebRequest();
        if (request.error != null)
        {
            string output = "Unknown Error! Try again!";
            if (request.isHttpError)
            {
                output = "Image Type Not Supported! Please Try Another Image.";
            }

            Output(output);

        }
        else
        {
            Debug.Log("IMAGE!!!");
            Texture2D image = ((DownloadHandlerTexture)request.downloadHandler).texture;
            profilePicture.sprite = Sprite.Create(image,new Rect(0,0,image.width,image.height), Vector2.zero);
        }
    }
    public void Output (string _output)
    {
        outputText.text = _output;
    }

    public void ChangePfpSucccess ()
    {
        Debug.Log("Big success!");
        //act
    }

    public void SubmitProfileImage ()
    {
        GetComponent<DataManager>().UpdateProfilePicture(profilePictureLink.text);
        Debug.Log("SUBMITED!" + profilePictureLink.text);
    }
 
}
