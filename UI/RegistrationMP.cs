using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegistrationMP : MonoBehaviour
{

    public Text errorsTxt;

    public InputField nickInput;
    public InputField loginInput;
    public InputField passInput;
    public InputField passRepeatInput;
  
    string patternNick;

    string q_nick;
    string q_login;
    string q_pass;

    public Button sendNickBtn;

    // Start is called before the first frame update
    void Start()
    {
        patternNick = @"^[a-zA-Z][a-zA-Z0-9 ]{1,14}$";

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

    }

  public  int CheckNick() {

        nickInput.text = nickInput.text.Trim();

        if (string.IsNullOrEmpty(nickInput.text)) {

            return 1;
        }

        if (!Regex.IsMatch(nickInput.text, patternNick, RegexOptions.IgnoreCase)) {

            return 2;

        }

        return 0;
  
    }

    public int CheckLogin()
    {

        loginInput.text = loginInput.text.Trim();

        if (string.IsNullOrEmpty(loginInput.text))
        {

            return 1;
        }

        if (!(loginInput.text.Length >= 2 && loginInput.text.Length <= loginInput.characterLimit))
        {

            return 2;

        }
        
        return 0;

    }

    int NickAndLoginEquals() {

        nickInput.text = nickInput.text.Trim();

        if (string.IsNullOrEmpty(nickInput.text) || string.IsNullOrEmpty(loginInput.text)) {
            return 2;
        }

        string nickLower = nickInput.text.ToLower();
        string loginLower = loginInput.text.ToLower();

        if (nickLower.Equals(loginLower)) {
            return 1;
        }

        return 0;

    }

    int CheckPassword() {

        if (string.IsNullOrEmpty(passInput.text) || string.IsNullOrEmpty(passRepeatInput.text)) {

            return 1;

        }

        if (passInput.text.Equals(passRepeatInput.text))
        {

            if (!(passInput.text.Length >= 4 && passInput.text.Length <= passInput.characterLimit) ||
                Regex.IsMatch(passInput.text, @"\s{4,30}", RegexOptions.IgnoreCase))
            {

                return 2;

            }
            else { return 0; }


        }
        else {

            return 3;

        }
 
    }
  

    public void SendQuery() {

        errorsTxt.text = "";
    bool sendQuery = true;

        if (CheckNick() == 1) {

            errorsTxt.text += "Empty nick\n";

            sendQuery = false;

        } else if (CheckNick() == 2) {

            errorsTxt.text += "Nick format\n";

            sendQuery = false;

        }

        if (CheckLogin() == 1)
        {

            errorsTxt.text += "Empty login\n";

            sendQuery = false;

        }
        else if (CheckLogin() == 2)
        {

            errorsTxt.text += "Login length\n";

            sendQuery = false;

        }

        if (NickAndLoginEquals() == 1) {

            errorsTxt.text += "Nick and login are equals\n";

            sendQuery = false;

        } else if (NickAndLoginEquals() == 2) {

            sendQuery = false;

        }

        if (CheckPassword() == 1) {

            errorsTxt.text += "Empty password\n";

            sendQuery = false;

        } else if (CheckPassword() == 2) {

            errorsTxt.text += "Check password length\n";

            sendQuery = false;

        } else if(CheckPassword() == 3){

            errorsTxt.text += "Check password\n";

            sendQuery = false;

        }

        if (!sendQuery) {
            return;
        }

        sendNickBtn.interactable = false;

        errorsTxt.text = "Waiting...";

             q_nick = ObmankaMP.EncodeForServer(nickInput.text);
             q_login = ObmankaMP.EncodeForServer(loginInput.text);
            q_pass = ObmankaMP.EncodeForServer(ObmankaMP.GetHash(passInput.text));

         StartCoroutine(PostRequest("http://www.chedse6e.bget.ru/flexterion/reg.php"));

    }

    IEnumerator PostRequest(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("nick", q_nick.Trim());
        form.AddField("login", q_login.Trim());
        form.AddField("pass", q_pass);

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

            if (uwr.downloadHandler.text.Equals("1")) //Успех
            {
                errorsTxt.text = "Ahuenno!";
                SceneManager.LoadScene("LoginMP");

            }
            else if (uwr.downloadHandler.text.Equals("2"))
            {

                errorsTxt.text = "Nick is busy!";

            }
            else if (uwr.downloadHandler.text.Equals("3"))
            {

                errorsTxt.text = "Fucking smartass!";

            }
            else
            {

                errorsTxt.text = "Fucked up!";

            }

        }
        else {

            errorsTxt.text = "Fucked up!";

        }
      
        sendNickBtn.interactable = true;

    }

  

}
