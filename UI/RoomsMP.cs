using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

public class RoomsMP : MonoBehaviourPunCallbacks
{
    public GameObject inputsBlock;
    public GameObject roomsBlock;
    public GameObject entryRoomPr;
    public GameObject roomsContainer;
    public GameObject connectBtn;

    public Text createRoomTxt;
    public Text createRoomHintTxt;
    public Text roomTxt;
    public Text nickTxt;

    public Text joinRoomTxt;
    public Button joinButton;
    public Button createRoomButton;
    public Button joinRandomButton;

    public InputField createRoomInput;

    string mapNumber = "1";

    readonly byte  maxPlayers = 10;

    Dictionary<string, int> cachedRoomDict = new Dictionary<string, int>();
    List<KeyValuePair<string, int>> cachedRoomList = new List<KeyValuePair<string, int>>();

    string login;
    string pass;

    int _kills;

    private void Start(){

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
 
    }
 

    public override void OnConnectedToMaster()
    {

        createRoomButton.interactable = true;
        joinButton.interactable = true;
        joinRandomButton.interactable = true;
        connectBtn.GetComponent<Button>().interactable = false;

        connectBtn.SetActive(false);

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.JoinLobby();

    }

    public void OnCreateRoomBtnClick() {
  
        createRoomTxt.text = createRoomTxt.text.Trim();

        if (createRoomTxt.text.ToLower().Contains("admin") || createRoomTxt.text.ToLower().Contains("flexter")) {

                roomTxt.text = "Reserved";
                return;
    
        }

        if (string.IsNullOrEmpty(createRoomTxt.text))
        {
            return;
        }

        CreateRoomPun(createRoomTxt.text.Trim());

        createRoomButton.interactable = false;
        joinButton.interactable = false;
        joinRandomButton.interactable = false;

    }

    void CreateRoomPun(string str) {

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        PhotonNetwork.CreateRoom(str, roomOptions);
        
    }

    public override void OnCreatedRoom()
    {

        roomTxt.text = "Connecting to " + PhotonNetwork.CurrentRoom.Name;

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // log error message and code
        roomTxt.text = "Creating error\nTry to join";

        createRoomButton.interactable = true;
        joinButton.interactable = true;
        joinRandomButton.interactable = true;

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // log error message and code
        roomTxt.text = "Joining error\nTry to create new one";

        createRoomButton.interactable = true;
        joinButton.interactable = true;
        joinRandomButton.interactable = true;

        createRoomInput.Select();

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // log error message and code
        roomTxt.text = "Joining error\nTry to create new one";

        createRoomButton.interactable = true;
        joinButton.interactable = true;
        joinRandomButton.interactable = true;

        createRoomInput.Select();
    }

    public override void OnJoinedRoom(){

        roomTxt.text = "Connecting to " + PhotonNetwork.CurrentRoom.Name;

        PhotonNetwork.LoadLevel("Map"+ mapNumber + "MP");

      }

    public override void OnJoinedLobby()
    {
        roomTxt.text = "Create or Join";
        inputsBlock.SetActive(true);
          

        cachedRoomDict.Clear();
        cachedRoomList.Clear();

    }

    public override void OnLeftLobby()
    {
        cachedRoomDict.Clear();
        cachedRoomList.Clear();
    }

    public void OnJoinRoomBtnClick() {

        joinRoomTxt.text = joinRoomTxt.text.Trim();

        if (joinRoomTxt.text.Length == 0) {

            return;

        }

        PhotonNetwork.JoinRoom(joinRoomTxt.text);

        createRoomButton.interactable = false;
        joinButton.interactable = false;
        joinRandomButton.interactable = false;


    }

    public void OnJoinRandomRoomBtnClick(Button btn)
    {

        createRoomButton.interactable = false;
        joinButton.interactable = false;
        joinRandomButton.interactable = false;

        PhotonNetwork.JoinRandomRoom();
  
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {

        createRoomButton.interactable = true;
        joinButton.interactable = true;
        joinRandomButton.interactable = true;

    }

    public void OnExitMasterClient(GameObject btn) {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();

        cachedRoomDict.Clear();
        cachedRoomList.Clear();

        roomTxt.text = "Disconnect";

        switch (btn.name) {

            case "menuBtn": SceneManager.LoadScene("MainMenu");break;
            case "loginBtn": SceneManager.LoadScene("LoginMP"); break;

        }
            

    }

    public override void OnDisconnected(DisconnectCause cause)
    {

        cachedRoomDict.Clear();
        cachedRoomList.Clear();

        try {

            roomTxt.text = "Disconnect";
            connectBtn.SetActive(true);
            connectBtn.GetComponent<Button>().interactable = true;

        } catch { }
      
    }

    public void OnConnectBtnClick() {
        connectBtn.GetComponent<Button>().interactable = false;
        PhotonNetwork.ConnectUsingSettings();

    }

    IEnumerator PostRequest(string url)
    {
        WWWForm form = new WWWForm();
        

        form.AddField("login", login);
        form.AddField("pass", pass);

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);

        uwr.SetRequestHeader("ContentType", "application/x-www-form-urlencoded");
        uwr.SetRequestHeader("Accept", "text/plain");

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {

            roomTxt.text = "Connection failed";
            inputsBlock.SetActive(false);

        }
        else if (uwr.result == UnityWebRequest.Result.Success)
        {

            if (uwr.downloadHandler.text.Equals("0") || uwr.downloadHandler.text.Equals(""))
            {

                roomTxt.text = "Fucked up! Get out!";
                inputsBlock.SetActive(false);

                SceneManager.LoadScene("RegistrationMP");
            }
            else
            {
                
             

                if (string.IsNullOrEmpty(uwr.downloadHandler.text))
                {
                    roomTxt.text = "Fucked up! Get out!";
                    inputsBlock.SetActive(false);

                    SceneManager.LoadScene("RegistrationMP");

                }
                else
                {

                    try {

                        string[] response = uwr.downloadHandler.text.Split('_');
                        string nick = response[0];
                        string kills = response[1];

                        Debug.Log("Ответ сервака " + uwr.downloadHandler.text);

                        PhotonNetwork.NickName = nick;

                        if (kills.Equals("0"))
                        {
                            nickTxt.text = PhotonNetwork.NickName;

                        }
                        else {

                            nickTxt.text = PhotonNetwork.NickName + " | " + kills;

                        }
       
                        roomTxt.text = "Second...";

                        PhotonNetwork.ConnectUsingSettings();

                    } catch {

                        roomTxt.text = "Fucked up! Get out!";
                        inputsBlock.SetActive(false);

                        SceneManager.LoadScene("RegistrationMP");

                    }
                   

                }

            }


           

        }

      

    }

    /***** ROOMS ****/
    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        if (roomList.Count == 0)
        {
            roomsContainer.SetActive(false);
            return;
        }
        else {

            roomsContainer.SetActive(true);

        }

        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomDict.Remove(info.Name);
            }
            else
            {
                cachedRoomDict[info.Name] = info.PlayerCount;
            }
        }

        cachedRoomList = cachedRoomDict.ToList();

        cachedRoomList.Sort((x, y) => y.Value.CompareTo(x.Value));

    }

    void ShowRooms(List<KeyValuePair<string, int>> roomListCached) {

        if (roomListCached.Count == 0) { return; }
            
        foreach (KeyValuePair<string, int> kvp in roomListCached)
        {

            GameObject entry = Instantiate(entryRoomPr, roomsContainer.transform);
            entry.transform.Find("roomName").GetComponent<Text>().text = kvp.Key;
            entry.transform.Find("playersCountInRoom").GetComponent<Text>().text = kvp.Value.ToString();

        }

    }

    void ClearRoomsContainer(GameObject container) {

        int itemsCount = container.transform.childCount;

        if (itemsCount == 0) { return; }

        for(int i = 0; i < itemsCount; i++) {

            Destroy(container.transform.GetChild(i).gameObject);

        }

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomList.Count == 0)
        {
            roomsBlock.SetActive(false);
        }
        else
        {

            roomsBlock.SetActive(true);

        }

        ClearRoomsContainer(roomsContainer);
        UpdateCachedRoomList(roomList);

        ShowRooms(cachedRoomList);

    }

    public void OnEntryRoomClick(GameObject entry) {

        Text roomName = entry.transform.GetChild(0).gameObject.GetComponent<Text>();

        createRoomButton.interactable = false;
        joinButton.interactable = false;
        joinRandomButton.interactable = false;

        PhotonNetwork.JoinRoom(roomName.text);
    
    }

}
