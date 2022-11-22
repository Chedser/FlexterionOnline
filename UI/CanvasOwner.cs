using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasOwner : MonoBehaviour
{
    public static CanvasOwner instance;

    void Awake()
    {
        if (instance)
        {

            Destroy(gameObject);

        }

        DontDestroyOnLoad(gameObject);

        instance = this;

    }

}
