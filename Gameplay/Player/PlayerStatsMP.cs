using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerStatsMP:MonoBehaviourPunCallbacks
{
    PhotonView PV;

    Hashtable playerProperties;

    PlayerManagerMP playerManager;

    RoomManager roomManager;

    int clickCount;

    public int Kills { get { return _kills; } set { _kills = value; } }

    int _kills;

    int _isWin;
    
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
            

        if (PV.IsMine) {
            playerProperties = new Hashtable() { { "Kills", 0 } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        }
     
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManagerMP>();

        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
    }
    
    public void SetKills()
    {
        PV.RPC(nameof(RPC_SetKills),RpcTarget.All);

    }

    [PunRPC]
    void RPC_SetKills() {
        if (!PV.IsMine)
        {

            return;
        }
        
        ++_kills;

        Debug.Log("Убито " + _kills);

        playerProperties["Kills"] = _kills;

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
      

        /* if (_kills >= roomManager.killsToWin) {

             _isWin = 1;

             roomManager.gameOver = true;
             roomManager.winnerName = PhotonNetwork.NickName;

             roomManager.ExitRoom();

         } */

    }

    private void Update()
    {

        if (PV.IsMine)
        {

            if (Input.GetKeyDown(KeyCode.Tab))
            {

                if (clickCount == 0)
                {
                    playerManager.showStats = true;
                    clickCount = 1;
                }
                else {

                    playerManager.showStats = false;
                    clickCount = 0;

                }

            }
        
        }
   
    }
      
    public  void OnDestroy()
    {

        playerManager.isUpdated = true;     

    }


    public override void OnPlayerLeftRoom(Player otherPlayer) {

        playerManager.isUpdated = true;

    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

            stream.SendNext(_kills);

        }
        else
        {

            try
            {

                _kills = (int)stream.ReceiveNext();

            }
            catch
            {

            }

        }
    }

}
