using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun;
using Firebase.Auth;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class Launcher : MonoBehaviourPunCallbacks
{
    public bool JoinedLobby;
    public static Launcher Instance;
    [SerializeField] private GameObject [] loadingScreens;
    [SerializeField] private InputField roomNameInputField, usernameInput;
    [SerializeField] private MenuManager manager;
    [SerializeField] Transform roomListContent;
    public GameObject pref, ErrorWindow;
    [SerializeField] private float TimeBetweenUpdates = 1.5f;
    float nextTimeToUpdate;
    [SerializeField] Text timer,playersCount,respawnCount;
    [SerializeField] Slider timeValue,playerValue,respawnValue;
    public string Ch;
    [SerializeField] private Animator cameraAnimator;

    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
  
        //PhotonNetwork.U
        PhotonNetwork.GameVersion = Application.version;
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        //loadingScreens[0].SetActive(false);
        //loadingScreens[1].SetActive(false);
        //  manager.AwaitingScreenActive(1);
        //manager.OpenWindow(5);
        Debug.Log("ConnectedToMaster");
        PhotonNetwork.JoinLobby();
    }
    public void JoinRoom(RoomInfo info)
    {
         if (info.CustomProperties["Version"].ToString() == Application.version.ToString())
        {
            PhotonNetwork.NickName = FirebaseAuth.DefaultInstance.CurrentUser.DisplayName;
            PhotonNetwork.JoinRoom(info.Name);
         
               /// GetComponent<MenuManager>().OpenWindow(0);

         }
         else
         {
               OnErrorWindow("The owner of room has different game version");
         }
    }
    public void OnErrorWindow (string message)
    {
        ErrorWindow.SetActive(true);
        ErrorWindow.GetComponentInChildren<Text>().text = message;
    }
    public void CameraSwipe (string Name)
    {   
        cameraAnimator.Play(Name);
    }
    public void OnRespawnTimeChange()
    {
        respawnCount.text = "_Respawn time:" + respawnValue.value;
    }
    public void OnTimeChange ()
    {
        timer.text = "_Time:" + timeValue.value;
    }
    public void OnPlayerValueChange()
    {
        playersCount.text = "_Players:" + playerValue.value;
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Loading(false);
        ErrorWindow.SetActive(true);
        ErrorWindow.GetComponentInChildren<Text>().text = "Error";
    }
  
    public override void OnJoinedLobby()
    {
        JoinedLobby = true;
        Debug.Log("Joined lobby");
    }
    public void Loading(bool state)
    {
        loadingScreens[0].SetActive(state);
        loadingScreens[1].SetActive(state);
    }
    public void CreateRoom ()
    {
        if (roomNameInputField.textComponent.text.Length > 4)
        {
            if (string.IsNullOrEmpty(roomNameInputField.text))
            {
                return;
            }
            // manager.AwaitingScreen[0].SetActive(false);
            PhotonNetwork.NickName = FirebaseAuth.DefaultInstance.CurrentUser.DisplayName;
            Loading(true);
            RoomOptions options = new RoomOptions();
            Hashtable customProperties = new Hashtable();
            customProperties.Add("Version", Application.version.ToString());
            customProperties.Add("Map", 1);
            customProperties.Add("Time", (int)timeValue.value);
            customProperties.Add("respawnTime", (int)respawnValue.value);
            options.CustomRoomProperties = customProperties;
            options.CustomRoomPropertiesForLobby = new string[] { "Version", "Map","Time","respawnTime" };
            options.MaxPlayers = (byte)playerValue.value;
            options.IsVisible = true;          
           // options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            PhotonNetwork.CreateRoom("_" + roomNameInputField.text, options,TypedLobby.Default);
         //   options.CustomRoomProperties.Add("Version", Application.version);
          //  options.CustomRoomProperties.Add("Map", 1);
           
        }
        else
        {
            if (usernameInput.textComponent.text.Length == 0)
            {
                ErrorWindow.SetActive(true);
                ErrorWindow.GetComponentInChildren<Text>().text = "Username is too short";
            }
            else
            {
                ErrorWindow.SetActive(true);
                ErrorWindow.GetComponentInChildren<Text>().text = "The room name is too short";
            }
        }
    }
  
    public void CloseErrorWindow ()
    {
        ErrorWindow.SetActive(false);
    }
    public override void OnJoinedRoom()
    {
        //(int)PhotonNetwork.CurrentRoom.CustomProperties["Map"]
        PhotonNetwork.LoadLevel(1);
    }

    public new void OnCreateRoomFailed(short returnCode, string message)
    {
        GetComponent<MenuManager>().OpenWindow(3);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
          foreach (Transform trans in roomListContent)
                  Destroy(trans.gameObject);

          for (int i = 0; i < roomList.Count; i++)    
          {
            if (roomList[i].RemovedFromList)
                continue;

            GameObject item = Instantiate(pref, roomListContent);
                item.GetComponent<RoomListItem>().SetUp(roomList[i]);
          }
    }
}
