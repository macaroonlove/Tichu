using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GamePlaySceneStart : MonoBehaviourPunCallbacks
{
    GameObject CharacterPrefab;

    [Header("CCTV")]
    public GameObject CCTV;
    public Text CCTV_Time;

    [Header("OpenDoor")]
    private Animator DoorAnim;
    private bool IsOpenDoor = false;

    [Header("InPlayer")]
    public Animator CharacterAnim;
    public Transform CharacterTransform;
    private bool WalkInRoom = false;
    private bool WalkInRoomAnimation = false;
    public static string[] PlayerList = new string[4];

    [Header("Noise")]
    public GameObject NoiseImage;
    public GameObject NoiseVideo;
    private bool OffVideo = true;

    public GameObject CardManager;

    void Awake()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            PlayerList[i] = PhotonNetwork.PlayerList[i].NickName;
        int xpo = -65;
        for(int i = 0; i < 4; i++, xpo -= 15)
        {
            if (PlayerList[i] == PhotonNetwork.LocalPlayer.NickName)
            {
                CharacterPrefab = PhotonNetwork.Instantiate("CharacterPrefab" + (i + 1), new Vector3(xpo, 0.5f, 36), Quaternion.Euler(0f, 90f, 0f), 0);
                CharacterPrefab.name = PhotonNetwork.LocalPlayer.NickName;
                CharacterPrefab.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = PlayerList[i];
                CharacterPrefab.layer = 6;
            }
        }
    }

    void Start()
    {
        DoorAnim = GameObject.Find("Door").GetComponent<Animator>();
        CharacterAnim = CharacterPrefab.transform.GetChild(0).gameObject.GetComponent<Animator>();
        CharacterTransform = CharacterPrefab.GetComponent<Transform>();
        Invoke("StartEnd", 1f);
    }

    void StartEnd()
    {
        for (int i = 0; i < 4; i++)
            if (PlayerList[i] != null && PlayerList[i] != PhotonNetwork.LocalPlayer.NickName)
                GameObject.Find("CharacterPrefab" + (i + 1) + "(Clone)").transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = PlayerList[i];
    }

    void Update()
    {
        CCTV_Time.text = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(30f, -45f, 0f), 1f * Time.deltaTime);
        if (transform.rotation == Quaternion.Euler(30f, -45f, 0f) && IsOpenDoor == false)
        {
            DoorOpen();
        }
        if (WalkInRoomAnimation)
        {
            for (int i = 0; i < 4; i++)
            {
                if (PlayerList[i] == PhotonNetwork.LocalPlayer.NickName)
                {
                    CharacterAnim.SetTrigger("Walk");
                }
            }
            WalkInRoomAnimation = false;
        }
        if (WalkInRoom)
        {
            int xpo = 90;
            for (int i = 0; i < 4; i++, xpo -= 30)
            {
                if (PlayerList[i] == PhotonNetwork.LocalPlayer.NickName)
                {
                    CharacterTransform.position = Vector3.Lerp(CharacterTransform.position, new Vector3(xpo, 0.5f, 36f), 0.8f * Time.deltaTime);
                    if (CharacterTransform.position.x > (xpo-8))
                    {
                        CharacterAnim.ResetTrigger("Walk");
                        CharacterAnim.SetTrigger("Turn");
                        CharacterPrefab.transform.rotation = Quaternion.Lerp(CharacterPrefab.transform.rotation, Quaternion.Euler(0, 180f, 0), 1f * Time.deltaTime);
                        DoorAnim.SetTrigger("Close");
                        //CharacterPrefab.transform.GetChild(0).gameObject.GetComponent<PhotonTransformView>().enabled = false;
                        if (OffVideo)
                        {
                            Invoke("NoiseStart", 1f);
                            OffVideo = false;
                        }
                    }
                }
            }
        }
    }

    private void DoorOpen()
    {
        DoorAnim.SetTrigger("Open");
        IsOpenDoor = true;
        Invoke("OnWalkInRoom", 0.6f);
    }

    private void OnWalkInRoom()
    {
        WalkInRoom = true;
        WalkInRoomAnimation = true;
    }

    private void NoiseStart()
    {
        CCTV.SetActive(false);
        NoiseImage.SetActive(true);
        NoiseVideo.SetActive(true);
        Invoke("InfectionSoul", 0.5f);
    }

    private void InfectionSoul()
    {
        NoiseImage.SetActive(false);
        NoiseVideo.SetActive(false);
        CharacterPrefab.transform.GetChild(1).gameObject.GetComponent<Camera>().depth = 2;
        for (int i = 0; i < 4; i++)
        {
            if (PlayerList[i] == PhotonNetwork.LocalPlayer.NickName)
            {
                CharacterPrefab.GetComponent<Character>().enabled = true;
                CharacterPrefab.GetComponent<OtherCharacter>().enabled = false;
            }
        }
        GameManager.PSDStart = true;
        CardManager.GetComponent<CardManager>().enabled = true;
        gameObject.SetActive(false);
    }
}