using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemMP : MonoBehaviour
{

    public ItemInfoMP itemInfo;
    public GameObject itemGameObject;

    public abstract void Use();

}
