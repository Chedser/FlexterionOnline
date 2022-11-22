using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ExitRoom : MonoBehaviour
{

    [SerializeField] GameObject exitRoomBlock;

    bool _isClicked;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (_isClicked == false)
            {

                exitRoomBlock.SetActive(true);
                _isClicked = true;

            }
            else
            {

                exitRoomBlock.SetActive(false);
                _isClicked = false;

            }

        }
    }
}
