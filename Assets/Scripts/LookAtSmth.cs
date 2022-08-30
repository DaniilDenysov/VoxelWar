using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LookAtSmth : MonoBehaviour
{
    Camera camera;
    Quaternion originalRotation;

    void Start()
    {

        originalRotation = transform.rotation;
        if (camera == null)
            camera = FindObjectOfType<Camera>();
    }
    void Update()
    {
        if (camera == null)
            camera = FindObjectOfType<Camera>();

        if (camera == null)
            return;

        transform.rotation = camera.transform.rotation * originalRotation;
    }   
}
