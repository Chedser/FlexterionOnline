using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EntryRoom : MonoBehaviourPunCallbacks
{

    Text roomTxt;
 
    private void Awake()
    {
        roomTxt = GameObject.Find("roomsTxt").GetComponent<Text>();
    }

    public void OnEntryRoomClick()
    {

        Text roomName = this.gameObject.transform.GetChild(0).GetComponent<Text>();
        try {

            if (PhotonNetwork.JoinRoom(roomName.text)) {

                roomTxt.text = "Connecting to " + roomName.text;

            }

        } catch {

            roomTxt.text = "Connecting error";

        }
        

    }

}
