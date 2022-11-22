using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{

    public bool showCursor;

    private void Start()
    {


        Cursor.lockState = CursorLockMode.Confined;

        if (showCursor)
        {

         
            ShowCursor();

        }
        else {


            HideCursor();

        }

    }

    public static void ShowCursor() {

        Cursor.visible = true;

    }

    public static void HideCursor()
    {
        
        Cursor.visible = false;

    }

}
