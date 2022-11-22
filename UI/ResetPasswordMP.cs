using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ResetPasswordMP : MonoBehaviour
{
    [SerializeField] InputField nickInput;
    [SerializeField] InputField loginInput;
    [SerializeField] Button resetPasswordBtn;
    [SerializeField] Text errorsTxt;

    string q_nick;
    string q_login;
    string pass;
    string hash;
    string hashForServer;
    string[] data;

    public void OnResetPasswordClick() {

        data = ObmankaMP.GetPassword();

        pass = data[0];
        hash = data[1];

        nickInput.text = nickInput.text.Trim();
        loginInput.text = loginInput.text.Trim();

        if (string.IsNullOrEmpty(nickInput.text) || string.IsNullOrEmpty(loginInput.text)) {

            return;

        }

        resetPasswordBtn.interactable = false;

        q_nick = ObmankaMP.EncodeForServer(nickInput.text);
        q_login = ObmankaMP.EncodeForServer(loginInput.text);
        hashForServer = ObmankaMP.EncodeForServer(hash);

        StartCoroutine(PostRequest("http://www.chedse6e.bget.ru/flexterion/reset_password.php"));

    }

    IEnumerator PostRequest(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("nick", q_nick.Trim());
        form.AddField("login", q_login.Trim());
        form.AddField("pass", hashForServer);

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);

        uwr.SetRequestHeader("ContentType", "application/x-www-form-urlencoded");
        uwr.SetRequestHeader("Accept", "text/plain");

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            errorsTxt.text = "Connection fucked!";
        }
        else if (uwr.result == UnityWebRequest.Result.Success)
        {

            if (uwr.downloadHandler.text.Equals("0"))
            {
                errorsTxt.text = "Fucked up!";

            }
            else {

                errorsTxt.text = "New password: " + pass;

                PlayerPrefs.SetString("MPL", q_login);
                PlayerPrefs.SetString("MPP", hash);

            }
        
        }
        else
        {

            errorsTxt.text = "Fucked up!";

        }

        resetPasswordBtn.interactable = true;

    } 

}
