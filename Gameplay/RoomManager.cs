using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : MonoBehaviourPunCallbacks
{

    public bool gameOver;
    public int killsToWin;
    public string killsToWinH;
    public string winnerName;

    public bool debug;

    Text debugInfo;

    PhotonView PV;

    // Start is called before the first frame update
    void Awake()
    {
    
        killsToWinH = ObmankaMP.GetHashSalted(killsToWin.ToString());

        debugInfo = GameObject.Find("debugInfo").GetComponent<Text>();

        PV = GetComponent<PhotonView>();

    }

    public override void OnEnable() {

        base.OnEnable();

        SceneManager.sceneLoaded += OnSceneLoaded; 

    }

    public override void OnDisable()
    {

        base.OnDisable();

        SceneManager.sceneLoaded -= OnSceneLoaded;

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {

        if (scene.name.Contains("MP")) {

                    PhotonNetwork.Instantiate(Path.Combine("Players/Epifan", "Epifan"), GetRandomSpawnPointPos(), Quaternion.identity);
                    
        }

    }

    Vector3 GetRandomSpawnPointPos() {

        GameObject[] sps = GameObject.FindGameObjectsWithTag("SpawnPoint");

        return sps[Random.Range(0, sps.Length)].transform.position;

    }

     bool NickExists() {

        bool flag = false;

        int namesCount = 0;

        foreach (Player player in PhotonNetwork.PlayerList) {

            if (player.NickName.Equals(PhotonNetwork.NickName)) {
                ++namesCount;
            }

            if (namesCount > 1) {

                flag = true;
                break;

            }

        }

        return flag;

    }

    public void ExitRoom() {

        PV.RPC(nameof(RPC_ExitRoom), RpcTarget.All);

    }

    [PunRPC]
    void RPC_ExitRoom() {

        debugInfo.text = winnerName + " win!";

        Invoke(nameof(ExitRoomDelayed), 3.0f);

    }

    void ExitRoomDelayed() {
          SceneManager.LoadScene("MenuMP");

    }



}
