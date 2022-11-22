using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CheckNickInServer : MonoBehaviourPunCallbacks
{

    public Text roomsTxt;
    public GameObject roomsBlock;
    public Text nickTxt;

    string nick;

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(PostRequest("https://www.frendors.com/flexterion/user_exist.php"));
    }

    IEnumerator PostRequest(string url)
    {
        WWWForm form = new WWWForm();

        string q = EncodeForServer(SystemInfo.deviceUniqueIdentifier.Trim());

        form.AddField("q", q);

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);

        uwr.SetRequestHeader("ContentType", "application/x-www-form-urlencoded");
        uwr.SetRequestHeader("Accept", "text/plain");

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            SceneManager.LoadScene("RegistrationMP");

            roomsTxt.text = "Fucked up! Get out!";
            roomsBlock.SetActive(false);
          
        }
        else if (uwr.result == UnityWebRequest.Result.Success)
        {

            if (uwr.downloadHandler.text.Equals("0"))
            {

                roomsTxt.text = "Fucked up! Get out!";
                roomsBlock.SetActive(false);

                SceneManager.LoadScene("RegistrationMP");
            }
            else
            {
               
                 nick = uwr.downloadHandler.text.Split('`')[1].Trim();

                if (string.IsNullOrEmpty(nick))
                {
                    roomsTxt.text = "Fucked up! Get out!";
                    roomsBlock.SetActive(false);

                    SceneManager.LoadScene("RegistrationMP");

                }
                else {

                    PhotonNetwork.NickName = nick;
                    nickTxt.text = PhotonNetwork.NickName;

                    roomsTxt.text = "Rooms";
                    roomsBlock.SetActive(true);

                }
               
               
            }
          
        }
    }

    string EncodeForServer(string str)
    {

        byte[] bytesToEncode = Encoding.UTF8.GetBytes(str);
        string encodedText = Convert.ToBase64String(bytesToEncode);

        return encodedText;

    }
    
}
