using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class SpawnAmmo : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform[] points;
    [SerializeField] private GameObject[] Pref, Spawned;
    [SerializeField] private float TimeBetweenSpawns;
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient) InvokeRepeating("Spawn",TimeBetweenSpawns, TimeBetweenSpawns);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
       if (PhotonNetwork.IsMasterClient) InvokeRepeating("Spawn", TimeBetweenSpawns, TimeBetweenSpawns);
    }

    void Spawn ()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (Spawned[i] == null)
                {
                    Spawned[i] = PhotonNetwork.Instantiate(Pref[0].name, points[i].transform.position, Quaternion.identity);
                    break;
                }
            }
        }
    }   

}
