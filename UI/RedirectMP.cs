using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RedirectMP : MonoBehaviour
{
    AsyncOperation asyncLoad;

   [SerializeField] GameObject loader;
    [SerializeField] Text connectingTxt;

   

    private void Update()
    {
      

     loader.transform.Rotate(new Vector3(0, 1.0f, 0)); return; 


    }

}
