using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class KillFedItem : MonoBehaviour
{
    public Text Killer,Killed;
    public Image weapon;

    private void Start()
    {
        Invoke("_Destroy",1.5f);
    }
    void _Destroy()
    {
        Destroy(this.gameObject);
    }
}
