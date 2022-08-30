using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static GameObject instance;

    public void Awake()
    {
        if (instance == null) { instance = this.gameObject; DontDestroyOnLoad(instance); }
        else Destroy(this.gameObject);


    }

}
