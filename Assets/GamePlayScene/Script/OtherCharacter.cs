using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class OtherCharacter : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    private Rigidbody rigid;

    public static bool RigidSeal = false;
    public static bool RigidUnSeal = false;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (RigidSeal)
        {
            rigid.isKinematic = true;
            RigidSeal = false;
        }
        if (RigidUnSeal)
        {
            rigid.isKinematic = false;
            RigidUnSeal = false;
        }
    }
}
