using UnityEngine;

public class CellController : MonoBehaviour
{
    public Inventory_Item II;
    [SerializeField] private KeyCode _button, alter_button;
    [SerializeField] private string string_button;
    public PlayerControll player;
    public RectTransform Selected;
    bool BeingHold;

    private void Start()
    {      
       if (II) II.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        player = FindObjectOfType<PlayerControll>();
    }

    private void FixedUpdate()
    {
        if (II != null)
        {
            if (Input.GetAxis(string_button) > 0f || Input.GetAxis("MouseLeft") > 0.2f && Selected.anchoredPosition == GetComponent<RectTransform>().anchoredPosition)
            {
                Selected.anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                II.Equip(II.Item_data.ID);
                BeingHold = true;
                return;
            }

            if (Input.GetKeyUp(_button) && BeingHold == true || Input.GetAxis("MouseLeft") < 0.2f && Input.GetAxis(string_button) == 0 && BeingHold == true && Selected.anchoredPosition == GetComponent<RectTransform>().anchoredPosition)
            {
                BeingHold = false;
                II.AlterUse();
                if (player.weapon.weapon_stats[II.Item_data.ID].Destroy == true) II.Equip(player.cell[0].II.Item_data.ID);               
            }

        }
    }
}
