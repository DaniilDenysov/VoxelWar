using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.EventSystems;

public class Inventory_Item : MonoBehaviour
{
    public CellController CC;
    public Item Item_data;
    void Start()
    {
        GetComponent<Image>().sprite = Item_data.InventorySprite;
    }
    public void Equip (int ID)
    {
        CC.player.weapon.ChangeWeapon(ID);
    }
    public void Use ()
    {    
        CC.player.gameObject.GetComponent<PlayerControll>().Action[Item_data.ID].Invoke();
    }
    public void AlterUse()
    {
        if (CC != null && CC.player != null)
        {          
            Debug.Log("Used");
            if (CC.player.weapon.weapon_stats[Item_data.ID].Destroy == true)
            {
                if (Item_data.ID != 9 && Item_data.ID != 10)
                {
                    CC.player.gameObject.GetComponent<PlayerControll>().AlterAction[Item_data.ID].Invoke();
                    CC.Selected.anchoredPosition = CC.player.cell[0].GetComponent<RectTransform>().anchoredPosition;
                    Equip(CC.player.cell[0].II.Item_data.ID);
                    Destroy(this.gameObject);
                }
                else 
                {
                    CC.Selected.anchoredPosition = CC.player.cell[0].GetComponent<RectTransform>().anchoredPosition;
                    Equip(CC.player.cell[0].II.Item_data.ID);
                    if (CC.player.GetComponent<HealthSystem>().currHP < CC.player.GetComponent<HealthSystem>().maxHP && Item_data.ID == 9) Destroy(this.gameObject);
                    if (CC.player.GetComponent<HealthSystem>().currAR < CC.player.GetComponent<HealthSystem>().maxAR && Item_data.ID == 10) Destroy(this.gameObject);
                }

            }
        }
    }
}
