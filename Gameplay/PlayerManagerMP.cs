using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class PlayerManagerMP:MonoBehaviourPunCallbacks
{

    PhotonView PV;

    public bool showStats;

    public GameObject statsBlock;
    public GameObject entriesBlock;
    public GameObject entryPr;

     Dictionary<string, int> playerDict = new Dictionary<string, int>();
    List<KeyValuePair<string, int>> playerList = new List<KeyValuePair<string, int>>();

    [SerializeField] Text playersCountTxt;

    public bool isUpdated;
     

    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();

    }

    private void Update()
    {

            statsBlock.SetActive(showStats);

        if (isUpdated) {

            PropertiesUpdated();
            isUpdated = true;
        }
           
    }

    void AddKillsToDict()
    {
        playerDict.Clear();
        playerList.Clear();

        foreach (Player player in PhotonNetwork.PlayerList)
        {

            if (!playerDict.ContainsKey(player.NickName)) {
                try {
                    playerDict.Add(player.NickName, (int)player.CustomProperties["Kills"]);
                } catch { continue; }
           
            }
        
        }

        playerList = playerDict.ToList();

        playerList.Sort((x, y) => y.Value.CompareTo(x.Value));

    }

    void ShowStats() {

        if (playerList.Count == 0) { return; }

        if (PhotonNetwork.PlayerList.Count<Player>() == 0) { return; }

        int i = 1;

        foreach (KeyValuePair<string, int> kvp in playerList)
        {
      
                GameObject entry = Instantiate(entryPr, entriesBlock.transform);

                entry.transform.Find("place").GetComponent<Text>().text = i.ToString();
                entry.transform.Find("nick").GetComponent<Text>().text = kvp.Key;
                entry.transform.Find("kills").GetComponent<Text>().text = kvp.Value.ToString();
                            

            ++i;
        }

        playersCountTxt.text = PhotonNetwork.PlayerList.Count<Player>().ToString();

    }

    void ClearEntriesBlock() {

        int entriesCount = entriesBlock.transform.childCount;

        if (entriesCount == 0) { return;}

        for (int i = 0; i < entriesCount; i++) { 
        
            Destroy(entriesBlock.transform.GetChild(i).gameObject);

        }

    }

    public override void OnPlayerPropertiesUpdate(Player player, ExitGames.Client.Photon.Hashtable changedProps) {

        base.OnPlayerPropertiesUpdate(player, changedProps);

        ClearEntriesBlock();
        AddKillsToDict();
        ShowStats();
    
    }


    void PropertiesUpdated() {
        try {

            ClearEntriesBlock();
            AddKillsToDict();
            ShowStats();


        } catch { }
       

    }

}
