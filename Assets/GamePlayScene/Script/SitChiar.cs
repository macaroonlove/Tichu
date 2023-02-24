using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class SitChiar : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public Collider[] NearPlayers;
    private Vector3 boxSize = new Vector3(20, 30, 20);

    public static bool KeyLock = false;
    public GameObject currentSitPlayer = null;
    private Vector3 CurrentPos;
    public bool currentPlayerCameraPos = false;
    public GameObject Table;
    public Image[] ChangeCardPN;

    private bool EmptySeat = true;

    private bool CloseEye = true;
    public RectTransform upperEye;
    public RectTransform underEye;

    void Start()
    {

    }

    void Update()
    {
        NearPlayers = Physics.OverlapBox(transform.position, boxSize / 2, transform.rotation, 1 << 6);
        
        if (NearPlayers.Length != 0 && EmptySeat && !GameManager.SettingKeyLock)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.F) && KeyLock == false && !currentPlayerCameraPos && NearPlayers.Length != 0 && EmptySeat)
        {
            currentSitPlayer = NearPlayers[0].gameObject;
            currentSitPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
            if (gameObject.name == "Chair1")
            {
                currentSitPlayer.transform.position = new Vector3(gameObject.transform.position.x + 3, -2.2f, gameObject.transform.position.z);
                currentSitPlayer.transform.rotation = Quaternion.Euler(0, 90, 0);
                Table.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                ChangeCardPN[0].color = new Color(0.39f, 0.0f, 0.0f);
                ChangeCardPN[1].color = new Color(0.0f, 0.0f, 0.39f);
                ChangeCardPN[2].color = new Color(0.39f, 0.0f, 0.0f);
                PV.RPC("PlayerOrderSerialize", RpcTarget.AllBuffered, 0, PhotonNetwork.LocalPlayer.NickName);
            }
            else if (gameObject.name == "Chair2")
            {
                currentSitPlayer.transform.position = new Vector3(gameObject.transform.position.x - 3, -2.2f, gameObject.transform.position.z);
                currentSitPlayer.transform.rotation = Quaternion.Euler(0, 270, 0);
                Table.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                ChangeCardPN[0].color = new Color(0.39f, 0.0f, 0.0f);
                ChangeCardPN[1].color = new Color(0.0f, 0.0f, 0.39f);
                ChangeCardPN[2].color = new Color(0.39f, 0.0f, 0.0f);
                PV.RPC("PlayerOrderSerialize", RpcTarget.AllBuffered, 2, PhotonNetwork.LocalPlayer.NickName);
            }                        
            else if (gameObject.name == "Chair3")
            {
                currentSitPlayer.transform.position = new Vector3(gameObject.transform.position.x, -2.2f, gameObject.transform.position.z + 3f);
                currentSitPlayer.transform.rotation = Quaternion.Euler(0, 0, 0);
                Table.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                ChangeCardPN[0].color = new Color(0.0f, 0.0f, 0.39f);
                ChangeCardPN[1].color = new Color(0.39f, 0.0f, 0.0f);
                ChangeCardPN[2].color = new Color(0.0f, 0.0f, 0.39f);
                PV.RPC("PlayerOrderSerialize", RpcTarget.AllBuffered, 1, PhotonNetwork.LocalPlayer.NickName);
            }
            else if(gameObject.name == "Chair4")
            {
                currentSitPlayer.transform.position = new Vector3(gameObject.transform.position.x, -2.2f, gameObject.transform.position.z - 3f);
                currentSitPlayer.transform.rotation = Quaternion.Euler(0, 180, 0);
                Table.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                ChangeCardPN[0].color = new Color(0.0f, 0.0f, 0.39f);
                ChangeCardPN[1].color = new Color(0.39f, 0.0f, 0.0f);
                ChangeCardPN[2].color = new Color(0.0f, 0.0f, 0.39f);
                PV.RPC("PlayerOrderSerialize", RpcTarget.AllBuffered, 3, PhotonNetwork.LocalPlayer.NickName);
            }
            PV.RPC("ColliderOff", RpcTarget.AllBuffered);
            CurrentPos = currentSitPlayer.transform.GetChild(1).position;
            currentSitPlayer.transform.GetChild(1).position = new Vector3(CurrentPos.x, CurrentPos.y - 8f, CurrentPos.z);
            currentPlayerCameraPos = true;
            Character.SitAnimationEnd = true;
            KeyLock = true;
        }
        else if(Input.GetKeyDown(KeyCode.F) && KeyLock == true && currentSitPlayer != null && !currentPlayerCameraPos)
        {
            if (currentSitPlayer.name == PhotonNetwork.LocalPlayer.NickName)
            {
                if (gameObject.name == "Chair1")
                    currentSitPlayer.transform.position = new Vector3(gameObject.transform.position.x - 14, 0.5f, gameObject.transform.position.z);
                else if (gameObject.name == "Chair2")
                    currentSitPlayer.transform.position = new Vector3(gameObject.transform.position.x + 14, 0.5f, gameObject.transform.position.z);
                else if (gameObject.name == "Chair3")
                    currentSitPlayer.transform.position = new Vector3(gameObject.transform.position.x, 0.5f, gameObject.transform.position.z - 14f);
                else if (gameObject.name == "Chair4")
                    currentSitPlayer.transform.position = new Vector3(gameObject.transform.position.x, 0.5f, gameObject.transform.position.z + 14f);
                PV.RPC("ColliderOn", RpcTarget.AllBuffered);
                CurrentPos = currentSitPlayer.transform.GetChild(1).position;
                currentSitPlayer.transform.GetChild(1).position = new Vector3(CurrentPos.x, CurrentPos.y + 8f, CurrentPos.z);
                currentSitPlayer = null;
                KeyLock = false;
            }
        }
        StartCoroutine("EyeBlink");
    }

    IEnumerator EyeBlink()
    {
        if (currentPlayerCameraPos)
        {
            if (CloseEye)
            {
                yield return null;
                upperEye.anchoredPosition = Vector2.Lerp(upperEye.anchoredPosition, new Vector2(0, -10f), 7f * Time.deltaTime);
                underEye.anchoredPosition = Vector2.Lerp(underEye.anchoredPosition, new Vector2(0, 10f), 7f * Time.deltaTime);
                CloseEye = upperEye.anchoredPosition.y <= 0f ? false : true;
            }
            else if (CloseEye == false)
            {
                yield return new WaitForSeconds(0.7f);
                upperEye.anchoredPosition = Vector2.Lerp(upperEye.anchoredPosition, new Vector2(0, 560f), 2f * Time.deltaTime);
                underEye.anchoredPosition = Vector2.Lerp(underEye.anchoredPosition, new Vector2(0, -560f), 2f * Time.deltaTime);
                if (upperEye.anchoredPosition.y >= 550f)
                {
                    CloseEye = true;
                    currentPlayerCameraPos = false;
                }
                else
                {
                    CloseEye = false;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }

    [PunRPC]
    void PlayerOrderSerialize(int i, string PlayerNickName) => CardManager.playerOrder[i] = PlayerNickName;

    [PunRPC]
    void ColliderOff()
    {
        GameObject.Find(PV.name).transform.GetChild(0).GetComponent<BoxCollider>().enabled = false;
        EmptySeat = false;
        CardManager.numberOfPlayersSitting--;
    }

    [PunRPC]
    void ColliderOn()
    {
        GameObject.Find(PV.name).transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;
        EmptySeat = true;
        CardManager.numberOfPlayersSitting++;
    }
}
