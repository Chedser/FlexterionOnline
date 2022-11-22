using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class HealthMP : MonoBehaviourPunCallbacks, IDamagable, IPunObservable
{

    const float MAX_HEALTH = 108.0f;
    const float MIN_HEALTH = 103.0f;
 public float CurrentHealth;

    public GameObject[] bloodHoles;

    [SerializeField] GameObject takeDamageSpeachGo;
    [SerializeField] AudioSource respawnAudio;
    AudioSource[] takeDamageSpeach;
  
    string _whoKicked;

    PhotonView PV;

    Collider bodyCollider;

  RawImage healthImage;
  RawImage cherepImage;

    Text whoKilledTxt; 

    Color colorHealth;

    // Start is called before the first frame update
    void Awake()
    {

        CurrentHealth = Random.Range(MIN_HEALTH, MAX_HEALTH);

        PV = GetComponent<PhotonView>();
        bodyCollider = GetComponent<Collider>();

        takeDamageSpeach = takeDamageSpeachGo.GetComponents<AudioSource>();

        healthImage = GameObject.Find("health_image").GetComponent<RawImage>();
        cherepImage = GameObject.Find("cherep_image").GetComponent<RawImage>();
        colorHealth = Color.green;

        healthImage.color = colorHealth;

        whoKilledTxt = GameObject.Find("debugInfo").GetComponent<Text>();
 

    }

   public override void OnEnable() {

        PlayRespawnSound();
      
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      
        if (!PV.IsMine) return;

        if (CurrentHealth > MAX_HEALTH) {

            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.Disconnect();
            PhotonNetwork.LoadLevel("MenuMP");
            return;

        }
    } 


public  void TakeDamage(float demage, string whoKicked) {

        PV.RPC(nameof(RPC_TakeDemage), RpcTarget.All, demage, whoKicked);
  
    }

    [PunRPC]
    void RPC_TakeDemage(float demage, string whoKicked) {

        if (!IsPlayingSpeach(takeDamageSpeach))
        {

            GetRandomSpeach(takeDamageSpeach).Play();
        }

        CurrentHealth -= demage;

        if (CurrentHealth <= 0) {

            Die();

        }

        if (!PV.IsMine) { return; }

        ShowBlood();

        if (CurrentHealth <= 0)
        {

            healthImage.enabled = false;
            cherepImage.enabled = true;

            this._whoKicked = whoKicked;

            whoKilledTxt.text = _whoKicked + " +1 kill";

            if (!IsPlayingSpeach(takeDamageSpeach))
            {

                GetRandomSpeach(takeDamageSpeach).Play();
            }


        }
        else if (CurrentHealth > 0)
        {

            colorHealth = Lerp3(Color.red, Color.blue, Color.green, CurrentHealth / MAX_HEALTH);
            healthImage.color = colorHealth;

        }

    }

    void PlayRespawnSound() {

        if (!PV.IsMine) { return; }

        respawnAudio.Play();

    }
    
    void Die() {

        bodyCollider.enabled = false;
        Invoke(nameof(Respawn), 3.0f);

    }
    
    void ShowBlood() {

        bloodHoles[Random.Range(0, bloodHoles.Length)].SetActive(true);

    }


    void Respawn() {

        GameObject[] sps = GameObject.FindGameObjectsWithTag("SpawnPoint");

        Vector3 pos = sps[Random.Range(0, sps.Length)].transform.position;

        CurrentHealth = Random.Range(MIN_HEALTH, MAX_HEALTH);

        bodyCollider.enabled = true;

        transform.position = pos;

        healthImage.enabled = true;
        healthImage.color = Color.green;

        cherepImage.enabled = false;

        _whoKicked = "";

        whoKilledTxt.text = _whoKicked;

    }

    public bool IsPlayingSpeach(AudioSource[] speach)
    {

        bool flag = false;

        foreach (AudioSource audio in speach)
        {

            if (audio.isPlaying)
            {

                flag = true;
                break;

            }

        }

        return flag;

    }

    public AudioSource GetRandomSpeach(AudioSource[] speach)
    {

        return speach[Random.Range(0, speach.Length)];

    }

    Color Lerp3(Color a, Color b, Color c, float t)
    {
        if (t < 0.5f) // 0.0 to 0.5 goes to a -> b
            return Color.Lerp(a, b, t / 0.5f);
        else // 0.5 to 1.0 goes to b -> c
            return Color.Lerp(b, c, (t - 0.5f) / 0.5f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

            stream.SendNext(CurrentHealth);

        }
        else {

            try {

                CurrentHealth = (float)stream.ReceiveNext();

            } catch { 
            
            }
            
        

        }
    }

}
