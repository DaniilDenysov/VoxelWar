using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterBuilding : MonoBehaviour
{
    [SerializeField] private GameObject roof;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" )
        {
            roof.SetActive(false); 
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            roof.SetActive(false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            roof.SetActive(true);
        }
    }
}
