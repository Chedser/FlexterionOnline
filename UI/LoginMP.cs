using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginMP : MonoBehaviour
{

    [SerializeField] InputField loginInput;
    [SerializeField] InputField passInput;
    [SerializeField] Text resultTxt;
    [SerializeField] Button sendLoginBtn;

    string q_login;
    string q_pass;

    public void OnLoginBtnClick() {

        loginInput.text = loginInput.text.Trim();
        passInput.text = passInput.text;

        if (loginInput.text.Length == 0 || passInput.text.Length == 0) {
            return;
        }

        q_login = ObmankaMP.EncodeForServer(loginInput.text);
        q_pass = ObmankaMP.EncodeForServer(ObmankaMP.GetHash(passInput.text));

        sendLoginBtn.interactable = false;

        resultTxt.text = "Waiting...";

        StartCoroutine(PostRequest("http://www.chedse6e.bget.ru/flexterion/login.php"));

    }

    IEnumerator PostRequest(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("login", q_login.Trim());
        form.AddField("pass", q_pass);

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);

        uwr.SetRequestHeader("ContentType", "application/x-www-form-urlencoded");
        uwr.SetRequestHeader("Accept", "text/plain");

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            resultTxt.text = "Connection fucked!";
        }
        else if (uwr.result == UnityWebRequest.Result.Success)
        {

            if (uwr.downloadHandler.text.Equals("1")) //Успех
            {
                resultTxt.text = "Ahuenno!";
                AddInfoInRegistry(q_login, q_pass);
                SceneManager.LoadScene("MenuMP");

            }
            else {

                resultTxt.text = "Fucked up!";

            }
           
        }
        else
        {

            resultTxt.text = "Fucked up!";

        }

        sendLoginBtn.interactable = true;

    }

    void AddInfoInRegistry(string l, string p) {

        PlayerPrefs.SetString("MPL", l);
        PlayerPrefs.SetString("MPP", p);

    }

}
