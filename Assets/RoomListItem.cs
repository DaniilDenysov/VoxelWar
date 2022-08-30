using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomListItem : MonoBehaviourPunCallbacks, IPointerClickHandler
{
    [SerializeField] Sprite [] map;
    [SerializeField] Text text;
    [SerializeField] Image Map_Image;
    RoomInfo info;


    public void OnPointerClick(PointerEventData eventData)
    {
        Launcher.Instance.JoinRoom(info);
    }

    public void SetUp(RoomInfo _info)
    {
        info = _info;
        Debug.Log("Info:" + info.Name);
        text.text = _info.Name + " " + info.PlayerCount + "/" + info.MaxPlayers;
        if (info.CustomProperties["Version"].ToString() != Application.version.ToString()) text.color = Color.red;
       //Debug.Log("Version:" + info.CustomProperties["Version"]);
        Map_Image.sprite = map[(int)_info.CustomProperties["Map"]];
        Debug.Log("Map:" + _info.CustomProperties["Map"]);
    }

     
}
