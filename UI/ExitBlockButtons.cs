using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class ExitBlockButtons : MonoBehaviourPunCallbacks
{

  public  int _current_button = 0;

    [SerializeField] GameObject returnGameButton;
    [SerializeField] GameObject exitGameButton;
    [SerializeField] GameObject exitCamera;

    public void OnButtonHover(int btnNumber)
    {

        if (btnNumber == 0)
        {

            returnGameButton.transform.localScale = new Vector3(1.3f, 1.3f, 0);
            exitGameButton.transform.localScale = new Vector3(1.0f, 1.0f, 0);

        }
        else
        {

            exitGameButton.transform.localScale = new Vector3(1.3f, 1.3f, 0);
            returnGameButton.transform.localScale = new Vector3(1.0f, 1.0f, 0);

        }

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {

            if (_current_button == 0)
            {

                _current_button = 1;

            }
            else
            {

                _current_button = 0;

            }

        }
        else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {

            if (_current_button == 1)
            {

                _current_button = 0;

            }
            else
            {

                _current_button = 1;

            }


        }

        OnButtonHover(_current_button);

        if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return))
        {

            if (_current_button == 1)
            {

                DestroyPlayer();

               PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
                exitCamera.GetComponent<Camera>().enabled = true;

                SceneManager.LoadScene("MenuMP");

                this.enabled = false;

            }
            else
            {

                _current_button = 0;

                this.gameObject.SetActive(false);

            }

        }

    }

    void DestroyPlayer() {

        GameObject[] players = GameObject.FindGameObjectsWithTag("HeadMP");

        foreach (GameObject player in players) {

            if (player == null) { continue; }

            if (player.GetComponentInParent<HealthMP>().gameObject.GetComponent<PhotonView>().IsMine) {

                PhotonNetwork.Destroy(player);
                break;

            }

        }

    }

    public override void OnDisconnected(DisconnectCause cause) {

        DestroyPlayer();
        SceneManager.LoadScene("RedirectMP");

    }

    public override void OnLeftRoom()
    {

        DestroyPlayer();

    }

}
