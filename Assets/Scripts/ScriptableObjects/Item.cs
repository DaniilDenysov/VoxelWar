
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon",menuName = "Weapon")]
public class Item : ScriptableObject
{
    [Header("Item stats")]
    public string Name;
    public string MethodName;
    public int ID;
    public Sprite InventorySprite;
    public Sprite KillFedSprite;
    public GameObject UI_prefarb;
    public GameObject Prefarb;
    public bool Destroy;
    public bool isQuestable;
    [Header("Weapon stats")]
    public float Max_Distance;
    public string Animation;
    public int Max_Bullets;
    public int Mag_size;
    public int Burst;
    public float Damage;
    public float FireRate;
    public float RangeHorizontal;
    public float RangeVertical;
    public float TimeToReload;
    public bool isWeapon;
    public bool isMelee;
    public int ScopeSize;
    public int ZoomSize;
    public bool isDMR;
    public bool CountBurst;
    public AudioClip sound;
   // [Header("Item stats")]
    //public bool sdf;

}
