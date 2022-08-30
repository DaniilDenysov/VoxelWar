using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class CameraDetection : MonoBehaviour
{
    [SerializeField] private float movementSpeed, movementTime, rotationAmount, fastSpeed;
    Vector3 newPosition;
    public Camera camera, UICamera;
    [SerializeField] private LayerMask layers;
    public PlayerControll player;


    private void Start()
    {
        camera.orthographicSize = player.weapon.weapon_stats[player.weapon.Weapon_ID].ScopeSize;
        UICamera.GetComponentInChildren<Camera>().orthographicSize = player.weapon.weapon_stats[player.weapon.Weapon_ID].ScopeSize;
    }
    private void LateUpdate()
    {
        if (player != null && player.gameObject.GetComponent<PhotonView>().IsMine == true)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                camera.orthographicSize = player.weapon.weapon_stats[player.weapon.Weapon_ID].ZoomSize;//zoom
                UICamera.GetComponentInChildren<Camera>().orthographicSize = player.weapon.weapon_stats[player.weapon.Weapon_ID].ZoomSize;
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                camera.orthographicSize = player.weapon.weapon_stats[player.weapon.Weapon_ID].ScopeSize;//zoom
                UICamera.GetComponentInChildren<Camera>().orthographicSize = player.weapon.weapon_stats[player.weapon.Weapon_ID].ScopeSize;
            }
            newPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.position = Vector3.Lerp(new Vector3(transform.position.x, transform.position.y, transform.position.z), newPosition, Time.deltaTime * movementTime);

        }
        else
        {
            Destroy(this);
        }
    }
}
