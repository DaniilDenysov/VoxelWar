using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    public GameObject _player;
    [SerializeField] private Transform [] spawn;
    [SerializeField] private GameObject PlayerCamera, Cursor;
    [SerializeField] private GameObject [] PlayerCameraSpawned, CursorSpawned, _playerSpawned;
    public GameObject [] Body;
    int number = -1;
    public GameObject UI,_Camera;
    public GameObject LodoutScreen;
    public int[] loadoutChoice;
    public GameObject Cam;
    GameObject spawnedCam;
    public LodoutItem [] Lodout_list;
    Hashtable _customRoomProperties = new Hashtable();
    public RoundTimer timer;
    public Text RespawnTimer;
    public float StartTime;
    public float TimeToRespawn;


    public IEnumerator StartTimer()
    {
        TimeToRespawn = StartTime;
        while (TimeToRespawn >= 0)
        {
            RespawnTimer.text = $"{TimeToRespawn / 60:00}:{TimeToRespawn % 60:00}";
            TimeToRespawn--;
            yield return new WaitForSeconds(1f);
        }
        RespawnTimer.text = "Press F to continue";
    }





    void Start()
    {
      //Application.H

        if (GetComponent<PhotonView>().IsMine)
        {
            spawnedCam = Instantiate(Cam);
            LodoutScreen.SetActive(true);
        }
        /*if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.CustomProperties.Clear();
            _customRoomProperties.Add("Version", Application.version.ToString());
            _customRoomProperties.Add("Map", SceneManager.sceneCountInBuildSettings);
            PhotonNetwork.CurrentRoom.SetCustomProperties(_customRoomProperties);*/

    /*    ExitGames.Client.Photon.Hashtable setValue = new ExitGames.Client.Photon.Hashtable();
        setValue.Add("Map", SceneManager.sceneCountInBuildSettings);
        setValue.Add("Version", Application.version);
        PhotonNetwork.CurrentRoom.SetCustomProperties(setValue);*/

        Debug.Log("Map:" + PhotonNetwork.CurrentRoom.CustomProperties["Map"]);
            Debug.Log("Version:" + PhotonNetwork.CurrentRoom.CustomProperties["Version"]);
            Debug.Log("Time:" + (int)PhotonNetwork.CurrentRoom.CustomProperties["Time"]);
        
        timer.spawn = GetComponent<SpawnPlayers>();
       // float t = (float)PhotonNetwork.CurrentRoom.CustomProperties["Time"];
        if (PhotonNetwork.IsMasterClient) timer.remainingDuration = (int)PhotonNetwork.CurrentRoom.CustomProperties["Time"];
      //  Spawn();
    }
    public void AcceptLodout ()
    {
        LodoutScreen.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F) && UI != null && TimeToRespawn <= 0)
        {
            LodoutScreen.SetActive(true);
            //RE_Spawn();
        }
    }
    public void RE_Spawn()
    {
        if (UI.GetComponent<PhotonView>().IsMine) Destroy(UI.gameObject);
        if (_Camera.GetComponent<PhotonView>().IsMine) Destroy(_Camera.gameObject);
       // if (_Camera != null && _Camera.GetComponent<PhotonView>().IsMine) Destroy(_Camera.gameObject);
        for (int i = 0; i<Body.Length;i++)
        {
           if (Body[i] != null) PhotonNetwork.Destroy(Body[i]);
        }
       // _playerSpawned = new GameObject[_playerSpawned.Length + 1];
        GameObject spawned = PhotonNetwork.Instantiate(_player.name, spawn[Random.Range(0,spawn.Length - 1)].transform.position, Quaternion.Euler(0, 45, 0));
        for (int i = 0; i < Lodout_list.Length; i++)
        {
            if (Lodout_list[i].Chosen == true)
            {
                for (int m = 0; m < Lodout_list[i].ID_List.Length; m++)
                {
                    spawned.GetComponent<PlayerControll>().Items[m] = Lodout_list[i].ID_List[m];
                }
                break;
            }
        }
    }

    public void Spawn ()    
    {
         GameObject spawned = PhotonNetwork.Instantiate(_player.name, spawn[Random.Range(0, spawn.Length - 1)].transform.position, Quaternion.Euler(0,45,0));
        if (spawned.GetComponent<PhotonView>().IsMine == false)
            Destroy(spawned.GetComponent<PlayerControll>());
        if (spawnedCam != null)
            Destroy(spawnedCam);

        for (int i = 0;i < Lodout_list.Length; i++)
            {
                if (Lodout_list[i].Chosen == true)
                {
                    for (int m = 0; m < Lodout_list[i].ID_List.Length; m++)
                    {
                        spawned.GetComponent<PlayerControll>().Items[m] = Lodout_list[i].ID_List[m];
                    }  
                    break;
                }
            }
        if (UI != null && UI.GetComponent<PhotonView>().IsMine) Destroy(UI.gameObject);
        if (_Camera != null) Destroy(_Camera.gameObject);
        LodoutScreen.SetActive(false);
        /*   _customProperties["Kills"] = 0;
           _customProperties["Deaths"] = 0;
           PhotonNetwork.LocalPlayer.CustomProperties = _customProperties;*/

        /* GameObject spawnedCam = PhotonNetwork.Instantiate(PlayerCamera.name, spawn[Random.Range(0, spawn.Length - 1)].transform.position, Quaternion.Euler(0, 45, 0));
         // spawnedCam.transform.parent.GetComponent<DestroySmth>().HaveOwner = this.gameObject;
         spawnedCam.GetComponent<CameraDetection>().player = spawned.GetComponent<PlayerControll>();
         spawned.GetComponent<PlayerControll>().camera = spawnedCam.GetComponent<Camera>();
     }
     else
     {
         Destroy(spawned.GetComponent<PlayerControll>());
         Debug.Log("Player not mine");
     }*/
        // spawned.GetComponent<PlayerControll>().camera = PhotonNetwork.Instantiate(PlayerCamera.name, spawn[Random.Range(0, spawn.Length - 1)].transform.position, Quaternion.identity).GetComponentInChildren<Camera>();
    }


}
