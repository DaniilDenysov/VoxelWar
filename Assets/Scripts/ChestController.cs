using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;
public class ChestController : MonoBehaviour
{

    public int[] Chest;
    //[SerializeField] DataController data;
    [SerializeField] CellController [] Cell;
    [SerializeField] PlayerControll player;
    [SerializeField] GameObject ChestUI;
    public ChestSaves ChestSave = new ChestSaves();
    public string path;
    public bool isOpened;
    public string SaveName;

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
         path = Path.Combine(Application.persistentDataPath,SaveName);
#else
        path = Path.Combine(Application.dataPath, SaveName);
#endif
        if (File.Exists(path))
        {
            ChestSave = JsonUtility.FromJson<ChestSaves>(File.ReadAllText(path));
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControll>();
            ChestUI = GameObject.Find("Canvas").transform.Find("ChestWindow").gameObject;
          //  data = GameObject.Find("Data").GetComponent<DataController>();
            //LoadChest();
        }
        else
        {
            SaveChest();
        }
    }

    public void SaveChest()
    {
        for (int i = 0; i < Cell.Length; i++)
        {
            if (Cell[i].II == null)
            {
                ChestSave.Cell[i] = 0;
            }
            else
            {
                ChestSave.Cell[i] = Cell[i].II.Item_data.ID;
                Destroy(Cell[i].II.gameObject);
            }
        }
        File.WriteAllText(path, JsonUtility.ToJson(ChestSave));
    }
    public void LoadChest ()
    {
        for (int i = 0; i < Cell.Length; i++)
        {         
            if (ChestSave.Cell[i] != 0)
            {
                if (Cell[i].II == null)
                {
                    Chest[i] = ChestSave.Cell[i];
                    // Debug.Log("Save:" + Chest[i]);
                    GameObject newItem = Instantiate(player.weapon.weapon_stats[Chest[i]].UI_prefarb);
                    Inventory_Item Itm = newItem.GetComponent<Inventory_Item>();
                    CellController CC = Cell[i].GetComponent<CellController>();
                    CC.II = Itm;
                  //  Itm.isQuestable = data.isQuestable[Chest[i]];
                    Itm.Item_data.ID = Chest[i];
                  
                   // Itm.name = data.Item_data[Chest[i]].Name;
                 
                    Itm.CC = CC;
                   // Itm.canvas = player.canvas;
                    Itm.transform.SetParent(CC.transform.parent);
                //    newItem.GetComponent<RectTransform>().anchoredPosition = Cell[i].GetComponent<RectTransform>().anchoredPosition;
                }
                //  Debug.Log("Item");
                //  Cell[i] = data.Chest[i];
            }
            else if (ChestSave.Cell[i] == 0)
            {
                Chest[i] = 0;
            }
        }
    }
    public void OpenChest ()
    {
      //  player.gameObject.GetComponent<PlayerControll>().OpenChest.SetActive(true);
      //  player.chest = this.GetComponent<ChestController>();
        isOpened = true;
    }


    public void CloseChest()
    {
        SaveChest();
    }

    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            OpenChest();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isOpened = false;
            //player.gameObject.GetComponent<PlayerControll>().OpenChest.SetActive(false);
        }
    }
   
#if UNITY_ANDROID && !UNITY_EDITOR
    private void OnApplicationPause(bool pause)
    {
        if (pause) File.WriteAllText(path, JsonUtility.ToJson(sv));
    }
#else
    private void OnApplicationQuit()
    {
        File.WriteAllText(path, JsonUtility.ToJson(ChestSave));
    }
#endif
    [System.Serializable]
    public class ChestSaves
    {
        public int [] Cell = new int[9];
    }
}
