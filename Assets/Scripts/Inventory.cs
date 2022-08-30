using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour {
   // public GameObject [] InventoryCell;
    public GameObject InventoryWindow;
  

    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetKeyDown("tab"))
        {
            if (InventoryWindow.active) InventoryWindow.SetActive(false);
            else InventoryWindow.SetActive(true);
            /*for (int i =5;i < 20;i++)
            {
                if (InventoryCell[i])
                {
                    if (InventoryCell[i].gameObject.active == true)
                    {
                        InventoryCell[i].gameObject.active = false;
                    } else
                    {
                        InventoryCell[i].gameObject.active = true;
                    }
                }
            }
            for (int i = 25; i < InventoryCell.Length; i++)
            {
                if (InventoryCell[i])
                {
                    if (InventoryCell[i].gameObject.active == true)
                    {
                        InventoryCell[i].gameObject.active = false;
                    }
                    else
                    {
                        InventoryCell[i].gameObject.active = true;
                    }
                }
            }*/
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
       /* if (other.gameObject.layer == 11)
        {
            for (int i = 5; i < InventoryCell.Length - 2; i++)
            {
                if (!InventoryCell[i])
                {
                    InventoryCell[i] = other.gameObject;
                }
            }
        }*/
    }


}
