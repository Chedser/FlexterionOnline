using Photon.Chat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgChooser : MonoBehaviour
{
    public GameObject[] bgs;

    // Start is called before the first frame update
    void Start()
    {

        bgs[Random.Range(0, bgs.Length)].SetActive(true);
        this.enabled = false;

    }
  
}
