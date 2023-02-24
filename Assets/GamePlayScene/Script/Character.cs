using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Character : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView PV;
    private Rigidbody rigid;
    private Animator anim;
    private float h = 0.0f;
    private float v = 0.0f;
    public float moveSpeed;
    public float rotateSpeed;
    public float lookSensitivity;
    private float currentPlayerRotationY = 180;
    public static bool SitAnimationEnd = false;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = transform.GetChild(0).gameObject.GetComponent<Animator>();
        moveSpeed = 40.0f;
        rotateSpeed = 80.0f;
        lookSensitivity = -1f;
    }

    void Update()
    {
        if (PV.IsMine)
        {
            PlayerRotation();
            SittingAniamtion();
            //StartCoroutine("SittingAniamtion");
        }
    }

    void FixedUpdate()
    {
        if (PV.IsMine && GameManager.KeyLock == false && SitChiar.KeyLock == false)
        {
            PlayerMove();
        }
    }

    void PlayerRotation()
    {
        if (!GameManager.SettingKeyLock)
        {
            if (SitChiar.KeyLock == false)
            {
                float yRotation = Input.GetAxisRaw("Mouse X");
                float playerRotationY = yRotation * -0.5f;
                currentPlayerRotationY += playerRotationY;

                gameObject.transform.localEulerAngles = new Vector3(0f, -currentPlayerRotationY, 0f);
            }
        }
    }

    void PlayerMove()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        Vector3 moveDir = (transform.forward * v) + (transform.right * h);
        rigid.velocity = moveDir.normalized * moveSpeed;
        if (h != 0.0f || v != 0.0f)
        {
            anim.SetBool("isWalk", true);
        }
        else
        {
            anim.SetBool("isWalk", false);
        }
    }

    void SittingAniamtion()
    {
        if (SitAnimationEnd)
        {
            Invoke("SitAnimationPositionUp", 1.43f);
        }
        if (anim.GetBool("StandToSit") == false && Input.GetKeyDown(KeyCode.F) && SitChiar.KeyLock == true)
        {
            anim.SetBool("SitToStand", false);
            anim.SetBool("StandToSit", true);
        }
        else if (anim.GetBool("StandToSit") == true && Input.GetKeyDown(KeyCode.F) && SitChiar.KeyLock == false)
        {
            anim.SetBool("SitToStand", true);
            anim.SetBool("StandToSit", false);
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }
    }

    void SitAnimationPositionUp()
    {
        transform.position = new Vector3(gameObject.transform.position.x, 4f, gameObject.transform.position.z);
        SitAnimationEnd = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
