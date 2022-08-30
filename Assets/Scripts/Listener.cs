using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Firebase.Database;
using UnityEngine;

public class Listener : MonoBehaviour
{
    public bool Called;
    public string Message;

    public void OnlineValueChanged(object sender, ValueChangedEventArgs args)
    {

        //instance != null && 

        if (Called) Called = false;
        else
        {
            Debug.Log("Listener called!");
            if (args.Snapshot.Exists)
            {
                if (!(bool)args.Snapshot.Value == false)
                {
                    PhotonNetwork.LeaveRoom();
                    PhotonNetwork.LoadLevel(0);
                    Message = "Other client connected from other device!";
                }
                else Message = "Player data have been changed or deleted!";

            }
        }
        //PhotonNetwork.LoadLevel(0);
    }

}
