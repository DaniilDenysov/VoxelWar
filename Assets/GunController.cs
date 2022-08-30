using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering.PostProcessing;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GunController : MonoBehaviourPunCallbacks
{
    public Item [] weapon_stats;
    public int Weapon_ID, Kills;
    public Text AmmoCount;
    public float nextTimeToFire;
    public int[] Mag, Bullets;
    public PlayerControll player;
    [SerializeField] private Transform[] bullet_spawn, sleeve_spawn;
    [SerializeField] private GameObject _damageDealt;
    [SerializeField] private ParticleSystem  [] shoot_particles;
    [SerializeField] private ParticleSystem sleeve;
    public GameObject[] particles;
    [SerializeField] private GameObject[] Gun;
    [SerializeField] private TrailRenderer _trail;
    [SerializeField] private LayerMask layer, bushLayer;
    PhotonView photon;
    public Scoreboard score;
    Hashtable _customProperties = new Hashtable();
    [SerializeField] private Transform GranadePoint;
    [SerializeField] private AnimationCurve curve, Trajectory;
    [SerializeField] private LineRenderer lineVisual;
    ChromaticAberration occlusion;
    float ThrowForce = 10f, duration = 0.2f;
    [SerializeField] private GameObject GranadeScope, GranadeScopePref, granade;
    Animator HitScan, animator;
    public AudioClip shooting_sound, knife_sound;

    public void ThrowGrande()
    {       
        animator.SetBool("Throwing", false);
        Rigidbody obj = PhotonNetwork.Instantiate(granade.name, GranadePoint.position, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360))).GetComponent<Rigidbody>();
        ThrowForce = Trajectory.Evaluate(Vector3.Distance(player.scope.transform.position, GranadePoint.transform.position));
        Vector3 startingVelocity = GranadePoint.transform.up * ThrowForce;
        obj.AddForce(startingVelocity, ForceMode.Impulse);
        obj.GetComponent<Granade>().Owner = photon.Owner;
        player.cell[0].Selected.GetComponent<RectTransform>().anchoredPosition = player.cell[0].GetComponent<RectTransform>().anchoredPosition;
        nextTimeToFire = 0;
        GranadeScope.SetActive(false);
        player.scope.SetActive(true);
        lineVisual.enabled = false;
    }
    public void ShowTrajectoryOfGrande()
    {
        Debug.Log("isShowing");
        player.scope.SetActive(false);
        lineVisual.enabled = true;
        GranadeScope.SetActive(true);
        ThrowForce = Trajectory.Evaluate(Vector3.Distance(player.scope.transform.position, GranadePoint.transform.position));
        Vector3 startingVelocity = GranadePoint.transform.up * ThrowForce;
        ShowTrajectory(GranadePoint.transform.position, startingVelocity);
    }

    void Start()
    {
      
        photon = player.GetComponent<PhotonView>();
        if (photon.IsMine)
        {
              Weapon_ID = weapon_stats[player.Items[0]].ID;
            GetComponent<PhotonView>().RPC("ChangeWeapon", RpcTarget.AllBuffered, Weapon_ID);
            AmmoCount = player.GetComponent<PlayerControll>().scope.GetComponent<CursorChanger>().Ammo;
            Gun[weapon_stats[player.Items[0]].ID].SetActive(true);
            animator = GetComponent<Animator>();
            player.camera.gameObject.transform.parent.GetComponent<PostProcessVolume>().profile.TryGetSettings(out occlusion);
            occlusion.intensity.Override(0f);
            GranadeScope = Instantiate(GranadeScopePref, new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0));
            HitScan = player.scope.GetComponentInChildren<Animator>();
            score = player.score;
        }
        if (PhotonNetwork.LocalPlayer.CustomProperties["Kills"] != null) Kills = (int)PhotonNetwork.LocalPlayer.CustomProperties["Kills"];     
    }


    public void WpnActive(GameObject exception)
    {
        for (int i = 0; i < Gun.Length; i++)
        {
            if (Gun[i] != null)
            {
                if (Gun[i] != exception) Gun[i].SetActive(false);
                else Gun[i].SetActive(true);
            }
        }
    }
    [PunRPC]
    public void ChangeWeapon(int ID)
    {
        Weapon_ID = ID;
        if (GetComponent<PhotonView>().IsMine)
        {
            if (Input.GetButton("Fire2"))
            {
                player.camera.orthographicSize = weapon_stats[Weapon_ID].ZoomSize;
                player.camera.transform.parent.GetComponent<CameraDetection>().UICamera.orthographicSize = weapon_stats[Weapon_ID].ZoomSize;
            }
            else
            {
                player.camera.orthographicSize = weapon_stats[Weapon_ID].ScopeSize;
                player.camera.transform.parent.GetComponent<CameraDetection>().UICamera.orthographicSize = weapon_stats[Weapon_ID].ScopeSize;
            }
        }
        WpnActive(Gun[ID]);
    }
    public void KillsCheck(int Kill)
    {
        if (!_customProperties.ContainsKey("Kills"))
            _customProperties.Add("Kills", Kills + 1);
        else
        {
            Kills = (int)PhotonNetwork.LocalPlayer.CustomProperties["Kills"];
            Debug.Log("Kills:" + Kills);
            _customProperties.Remove("Kills");
            _customProperties.Add("Kills", Kills + 1);
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(_customProperties);
        score.UpdateList();
    }

    public void ShowTrajectory(Vector3 origin, Vector3 speed)
    {
        Vector3[] points = new Vector3[50];
        lineVisual.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            float time = i * 0.1f;

            points[i] = origin + speed * time + Physics.gravity * time * time / 2f;

            if (points[i].y < 2.1f)
            {
                lineVisual.positionCount = i + 1;
                break;
            }
        }
  
        lineVisual.SetPositions(points);
        GranadeScope.transform.position = new Vector3(lineVisual.GetPosition(lineVisual.positionCount - 1).x,2.1f, lineVisual.GetPosition(lineVisual.positionCount - 1).z);
    }

    void Update()
    {
        if (photon.IsMine)
        {           
                if (!player.EscScreen.active)
                {
                    AmmoCount.text = Mag[Weapon_ID] + "/" + Bullets[Weapon_ID];
                        if (weapon_stats[Weapon_ID].isWeapon && Input.GetAxis("Fire1") > 0 && Time.time >= nextTimeToFire && animator.GetBool("Reload") == false && !weapon_stats[Weapon_ID].isDMR || Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextTimeToFire && animator.GetBool("Reload") == false && weapon_stats[Weapon_ID].isWeapon && weapon_stats[Weapon_ID].isDMR)
                        {
                            nextTimeToFire = Time.time + 1f / weapon_stats[Weapon_ID].FireRate;
                            Shoot();
                        }
                    
                }
                if (Input.GetKey("r") && GetComponent<PhotonView>())
                {
                    if (Bullets[Weapon_ID] > 0 && Mag[Weapon_ID] < weapon_stats[Weapon_ID].Mag_size && !animator.GetBool("Reload"))
                    {
                        GetComponent<PhotonView>().RPC("AnimationSpeed", RpcTarget.All);
                        animator.SetBool("Reload", true);
                        StartReload();
                    }
                }
            if (!player.EscScreen.active)
            {
                if (Input.GetAxis("Mouse ScrollWheel") < 0 && GetComponent<PhotonView>()) Scroll(1);
                else if (Input.GetAxis("Mouse ScrollWheel") > 0 && GetComponent<PhotonView>()) Scroll(-1);
            }
          
                
                if (Input.GetAxis("G") > 0.5f && !animator.GetBool("Throwing") && player.Granades > 0 && Input.GetAxis("Fire1") == 0)
                {
                    player.scope.SetActive(false);
                    lineVisual.enabled = true;
                    GranadeScope.SetActive(true);
                    ThrowForce = Trajectory.Evaluate(Vector3.Distance(player.scope.transform.position, GranadePoint.transform.position));
                    Vector3 startingVelocity = GranadePoint.transform.up * ThrowForce;
                    ShowTrajectory(GranadePoint.transform.position, startingVelocity);
                    if (Input.GetKeyUp(KeyCode.G) && !animator.GetBool("Throwing") && player.Granades > 0) //&& Input.GetAxis("Fire1") == 0)
                    {
                        GranadeScope.SetActive(false);
                        lineVisual.enabled = false;
                        for (int i = 0; i < player.cell.Length; i++)
                        {
                            if (player.cell[i].II && player.cell[i].II.Item_data.ID == 11)
                            {
                                player.Granades -= 1;
                                Destroy(player.cell[i].II.gameObject);
                                animator.SetBool("Throwing", true);
                                Invoke("ThrowGrande", 0.25f);
                                break;
                            }
                        }

                    }
                }
        }
    }

    public void Scroll (int Dir)
    {

        if (Dir > 0)
        {
            for (int i = 0; i <= 4; i++)
            {

                if (player.cell[i].Selected.GetComponent<RectTransform>().anchoredPosition == player.cell[i].GetComponent<RectTransform>().anchoredPosition)
                {

                    if (i + 1 > 4)
                    {
                        player.cell[0].Selected.GetComponent<RectTransform>().anchoredPosition = player.cell[0].GetComponent<RectTransform>().anchoredPosition;
                        if (player.cell[0].II != null)
                        {
                            player.cell[0].II.Equip(player.cell[0].II.Item_data.ID);
                            break;
                        }
                    }
                    else
                    {

                        player.cell[i + 1].Selected.GetComponent<RectTransform>().anchoredPosition = player.cell[i + 1].GetComponent<RectTransform>().anchoredPosition;
                        if (player.cell[i + 1].II != null)
                        {
                            player.cell[i + 1].II.Equip(player.cell[i + 1].II.Item_data.ID);
                            break;
                        }
                    }
                }
                
            }
        }
        else
        {
            for (int i = 4; i >= 0; i--)
            {
             
                    if (player.cell[i].Selected.GetComponent<RectTransform>().anchoredPosition == player.cell[i].GetComponent<RectTransform>().anchoredPosition)
                    {
                        if (i - 1 < 0)
                        {
                             player.cell[4].Selected.GetComponent<RectTransform>().anchoredPosition = player.cell[4].GetComponent<RectTransform>().anchoredPosition;
                             if (player.cell[4].II != null)
                             {
                              player.cell[4].II.Equip(player.cell[4].II.Item_data.ID);
                            break;
                             }
                        }
                        else
                        {
                            player.cell[i - 1].Selected.GetComponent<RectTransform>().anchoredPosition = player.cell[i - 1].GetComponent<RectTransform>().anchoredPosition;
                            if (player.cell[i - 1].II != null)
                            {
                             player.cell[i - 1].II.Equip(player.cell[i - 1].II.Item_data.ID);
                            break;
                            }
                        }
                    }
                
            }
        }
            
        
    }
    public void Reload()
    {
        animator.SetBool("Reload", false);
        animator.speed = 1;

        if (Mag[Weapon_ID] < weapon_stats[Weapon_ID].Mag_size)
        {
            int NeedBullets = weapon_stats[Weapon_ID].Mag_size - Mag[Weapon_ID];
            if (Bullets[Weapon_ID] > NeedBullets)
            {
                Bullets[Weapon_ID] = Bullets[Weapon_ID] - NeedBullets;
                Mag[Weapon_ID] += NeedBullets;
            }
            else
            {
                Mag[Weapon_ID] += Bullets[Weapon_ID];
                Bullets[Weapon_ID] = 0;
            }
        }

    }
    [PunRPC]
    public void AnimationSpeed()
    {
        animator.speed = weapon_stats[Weapon_ID].TimeToReload / 2;
    }
    public void StartReload()
    {
        Invoke("Reload", weapon_stats[Weapon_ID].TimeToReload);
    }
    [PunRPC]
    private IEnumerator TrailCheck(TrailRenderer trail, RaycastHit hit, float time)
    {
        Vector3 origin = trail.transform.position;
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(origin, hit.point, time);
            time += Time.deltaTime / trail.time;
        }
        trail.transform.position = hit.point;
        Destroy(trail.gameObject);
        yield return null;
    }

    [PunRPC]
    public void PlaySound ()
    {
        GetComponent<AudioSource>().PlayOneShot(weapon_stats[Weapon_ID].sound);
    }

    public void Shoot()
    {
        if (photon.IsMine && !animator.GetBool("Reload") && player.scope.active && Mag[Weapon_ID] > 0 || photon.IsMine && !animator.GetBool("Reload") && player.scope.active && Weapon_ID == 12) //&& player.InventoryTab.active == false)
        {
            //animator.SetBool("isShooting",true);
           // photon.RPC("PlaySound",RpcTarget.All);
            animator.Play("Shoot");
            occlusion.intensity.Override(0.7f);
            StartCoroutine(Shaking(duration));
            GetComponent<PhotonView>().RPC("PlaySound", RpcTarget.All);
            if (!weapon_stats[Weapon_ID].isMelee)
            {
                if (!weapon_stats[Weapon_ID].CountBurst)
                {
                    Mag[Weapon_ID] -= 1;
                }
                else
                {
                    if (Mag[Weapon_ID] - weapon_stats[Weapon_ID].Burst > 0) Mag[Weapon_ID] -= weapon_stats[Weapon_ID].Burst;
                    else Mag[Weapon_ID] = 0;
                }
            }
            else
            {
                    GetComponent<PhotonView>().RPC("Effects", RpcTarget.All);
                    Mag[Weapon_ID] = 0;
            }
            for (int i = weapon_stats[Weapon_ID].Burst; i > 0; i--)
            {
                
                Vector3 direction = SetDirection();
                if (Physics.Raycast(bullet_spawn[Weapon_ID].transform.position, direction, out RaycastHit hit, weapon_stats[Weapon_ID].Max_Distance, layer)) //layers
                {
                    GameObject trail = PhotonNetwork.Instantiate(_trail.name, bullet_spawn[Weapon_ID].transform.position, Quaternion.identity);
                    if (!weapon_stats[Weapon_ID].isMelee) StartCoroutine(SpawnTrail(trail.GetComponent<TrailRenderer>(), hit));
                    if (hit.collider.gameObject.layer == 14)
                    {
                        if (hit.collider.gameObject.GetComponent<HealthSystem>().currHP > 0)
                        {
                            if (hit.collider.gameObject.GetComponent<HealthSystem>().currHP - weapon_stats[Weapon_ID].Damage <= 0)
                            {
                                FindObjectOfType<KillFedManager>().InstantPref(photon.Owner.NickName.ToString(), hit.collider.GetComponent<PhotonView>().Owner.NickName.ToString(), Weapon_ID);
                                KillsCheck(1);
                            }
                            hit.collider.gameObject.GetComponent<HealthSystem>().TakeDamage(weapon_stats[Weapon_ID].Damage);
                        }
                        Instantiate(_damageDealt, hit.point, Quaternion.identity).GetComponent<DamageDisplayController>().DamageDealt = weapon_stats[Weapon_ID].Damage;
                        HitScan.Play("Hitmarker");
                    }
                    SpawnParticles(hit.collider.gameObject.layer, hit);
                }
                if (Physics.Raycast(bullet_spawn[Weapon_ID].transform.position, direction, out RaycastHit hits, weapon_stats[Weapon_ID].Max_Distance, bushLayer)) //layers
                {
                    if (hits.collider.gameObject.layer == 17) SpawnParticles(hits.collider.gameObject.layer, hits);
                }             
                
            }
            animator.SetBool("isShooting", false);
        }
    }

    [PunRPC]
    public void Effects()
    {
       // GetComponent<AudioSource>().PlayOneShot(shooting_sound);
        shoot_particles[Weapon_ID].Play();
        sleeve.Play();
    }

    public IEnumerator Shaking(float Newduration)
    {      
        float elapsedTime = 0f;
        int ID = Weapon_ID;
        while (elapsedTime < Newduration)
        {
            if (ID == Weapon_ID)
            {
                if (!weapon_stats[Weapon_ID].isMelee)
                {
                    elapsedTime += Time.deltaTime;
                    float strength = curve.Evaluate(elapsedTime / duration);
                    player.camera.transform.position = player.camera.transform.position + Random.insideUnitSphere * strength;
                }
                occlusion.intensity.Override(occlusion.intensity - 0.1f);
                yield return null;
            }
            else
            {
                break;
            }
        }
        occlusion.intensity.Override(0);
        player.camera.transform.localPosition = new Vector3(0,-1,4);
    }

    public void UseGrande ()
    {
            player.scope.SetActive(false);
            lineVisual.enabled = true;
            GranadeScope.SetActive(true);
            ThrowForce = Trajectory.Evaluate(Vector3.Distance(player.scope.transform.position, GranadePoint.transform.position));
            Vector3 startingVelocity = GranadePoint.transform.up * ThrowForce;
            ShowTrajectory(GranadePoint.transform.position, startingVelocity);      
    }

    private Vector3 SetDirection ()
    {
        Vector3 direction = player.head.transform.forward;
        direction += new Vector3(Random.Range(-weapon_stats[Weapon_ID].RangeHorizontal, weapon_stats[Weapon_ID].RangeHorizontal), Random.Range(-weapon_stats[Weapon_ID].RangeVertical, weapon_stats[Weapon_ID].RangeVertical),0);
        direction.Normalize();
        return direction;
    }     
   

    private IEnumerator SpawnTrail (TrailRenderer trail,RaycastHit hit)
    {
        float time = 0;
        Vector3 origin = trail.transform.position;
        while (time < 3)
        {
            trail.transform.position = Vector3.Lerp(origin,hit.point,time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
       if (trail) PhotonNetwork.Destroy(trail.gameObject);      
    }
    public void SpawnParticles(int Number,RaycastHit hit)
    {
        GameObject _particles = PhotonNetwork.Instantiate(particles[Number].name,hit.point, Quaternion.LookRotation(player.transform.position));
        _particles.transform.LookAt(transform.parent.transform.position);
        _particles.GetComponent<DestroySmth>().TimeForDestroy = _particles.GetComponent<ParticleSystem>().startLifetime;
    }
}
