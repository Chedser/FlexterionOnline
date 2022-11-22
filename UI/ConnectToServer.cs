using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {

        PhotonNetwork.AutomaticallySyncScene = true;
        SceneManager.LoadScene("MenuMP");

    }

    public override void OnDisconnected(DisconnectCause cause){

        SceneManager.LoadScene("MainMenu");

    }

}
