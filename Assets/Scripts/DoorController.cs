using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshObstacle NMO;
    public string Password;
    public PlayerControll Player;
    [SerializeField] private bool main;

    private void Start()
    {
        this.gameObject.name = Password;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && Password == "Unlocked")
        {
            animator.SetBool("isOpen", true);         
            NMO.enabled = false;
        }
        else
        {
            Player = other.gameObject.GetComponent<PlayerControll>();
        }
    }
    private void OnTriggerStay(Collider other)
    {
       
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            animator.SetBool("isOpen", false);                  
            NMO.enabled = true;
        }
    }
    public void UnLock ()
    {
        animator.SetBool("isOpen", true);
        Password = this.gameObject.name = "Unlocked";
      //  Player.Lock.SetActive(false);
      //  Player.HackButton.SetActive(false);
        Player.GetComponent<NavMeshAgent>().isStopped = false;
    }
}
