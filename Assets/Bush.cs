using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    public Quaternion oldRotation, newRotation;
    bool PlayerEntered;
    private void Start()
    {
      oldRotation = transform.rotation;
    }

    private void LateUpdate()
    {   
          if (transform.rotation != newRotation)  transform.rotation = Quaternion.Slerp(oldRotation, newRotation, 10);
          else PlayerEntered = false;
        if (!PlayerEntered && transform.rotation != oldRotation) transform.rotation = Quaternion.Slerp(newRotation, oldRotation, 10);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 14)
        {
            PlayerEntered = true;
            newRotation = Quaternion.Euler(Random.Range(-5, 5), 0, Random.Range(-5, 5));
         //   StartCoroutine(BushRotation());
           // newRotation = Quaternion.Euler(Random.Range(-95,95), Random.Range(-95, 95), Random.Range(-95, 95));
          
          //  GetComponent<Animator>().SetBool("PlayerEntered", true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 14)
        {
         if (PlayerEntered && transform.rotation == newRotation)  PlayerEntered = false;
            // newRotation = Quaternion.Euler(Random.Range(-15, 15), Random.Range(-15, 15), Random.Range(-15, 15));

            // transform.rotation = Quaternion.Lerp(oldRotation, newRotation, 15);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 14)
        {
          //  PlayerEntered = true;
            //transform.rotation = Quaternion.Lerp(transform.rotation, oldRotation, 5);
            //GetComponent<Animator>().SetBool("PlayerEntered", false);
        }
    }
    IEnumerator BushRotation ()
    {
        float time = 0;
        while (time < 5)
        {
            time += 0.01f;
            transform.rotation = Quaternion.Slerp(oldRotation, newRotation, 5);
        }
        yield return null;
    }
}
