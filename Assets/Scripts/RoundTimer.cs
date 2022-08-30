using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class RoundTimer : MonoBehaviourPunCallbacks
{
    public Text Timer;
    public float RoundTime;
    //[SerializeField] private Player player;
    [SerializeField] private GameObject EndGameWindow;
    public bool GameStopped;
    [SerializeField] private Transform List;
    public Scoreboard score;
    // public PlayerControll player;
    public SpawnPlayers spawn;
    public PlayerControll player;
    public int duration;
    public int remainingDuration;
    public GameObject CreateNewRoomButton, ConnectToNewRoomButton,LoadingScreen;
    RoomOptions options;
    private void Start()
    {     
        StartCoroutine(UpdateTimer());
        Timer.text = $"{remainingDuration / 60:00}:{remainingDuration % 60:00}";
    }
    [PunRPC]
    public void Begin (int second)
    {
        remainingDuration = second;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GetComponent<PhotonView>().RPC("Begin", RpcTarget.All, remainingDuration);
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GetComponent<PhotonView>().RPC("Begin", RpcTarget.All, remainingDuration);
        }
    }

    public IEnumerator UpdateTimer ()
    {
        while (remainingDuration >= 0)
        {
            Timer.text = $"{remainingDuration / 60:00}:{remainingDuration % 60:00}";
            remainingDuration--;
            yield return new WaitForSeconds(1f);
        }
        OnEnd();
    }

    void OnEnd()
    {
            GameStopped = true;
        if (PhotonNetwork.IsMasterClient)  GetComponent<PhotonView>().RPC("StopGame", RpcTarget.All);
    }

    public void Leave ()
    {
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

    public void CreateRoom ()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            LoadingScreen.SetActive(true);
            GetComponent<PhotonView>().RPC("OnCreateRoomButtonActive",RpcTarget.All,options);
            PhotonNetwork.CreateRoom(PhotonNetwork.CurrentRoom.Name, options, TypedLobby.Default);
        }
    }

    [PunRPC]
    public void OnCreateRoomButtonActive (RoomOptions op)
    {
        options = op;
        ConnectToNewRoomButton.SetActive(true);
    }

    
    public void ConnetToNewRoom ()
    {
        LoadingScreen.SetActive(true);
        PhotonNetwork.JoinRoom(PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.NickName = PhotonNetwork.LocalPlayer.NickName;
    }

    [PunRPC]
    public void StopGame()
    {
        GameStopped = true;
        Cursor.visible = true;
        score.EndList(List);
        if (PhotonNetwork.IsMasterClient)
        {
           /* RoomOptions options = new RoomOptions();
            Hashtable customProperties = new Hashtable();
            customProperties.Add("Version", PhotonNetwork.CurrentRoom.CustomProperties["Version"]);
            customProperties.Add("Map", PhotonNetwork.CurrentRoom.CustomProperties["Map"]);
            customProperties.Add("Time", PhotonNetwork.CurrentRoom.CustomProperties["Time"]);
            customProperties.Add("respawnTime", PhotonNetwork.CurrentRoom.CustomProperties["respawnTime"]);
            options.CustomRoomProperties = customProperties;
            options.CustomRoomPropertiesForLobby = new string[] { "Version", "Map", "Time", "respawnTime" };
            options.MaxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
            options.IsVisible = true;
            // options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            CreateNewRoomButton.SetActive(true);*/
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        if (spawn.LodoutScreen)
        {
            spawn.LodoutScreen.SetActive(false);
        }
        if (spawn.UI)
        {
         
            Destroy(spawn.UI);
        }
        if (player != null && player.FastAccessTab != null)
        {
            player.FastAccessTab.SetActive(false);
            //player.Inventory.SetActives(false);
        }
        EndGameWindow.SetActive(true);
    }

}
