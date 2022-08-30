using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemController : MonoBehaviour
{
    public Item Item_data;
    [SerializeField] private float RotationSpeed, DestroyTime;
    public bool stickUpTo;
    float speed = 0.3f;
    [SerializeField] private int[] colidersToIgnore;

    private void Start()
    {

        for (int i = 0; i < colidersToIgnore.Length - 1; i++)
        {
            if (colidersToIgnore[i] != gameObject.layer)
                Physics.IgnoreLayerCollision(gameObject.layer, colidersToIgnore[i], true);
        }
        gameObject.name = Item_data.Name;
        if (DestroyTime < 70) Invoke("_Destroy", DestroyTime);

    }
    void Update()
    {
      //  if (GetComponent<PhotonView>().IsMine) transform.Rotate(0, RotationSpeed, 0);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 14 && collision.gameObject.GetComponent<PlayerControll>())
        {
            PlayerControll player = collision.gameObject.GetComponent<PlayerControll>();
            if (gameObject.layer != 12 && player.weapon.Bullets[player.weapon.Weapon_ID] + player.weapon.Mag[player.weapon.Weapon_ID] < player.weapon.weapon_stats[player.weapon.Weapon_ID].Max_Bullets + player.weapon.weapon_stats[player.weapon.Weapon_ID].Mag_size)
            {
                player.PickUpAmmo(this.gameObject);
                GetComponent<PhotonView>().RPC("DestroyObj",RpcTarget.All);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            if (GetComponent<SphereCollider>()) GetComponent<SphereCollider>().enabled = true;
            else GetComponent<BoxCollider>().isTrigger = false;
        }
    }


    [PunRPC]
    public void DestroyObj ()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerControll>() && gameObject.layer != 12 && other.gameObject.GetComponent<PlayerControll>().weapon.Bullets[other.gameObject.GetComponent<PlayerControll>().weapon.Weapon_ID] + other.gameObject.GetComponent<PlayerControll>().weapon.Mag[other.gameObject.GetComponent<PlayerControll>().weapon.Weapon_ID] < other.gameObject.GetComponent<PlayerControll>().weapon.weapon_stats[other.gameObject.GetComponent<PlayerControll>().weapon.Weapon_ID].Max_Bullets + other.gameObject.GetComponent<PlayerControll>().weapon.weapon_stats[other.gameObject.GetComponent<PlayerControll>().weapon.Weapon_ID].Mag_size)
        {
            GetComponent<BoxCollider>().isTrigger = false;

            transform.position = Vector3.Lerp(transform.position,other.gameObject.transform.position,speed += 0.05f);
          
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerControll>() && gameObject.layer != 12 && other.gameObject.GetComponent<PlayerControll>().weapon.Bullets[other.gameObject.GetComponent<PlayerControll>().weapon.Weapon_ID] + other.gameObject.GetComponent<PlayerControll>().weapon.Mag[other.gameObject.GetComponent<PlayerControll>().weapon.Weapon_ID] < other.gameObject.GetComponent<PlayerControll>().weapon.weapon_stats[other.gameObject.GetComponent<PlayerControll>().weapon.Weapon_ID].Max_Bullets + other.gameObject.GetComponent<PlayerControll>().weapon.weapon_stats[other.gameObject.GetComponent<PlayerControll>().weapon.Weapon_ID].Mag_size)
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }
    }
  
}
