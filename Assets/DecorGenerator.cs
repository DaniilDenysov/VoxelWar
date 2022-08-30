using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;

public class DecorGenerator : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject [] pref;
    [SerializeField] private Transform [] spawnPoint;
    public int DecorCount;
    public int[] spawnedID;
    [SerializeField] private GameObject [] spawned;
    

    private void Awake()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < DecorCount; i++)
            {
                if (!spawned[i]) GetComponent<PhotonView>().RPC("SpawnDecor", RpcTarget.AllBuffered, i,Random.Range(0, pref.Length - 1));
                else return;
            }
        }
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient == PhotonNetwork.LocalPlayer)
        {
            for (int i = 0; i < spawnedID.Length; i++)
            {
                GetComponent<PhotonView>().RPC("SpawnDecor", RpcTarget.AllBuffered, i, spawnedID[i]);
            }
        }
    }

    /* public override void OnPlayerLeftRoom(Player otherPlayer)
     {
         if (PhotonNetwork.IsMasterClient)
         {
             for (int i = 0; i < DecorCount; i++)
             {
                  GetComponent<PhotonView>().RPC("SpawnDecor", RpcTarget.AllBuffered, i, spawned[i]);             
             }
         }
     }*/

    [PunRPC]
    public void SpawnDecor (int spawnID,int rand)
    {
        if (spawned[spawnID] == null) { spawned[spawnID] = Instantiate(pref[rand], spawnPoint[spawnID].transform.position, Quaternion.identity); spawnedID[spawnID] = rand; Debug.Log("Spawned!"); }
    }

}
