using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class HealthSystem : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image barFast, barSlow, barFastAR, barSlowAR;
    public float maxHP = 100, currHP, currHPSlow, currAR, currARSlow, maxAR;
    [SerializeField] private float Speed = 0.05f;
    [SerializeField] private UnityEvent onDie;
    public GameObject DeathScreen, Hands,cursor;
    public SpawnPlayers spawn;
    public Canvas canvas;
    [SerializeField] private GameObject[] Items;
    [SerializeField] private GameObject [] LimbsPrefs;
    [SerializeField] private GameObject [] LimbsPositions;
    public GunController weapon;
    public Camera camera;
    public bool isDied;
    bool DeadAlready;
    public int Kills, Deaths;
    public string KilledBy;
    PhotonView photon;
    Hashtable _customProperties = new Hashtable();
    public  Player player;
    DepthOfField depth;
    ChromaticAberration aberration;
    LensDistortion lens;
    public Text RespawnTimer;
    PlayerControll controll;
    PostProcessVolume volume;
    Vignette vingette;
    public bool effect,armour_effect;
    public float effect_index, armour_index;
    public AnimationCurve BlurCurve, ArmourBlur,ItemForce;
    float Force;
    public AudioClip[] Clip;


    private void Awake()
    {
       photon = GetComponent<PhotonView>();
        // player = PhotonNetwork.LocalPlayer;

    }

    // Start is called before the first frame update
    void Start()
    {
       // GetComponent<PlayerControll>().camera.gameObject.transform.parent.GetComponent<PostProcessVolume>().profile.TryGetSettings(out depth);
     //   depth.focalLength.Override(12f);
        photon = GetComponent<PhotonView>();
        player = PhotonNetwork.LocalPlayer;
        if (photon.IsMine)
        {
            controll = GetComponent<PlayerControll>();
            volume = controll.camera.transform.parent.GetComponent<PostProcessVolume>();
            volume.profile.TryGetSettings(out aberration);
            volume.profile.TryGetSettings(out depth);
            //volume.profile.TryGetSettings(out vingette);
            // GetComponent<PlayerControll>().camera.gameObject.transform.parent.GetComponent<PostProcessVolume>().profile.TryGetSettings(out occlusion);
        }
        if (player.CustomProperties["Deaths"] != null) Deaths = (int)player.CustomProperties["Deaths"];
        if (photon.IsMine) spawn.RespawnTimer = RespawnTimer;
       /* if (player.CustomProperties["HP"] != null)
        {
            currHP = (float)GetComponent<PhotonView>().Owner.CustomProperties["HP"];
            currHPSlow = (float)GetComponent<PhotonView>().Owner.CustomProperties["AR"];
        }*/
        //_customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        //Deaths = (int)_customProperties["Deaths"];

        /* currHP = maxHP;
         currHPSlow = maxHP;
         if (maxAR != 0)
         {
             currAR = maxAR;
             currARSlow = maxAR;
         }*/
    }

    public float _time = 0, _ARtime = 0;

    [PunRPC]
    public void PlayerDied ()
    {
        GetComponent<AudioSource>().PlayOneShot(Clip[7]);
    }

    public void OnDeath()
    {
        if (DeathScreen != null && !DeathScreen.active)
        {
            // int D = (int)player.CustomProperties["Deaths"];
            //   int D = (int)player.CustomProperties["Deaths"];
            
            _customProperties.Add("Deaths",Deaths + 1);
            player.SetCustomProperties(_customProperties);
            if (spawn != null) spawn.UI = canvas.gameObject;
            if (spawn != null) spawn._Camera = GetComponent<PlayerControll>().camera.gameObject.transform.parent.gameObject;
            canvas.transform.parent = null;
            DeathScreen.SetActive(true);
            //  DeathScreen.GetComponent<Animator>().Play("DeathText");
            DeathScreen.GetComponent<Animator>().Play("DarkScreen");
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<BoxCollider>());
            GameObject bodyPart = PhotonNetwork.Instantiate(LimbsPrefs[0].name, LimbsPositions[0].transform.position, LimbsPositions[0].transform.rotation);
            spawn.Body[0] = bodyPart;
            /*  for (int i = 0; i < LimbsPrefs.Length-1; i++)
              {
                 GameObject bodyPart = PhotonNetwork.Instantiate(LimbsPrefs[i].name, LimbsPositions[i].transform.position, LimbsPositions[i].transform.rotation);
                 spawn.Body[i] = bodyPart;
                  // PhotonNetwork.Destroy(bodyPart);
              }
              GameObject wpn = PhotonNetwork.Instantiate(LimbsPrefs[4].name, LimbsPositions[4].transform.position, LimbsPositions[4].transform.rotation);
              wpn.GetComponent<MeshFilter>().mesh = weapon.Model[weapon.Weapon_ID];
              wpn.AddComponent<BoxCollider>();
              wpn.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
              spawn.Body[4] = wpn;*/
            Deaths += 1;

            //  FindObjectOfType<Scoreboard>().
            Quaternion firstRotation = Quaternion.Euler(0, Random.Range(-360, 360), 0);
            Vector3 startingVelocity = transform.right * ItemForce.Evaluate(Random.Range(0,10));            
            PhotonNetwork.Instantiate(Items[0].name,transform.position, firstRotation).GetComponent<Rigidbody>().AddForce(startingVelocity, ForceMode.Impulse); ;
            startingVelocity = -transform.right * ItemForce.Evaluate(Random.Range(0, 10));
            PhotonNetwork.Instantiate(Items[Random.Range(1,7)].name, new Vector3(transform.position.x, transform.position.y+2, transform.position.z), firstRotation).GetComponent<Rigidbody>().AddForce(startingVelocity, ForceMode.Impulse); ;
            // PhotonNetwork.Destroy(wpn);
            //  Destroy(GetComponent<BoxCollider>());       
            Hands.SetActive(false);
            if (cursor) Destroy(cursor);
            Cursor.visible = true;
            if (Deaths * (int)PhotonNetwork.CurrentRoom.CustomProperties["respawnTime"] < 60) spawn.StartTime = (int)PhotonNetwork.CurrentRoom.CustomProperties["respawnTime"] * Deaths;
            else spawn.StartTime = 60;
            // if (10 * Deaths < 60) spawn.StartTime = 10 * Deaths;
            //else spawn.StartTime = 60;
            spawn.StartCoroutine(spawn.StartTimer());
            GetComponent<PlayerControll>().FastAccessTab.SetActive(false);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        if (isDied)
        {
            OnDeath();
            //onDie.Invoke();
        }
        if (effect && photon.IsMine)
        {
            aberration.intensity.value = Mathf.Lerp(1, 0, _time);
            depth.focalLength.value = Mathf.Lerp(2, 1, _time);
           // vingette.opacity.value = Mathf.Lerp(1, 0, _time);
            _time += Time.deltaTime / effect_index;
            if (depth.focalLength.value == 0) effect = false;
        }
        if (armour_effect && photon.IsMine)
        {
          //  aberration.intensity.value = Mathf.Lerp(1, 0, _time);
            depth.focalLength.value = Mathf.Lerp(2, 1, _time);
            // vingette.opacity.value = Mathf.Lerp(1, 0, _time);
            _time += Time.deltaTime / armour_index;
            if (depth.focalLength.value == 0) armour_effect = false;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (photon.IsMine) photon.RPC("SendUpdate",RpcTarget.All,currHP,currAR,PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    void SendUpdate (float curHP,float curAR,Player MyOwner)
    {
        currHP = curHP;
        currHPSlow = curHP;
        Health();

        currAR = curAR;
        currARSlow = curAR;
        Armour();
    }

    public void Health ()
    {
        if (currHPSlow != currHP)
        {
            currHPSlow = Mathf.Lerp(currHPSlow, currHP, _time);
            _time += Speed * Time.deltaTime;
           
            /*Hashtable hash = new Hashtable();
            hash.Add("HP", currHP);
            hash.Add("AR", currAR);
            player.SetCustomProperties(hash);*/
        }
        barFast.fillAmount = currHP / maxHP;
        barSlow.fillAmount = currHPSlow / maxHP;
    }
    public void Armour ()
    {
        if (currARSlow != currAR)
        {
            currARSlow = Mathf.Lerp(currARSlow, currAR, _ARtime);
            _ARtime += Speed * Time.deltaTime;
        }
        barFastAR.fillAmount = currAR / maxAR;
        barSlowAR.fillAmount = currARSlow / maxAR;
    }

    public void TakeDamage(float Damage)
    {
        //DoDamage(Damage);
      /*  if (currAR <= 0 && currHP - Damage < maxHP && photon.IsMine)
        {
            GetComponent<PlayerControll>().camera.gameObject.transform.parent.GetComponent<PostProcessVolume>().profile.TryGetSettings(out depth);
            GetComponent<PlayerControll>().camera.gameObject.transform.parent.GetComponent<PostProcessVolume>().profile.TryGetSettings(out occlusion);
        }*/
     //   GetComponent<PlayerControll>().camera.gameObject.transform.parent.GetComponent<PostProcessVolume>().profile.TryGetSettings(out depth);
        Debug.Log("StartedDepth");
        //  DepthHealEffects(1);

        photon.RPC("DoDamage",RpcTarget.All,Damage);
    }

    void PostProccessingEffects ()
    {
        effect = true;
    }

    IEnumerator LensHealEffects(int Index)
    {
        while (depth.focalLength < 10f)
        {
            lens.intensity.Override(lens.intensity - Index * 0.01f);
        }
     if (depth.focalLength == 10f)   yield return null;
    }

    public void voidHeal()
    {
        photon.RPC("HealHP", RpcTarget.All);
        /*   if (!isDied && HP.currHP < HP.maxHP)
           {
               HP.currHP = HP.maxHP;
           }*/
    }
    [PunRPC]
    public void HealHP()
    {
        if (!isDied && currHP < maxHP)
        { 
            currHP = maxHP;
        }
    }

    IEnumerator BarsCheck ()
    {
        while (!isDied)
        {
            Health();
            Armour();
            if (currHP <= 0)
            {
                isDied = true;
            }
            yield return null;
        }
   }
    [PunRPC]
    public void DoDamage (float Damage)
    {
        //if (!photon.IsMine)
          //  return;
        float DamageLeft;
        GetComponent<AudioSource>().PlayOneShot(Clip[Random.Range(0,5)]);
        if (currAR > 0 || currHP > 0)
        {
            currAR -= Damage;
            if (currAR > 0) { _ARtime = 0; }
            else { currHP += currAR; currAR = 0; _time = 0; }
        }
        StartCoroutine(BarsCheck());
        if (currAR < 100 && currAR > 0)
        {
            armour_index = ArmourBlur.Evaluate(currAR);
            armour_effect = true;
        }else if (currHP < 100)
        {
            effect_index = BlurCurve.Evaluate(currHP);
            effect = true;
        }


    }
}
