using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : Destroyable
{
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        Invoke(nameof(Destroy), timeToDestroy);
    }

    public override void Destroy()
    {
        if (!PV.IsMine) { return; }
        PhotonNetwork.Destroy(this.gameObject);
    }

}
