using System.Collections;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class DefaultGunMP : GunMP
{
    [SerializeField] Camera cam;

    Animator anim;
    Animator animAim;

    AudioManagerMP instanceAudioManager;
    AudioSource audioShoot;
  
    public GunInfoMP gunInfo;
    PlayerStatsMP playerStats;
    int _bulletsToGo;
    bool _canShoot = true;

    float _maxKillDistance;

    float _shootTime;
    float _damage;
    float _damageHead;

    HealthMP healthMP;

    PhotonView PV;

    public Transform gunFlashPlace;

    RoomManager roomManager;
    
    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        playerStats = GetComponentInParent<PlayerStatsMP>();

        audioShoot = GetComponent<AudioSource>();
       
        healthMP = GetComponentInParent<HealthMP>();
        gunInfo = GetComponent<GunInfoMP>();
        anim = cam.GetComponent<Animator>();
        animAim = GameObject.Find("aim").GetComponent<Animator>();

        _bulletsToGo = gunInfo.bulletsToGo;
        _maxKillDistance = gunInfo.maxKillDistance;
        _shootTime = gunInfo.shootTime;
        _damage = gunInfo.damage;
        _damageHead = gunInfo.damageHead;

        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
      
    }

    public override void Use()
    {

        Shoot();
    }

    private void Update()
    {

        if (roomManager.gameOver) { return; }

        if (_bulletsToGo > gunInfo.bulletsToGo || 
            _maxKillDistance != gunInfo.maxKillDistance || 
            _shootTime != gunInfo.shootTime ||
            _damage != gunInfo.damage ||
            _damageHead != gunInfo.damageHead || 
            healthMP.CurrentHealth <= 0) {
            return;
        }

        // при нажатии на левую кнопку мыши и если нам разрешено стрелять
        if (Input.GetMouseButtonDown(0) & _canShoot && PV.IsMine)
        {
            // выключаем триггер (то есть уже стрелять нельзя)
            _canShoot = false;

            Use();
        }

    }

     void Shoot() {

            anim.SetBool("shootCamera", true);
            animAim.SetBool("isShooting", true);
       
        Vector3 DirectionRay = cam.transform.TransformDirection(Vector3.forward);

        Debug.DrawLine(cam.transform.position, DirectionRay * _maxKillDistance, Color.red);

        RaycastHit hit;
       
            GameObject gunflash =  PhotonNetwork.Instantiate(Path.Combine("Effects", "GunFlash"), gunFlashPlace.position, Quaternion.identity);

        gunflash.transform.SetParent(transform);

      if (Physics.Raycast(cam.transform.position, DirectionRay, out hit, _maxKillDistance)) {

            try {

                if (hit.collider.gameObject.GetComponent<HealthMP>() &&
              hit.collider.gameObject.GetComponent<HealthMP>().CurrentHealth > 0)
                {

                    if (hit.collider.GetComponent<PhotonView>().IsMine) { return; }

                    hit.collider.gameObject.GetComponent<HealthMP>().TakeDamage(_damage, PhotonNetwork.NickName);

                    Debug.Log("Кик " + hit.collider.gameObject.GetComponent<HealthMP>().CurrentHealth);

                    if (hit.collider.gameObject.GetComponent<HealthMP>().CurrentHealth <= 0)
                    {

                        playerStats.SetKills();

                        Debug.Log("Убил");

                    }

                }

                if (hit.collider.gameObject.CompareTag("HeadMP") &&
                    hit.collider.gameObject.GetComponentInParent<HealthMP>().CurrentHealth > 0)
                {

                    if (hit.collider.GetComponent<PhotonView>().IsMine) { return; }

                    hit.collider.gameObject.GetComponentInParent<HealthMP>().TakeDamage(_damageHead, PhotonNetwork.NickName);

                    if (hit.collider.gameObject.GetComponentInParent<HealthMP>().CurrentHealth <= 0)
                    {

                        playerStats.SetKills();

                    }

                }

            } catch { 
            
            }

          

        }

        PV.RPC(nameof(RPC_PlayGunSound), RpcTarget.All, true);

        StartCoroutine(CoroutineShoot());

    }

    public IEnumerator CoroutineShoot()
    {

        // небольшая задержка
        yield return new WaitForSeconds(_shootTime);
        // если еще не все выстрелы произвели...
        if (_bulletsToGo > 0)
        {

       //     WeaponManager.canSwitch = false;

            // то уменьшаем нашу переменную
            _bulletsToGo--;
            // и еще раз стреляем
            Use();

        }
        else
        {// если все выстрелы уже произвели...
         // то небольшая задержка
            yield return new WaitForSeconds(_shootTime);
            // разрешаем стрелять
            _canShoot = true;

            //   WeaponManager.canSwitch = true;

            anim.SetBool("shootCamera", false);
            animAim.SetBool("isShooting", false);

            PV.RPC(nameof(RPC_PlayGunSound), RpcTarget.All, false);

            _bulletsToGo = 4;

            // выходим с сопрограммы
            yield break;

        }

    }

   [PunRPC]
    public void RPC_PlayGunSound(bool flag)
    {

      //  if (!PV.IsMine) { return; }

        if (flag)
        {

                audioShoot.Play();
            
        }
        else {

            audioShoot.Stop();

        }

    }
   

}
