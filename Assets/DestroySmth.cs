using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroySmth : MonoBehaviour
{
    public GameObject HaveOwner;
    public bool NoOwner;
    public float TimeForDestroy;
    PhotonView photon;
    void Start()
    {
        photon = GetComponent<PhotonView>();
        if (!HaveOwner && !NoOwner) Invoke("_OnDestroy", 1f);
        else if (NoOwner) Invoke("Photon_OnDestroy", TimeForDestroy);
    }
    private void Update()
    {
      if (!HaveOwner || !photon.IsMine && !NoOwner) Invoke("_OnDestroy", 1f);
    }

    public void _OnDestroy ()
    {
       Destroy(this.gameObject);
    }
    public void Photon_OnDestroy()
    {
        Destroy(this.gameObject);
    }
}
