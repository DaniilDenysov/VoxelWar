using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class KillFedManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject prefarb;
    [SerializeField] private Transform list;
    [SerializeField] private Sprite[] WeaponType;
    public PhotonView Killer, Killed;
    PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

    public void InstantPref (string Killer, string Killed, int wpn_ID)
    {
       // Debug.Log("WORK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        view.RPC("NewKill",RpcTarget.All,Killer,Killed,wpn_ID);
    }


    [PunRPC]
     public void NewKill (string Killer,string Killed,int wpn_ID)
    {
       /* if (GetComponent<PhotonView>().IsMine == false)
            return;*/
            KillFedItem item = Instantiate(prefarb, list).GetComponent<KillFedItem>();           
            item.transform.SetParent(list.transform);
            item.Killer.text = Killer;
            item.Killed.text = Killed;
            item.weapon.sprite = WeaponType[wpn_ID];
        
    }

}
