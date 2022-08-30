using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;
using Firebase.Auth;
using Firebase;
using Firebase.Database;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;


public class PlayerControll : MonoBehaviourPunCallbacks
{
    public float move_speed, acceleration; //урон оружием,затраты по ОД,размер магазина
    public CellController[] cell;
    public int[] Items;
    [SerializeField] LayerMask layers;
    public GameObject EscScreen, scope, head, obj, DeathScreen; //InventoryTab, Inventory, ScoreboardTab, EscScreen; //ChestWindow, OpenChest;
    [SerializeField] private bool isDied; 
    [SerializeField] private CameraDetection camDetect;
    [SerializeField] private Text advice;
    public Scoreboard score;
    public Camera camera;
    Rigidbody rb;
    public SpawnPlayers spawn;
    [SerializeField] private Transform bullet_spawn;
    public GunController weapon;
    Vector3 _rotation, _scope_rotation;
    public UnityEvent[] Action, AlterAction;
    public Text nick;
    public int Kills, Deaths, Granades;
    [SerializeField] private GameObject[] Prefs; 
    HealthSystem HP;
    PhotonView view;
    public string Message;
    [SerializeField] private Canvas canvas;
    public GameObject FastAccessTab;
    public RoundTimer timer;
    [SerializeField] private ParticleSystem ground;
    public AudioClip PickUp_sound;
    public bool Called = true;
    public GameObject Settings_Screen,Title_Screen;
   

    private void Awake()
    {
        nick.text = GetComponent<PhotonView>().Owner.NickName;
        if (!GetComponent<PhotonView>().IsMine)
        {
            if (canvas != null) Destroy(canvas.gameObject);
            Destroy(GetComponent<PlayerControll>());
        }
    }

    void Start()
    {
        if (GetComponent<PhotonView>().IsMine == true)
        {
            HP = GetComponent<HealthSystem>();
            if (camera == null)
            {
                camera = Instantiate(Prefs[0], transform.position, Quaternion.Euler(0, 45f, 0)).GetComponentInChildren<Camera>();
                camera.transform.parent.GetComponent<CameraDetection>().player = GetComponent<PlayerControll>();
            }
            if (scope == null)
            {
                scope = Instantiate(Prefs[1], transform.position, Quaternion.identity);
                HP.cursor = scope;
            }
            view = GetComponent<PhotonView>();
            spawn = GameObject.FindGameObjectWithTag("Spawner").GetComponent<SpawnPlayers>();
            HP.spawn = spawn;
            weapon.player = this.gameObject.GetComponent<PlayerControll>();           
            rb = GetComponent<Rigidbody>();         
            LoadInventory();
            timer = spawn.timer;
            timer.player = GetComponent<PlayerControll>();
            // timer.score = ScoreboardTab.transform.parent.GetComponent<Scoreboard>();
            score = timer.score;

           
            //StartCoroutine(GetName(FindObjectOfType<DataManager>().auth.CurrentUser.UserId));
        }
        else
        {
            HP.spawn = spawn;
            Destroy(GetComponent<PlayerControll>());
        }
    }

    public void OnlineValueChanged(object sender, ValueChangedEventArgs args)
    {

        //instance != null && 
        if (Called) Called = false;
        else
        {
            Debug.Log("Listener called!");
            if (args.Snapshot.Exists)
            {
                if ((bool)args.Snapshot.Value == true)
                {
                    PhotonNetwork.LeaveRoom();
                    PhotonNetwork.LoadLevel(0);
                    Message = "Other client connected from other device!";
                }
                else Message = "Player data have been changed or deleted!";
            }
        }
        //PhotonNetwork.LoadLevel(0);
    }

    public void LoadInventory()
    {
        for (int i = 0; i < cell.Length; i++)
        {
            if (Items[i] > 0)
            {
                int ID = Items[i];
                Inventory_Item Item = Instantiate(weapon.weapon_stats[ID].UI_prefarb).GetComponent<Inventory_Item>();
                CellController CC = cell[i].GetComponent<CellController>();
                CC.II = Item;
                Item.CC = CC;
                Item.Item_data = weapon.weapon_stats[ID];
                //  Itm.canvas = canvas;
                Item.transform.SetParent(CC.transform.parent);
                Item.GetComponent<RectTransform>().anchoredPosition = cell[i].GetComponent<RectTransform>().anchoredPosition;
                if (Item.Item_data.ID == 11) Granades += 1;
            }

        }
    }

    void Invent (bool active)
    {
        for (int i = 0; i < 5; i++)
        {
            cell[i].GetComponent<Image>().raycastTarget = active;
        }
    }



    void Update()
    {
        if (view.IsMine && !timer.GameStopped)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                score.ScoreboardActive(true);
                weapon.score.UpdateListItems();
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                score.ScoreboardActive(false);
            }
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                onWindow(EscScreen);
                Title_Screen.SetActive(true);
                Settings_Screen.SetActive(false);
            }
            if (!Input.GetKey(KeyCode.LeftShift)) acceleration = 0.5f;
            else acceleration = 0f;
            OnAction();
            if (Input.GetKeyDown("h") && !isDied)
            {
                FindItem(9, "Heal");
            }
        }
    }

    public void FindItem(int ID, string MethodName)
    {
        for (int i = 0; i < cell.Length; i++)
        {
            if (cell[i].GetComponent<CellController>().II && GetComponent<PlayerControll>().cell[i].GetComponent<CellController>().II.Item_data.ID == ID)
            {
                Invoke(MethodName, 0.01f);
                Destroy(GetComponent<PlayerControll>().cell[i].GetComponent<CellController>().II.gameObject);
                break;
            }
        }
    }

    public void OnAction()
    {
        if (view.IsMine)
        {
            if (!EscScreen.active)
            {
                Cursor.visible = false;
                transform.position += transform.forward * move_speed * Time.deltaTime * Input.GetAxis("Vertical");
                transform.position += transform.right * move_speed * Time.deltaTime * Input.GetAxis("Horizontal");
                if (Input.GetAxis("Horizontal") != 0 && !ground.isPlaying|| Input.GetAxis("Vertical") != 0 && !ground.isPlaying) ground.Play();
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layers))
                    {
                        _rotation = new Vector3(hit.point.x, head.transform.position.y, hit.point.z);
                        _scope_rotation = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z);
                        scope.transform.position = _scope_rotation;
                        head.transform.LookAt(_rotation);
                    }
            }
            else
            {
                Cursor.visible = true;
            }
        }
    }
    public void Heal()
    {
        if (!isDied && HP.currHP < HP.maxHP)
        {
            HP.voidHeal();
        }
    }
   

    public void HealArmour()
    {
        if (!isDied && HP.currAR < HP.maxAR)
        {
            HP.currAR = HP.maxAR;
        }
    }
    public void MainMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }



    public void PickUpAmmo (GameObject _collision)
    {
        for (int i = 0;i<cell.Length;i++)
        {
            if (cell[i].II != null && weapon.weapon_stats[cell[i].II.Item_data.ID].isWeapon == true)
            {
                if (weapon.Mag[cell[i].II.Item_data.ID]  < weapon.weapon_stats[cell[i].II.Item_data.ID].Mag_size)
                {
                    weapon.GetComponent<PhotonView>().RPC("AnimationSpeed", RpcTarget.All);
                    weapon.GetComponent<Animator>().SetBool("Reload", true);
                    weapon.StartReload();
                }
                if (weapon.Bullets[cell[i].II.Item_data.ID] + weapon.weapon_stats[cell[i].II.Item_data.ID].Mag_size < weapon.weapon_stats[cell[i].II.Item_data.ID].Max_Bullets)
                {
                  
                    int BulletsNeed = weapon.weapon_stats[cell[i].II.Item_data.ID].Mag_size - weapon.Mag[cell[i].II.Item_data.ID]; //нужно до полного магазина 
                    int BulletsLeft = weapon.weapon_stats[cell[i].II.Item_data.ID].Mag_size - BulletsNeed; //осталось в магазине
                    if (weapon.Mag[cell[i].II.Item_data.ID] < weapon.weapon_stats[cell[i].II.Item_data.ID].Mag_size)
                    {
                        weapon.Bullets[cell[i].II.Item_data.ID] = weapon.Bullets[cell[i].II.Item_data.ID] + BulletsLeft;
                        weapon.Mag[cell[i].II.Item_data.ID] = weapon.weapon_stats[cell[i].II.Item_data.ID].Mag_size;
                    }
                    else
                    {
                        weapon.Bullets[cell[i].II.Item_data.ID] = weapon.Bullets[cell[i].II.Item_data.ID] + weapon.weapon_stats[cell[i].II.Item_data.ID].Mag_size;
                    }
                }
                else
                {
                    weapon.Bullets[cell[i].II.Item_data.ID] = weapon.weapon_stats[cell[i].II.Item_data.ID].Max_Bullets;
                }
            }
        }
        GetComponent<AudioSource>().PlayOneShot(PickUp_sound);
        weapon.nextTimeToFire = 0;    
    }
    public void PickUpItem (GameObject _collision)
    {
        int ID;
        if (obj == _collision)
        {
            advice.text = "Press E to take " + _collision.name;
            ID = _collision.gameObject.GetComponent<ItemController>().Item_data.ID;
           
            for (int i = 0; i < cell.Length; i++)
            {
                if (cell[i].GetComponent<CellController>().II == null)
                {
                    PhotonNetwork.Destroy(_collision.gameObject);
                    if (ID == 9 && HP.currHP < HP.maxHP)
                    {
                        HP.voidHeal();
                        break;
                    }
                    advice.text = "";
                        GameObject Item = Instantiate(weapon.weapon_stats[ID].UI_prefarb);
                        RectTransform rect = Item.GetComponent<RectTransform>();
                        Inventory_Item II = Item.GetComponent<Inventory_Item>();
                        rect.gameObject.transform.SetParent(cell[i].transform.parent);
                        rect.anchoredPosition = cell[i].GetComponent<RectTransform>().anchoredPosition;
                        cell[i].GetComponent<CellController>().II = Item.GetComponent<Inventory_Item>();
                        II.CC = cell[i].GetComponent<CellController>();
                       // II.canvas = canvas;                     
                        II.GetComponent<Image>().sprite = weapon.weapon_stats[ID].InventorySprite;
                        if (II.Item_data.ID == 11) Granades += 1;
                    GetComponent<AudioSource>().PlayOneShot(PickUp_sound);
                    break;
                    
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isDied)
        {
            if (other.gameObject.layer == 17)
            {
                move_speed = move_speed / 3.5f;
                PhotonNetwork.Instantiate(weapon.particles[17].name,transform.position,Quaternion.identity);
            }
            if (!obj && other.gameObject.layer == 12)
            {
                advice.text = "Press E to take " + other.gameObject.name;
                obj = other.gameObject;
            }   
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!isDied)
        {

           if (obj) advice.text = "Press E to take " + obj.name;
            if (obj == other.gameObject && Input.GetKey(KeyCode.E))
            {
                PickUpItem(other.gameObject);
            }
        }     
    }
    private void OnTriggerExit(Collider other)
    {
        if (!isDied)
        {
            advice.text = "";
            if (other.gameObject.layer == 12)
            {
                advice.text = "";
            }
            if (other.gameObject.layer == 17)
            {
                move_speed = move_speed * 3.5f;
                PhotonNetwork.Instantiate(weapon.particles[17].name, transform.position, Quaternion.identity);
            }
            if (obj && other.gameObject.layer == obj.gameObject.layer)
            {
                advice.text = "";
                obj = null;
            }
        }

    }
    public void ThrowGranade ()
    {
        weapon.ThrowGrande();
    }
    public void ShowTrajectoryOfGranade()
    {
        weapon.ShowTrajectoryOfGrande();
    }
    public void ChangeWpn(int ID)
    {
        weapon.GetComponent<PhotonView>().RPC("ChangeWeapon", RpcTarget.All,ID);
        Equip(ID);
    }


    public void Equip (int ID)
    {
        if (!isDied && view.IsMine)
        {
            if (ID != weapon.Weapon_ID)
            {
                weapon.Bullets[weapon.Weapon_ID] = weapon.Bullets[weapon.Weapon_ID] + weapon.Mag[weapon.Weapon_ID];
                weapon.Mag[weapon.Weapon_ID] = 0;
                weapon.Weapon_ID = ID;
                if (weapon.Mag[weapon.Weapon_ID] != weapon.weapon_stats[weapon.Weapon_ID].Mag_size)
                {
                    if (weapon.Bullets[weapon.Weapon_ID] >= weapon.weapon_stats[weapon.Weapon_ID].Mag_size)
                    {
                        weapon.Bullets[weapon.Weapon_ID] -= weapon.weapon_stats[weapon.Weapon_ID].Mag_size;
                        weapon.Mag[weapon.Weapon_ID] = weapon.weapon_stats[weapon.Weapon_ID].Mag_size;
                    }
                    else if (weapon.Bullets[weapon.Weapon_ID] > 0)
                    {
                        weapon.Mag[weapon.Weapon_ID] += weapon.Bullets[weapon.Weapon_ID];
                        weapon.Bullets[weapon.Weapon_ID] = 0;
                    }
                }          
                weapon.ChangeWeapon(ID);
            }
        }
    }



    public void onWindow (GameObject window)
    {
        if (window.active == false) window.SetActive(true); 
        else window.SetActive(false);
    }
}

