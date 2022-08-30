using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

public class ScoreboardListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] Text PlayerNick, PlayerKills, PlayerDeaths;


   // PlayerControll 
    void Awake ()
    {
       // SetUp(PhotonNetwork.LocalPlayer);
    }

    //[PunRPC]
    public void SetUp(Player player)
    {
        PlayerNick.text = player.NickName;
        if (player.IsLocal)
        {
            PlayerNick.color = Color.green;
            PlayerKills.color = Color.green;
            PlayerDeaths.color = Color.green;
        }
        if (player.CustomProperties["Kills"] != null)
        {
            PlayerKills.text = "" + player.CustomProperties["Kills"];
        }
        else
        {
            PlayerKills.text = "0";
        }
        if (player.CustomProperties["Deaths"] != null)
        {
            PlayerDeaths.text = "" + player.CustomProperties["Deaths"];
        }
        else
        {
            PlayerDeaths.text = "0";
        }
        Debug.Log("Updated");
        
        /* info = _info;
         text.text = _info.Name + " " + info.PlayerCount + "/" + info.MaxPlayers;*/
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

    //    SetUp(targetPlayer);
    }



    void Update()
    {
        
    }
}
