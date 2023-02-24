using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    [Header("ByPlayer")]
    public static string[] PlayerList = new string[4];
    private GameObject MyPlayer;
    public static bool PSDStart = false;

    [Header("Setting")]
    public GameObject InGameSettingUI;
    public static bool SettingKeyLock = false;
    public Text Phone_Time;
    
    [Header("Chating")]
    public GameObject ChatInput;
    public InputField WillSendChat;
    public Text[] ChatText;
    public static bool KeyLock = false;

    [Header("Score")]
    public GameObject ScorePanel;

    void Awake()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            PlayerList[i] = PhotonNetwork.PlayerList[i].NickName;

        WillSendChat.text = "";
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }

    void Start()
    {
        MyPlayer = GameObject.Find(PhotonNetwork.LocalPlayer.NickName);
    }

    void Update()
    {
        //PlayerStandard();
        InGameSetting();
        ReturnChat();
    }

    void PlayerStandard()
    {
        if(PSDStart == true)
        {
            for (int i = 0; i < 4; i++)
                if (PlayerList[i] != null && PlayerList[i] != PhotonNetwork.LocalPlayer.NickName)
                    GameObject.Find("CharacterPrefab" + (i + 1) + "(Clone)").transform.GetChild(2).transform.GetChild(0).gameObject.GetComponent<RectTransform>().transform.LookAt(MyPlayer.transform);
        }
    }

    #region 설정창
    void InGameSetting()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !InGameSettingUI.activeSelf)
        {
            Phone_Time.text = "SKT " + DateTime.Now.ToString("HH:mm");
            SettingKeyLock = true;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            InGameSettingUI.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && InGameSettingUI.activeSelf)
        {
            SettingKeyLock = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            InGameSettingUI.SetActive(false);
        }
    }

    public void Continue()
    {
        SettingKeyLock = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        InGameSettingUI.SetActive(false);
    }

    public void OutRoom()
    {
        PV.RPC("AllPlayerLoadRoom", RpcTarget.AllBuffered);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Intro");
    }

    [PunRPC]
    void AllPlayerLoadRoom()
    {
        Invoke("RemainingPlayers", 0.5f);
    }

    void RemainingPlayers()
    {
        SceneManager.LoadScene("Intro");
    }
    #endregion

    #region 채팅 구현
    void ReturnChat()
    {
        if (Input.GetKeyDown(KeyCode.Return) && ChatInput.activeSelf == false)
        {
            ChatInput.SetActive(true);
            WillSendChat.ActivateInputField();
            KeyLock = true;
        }
        else if (Input.GetKeyDown(KeyCode.Return) && ChatInput.activeSelf)
        {
            if (WillSendChat.text != "")
            {
                Send();
            }
            ChatInput.SetActive(false);
            KeyLock = false;
        }
    }

    public void Send()
    {
        PV.RPC("Chating", RpcTarget.All, "<color=green>" + PhotonNetwork.NickName + " :</color> " + WillSendChat.text);
        WillSendChat.text = "";

    }

    [PunRPC]
    void Chating(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
        {
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        }
        if (!isInput)
        {
            for (int i = 1; i < ChatText.Length; i++)
                ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }
    #endregion


    public void ScoreAnimationOn()
    {
        ScorePanel.transform.GetChild(1).GetComponent<Animator>().SetTrigger("ScorePanelOn");
        ScorePanel.transform.GetChild(1).GetComponent<Animator>().ResetTrigger("ScorePanelOff");
        ScorePanel.transform.GetChild(1).GetComponent<Button>().enabled = false;
        ScorePanel.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void ScoreAnimationOff()
    {
        ScorePanel.transform.GetChild(1).GetComponent<Animator>().SetTrigger("ScorePanelOff");
        ScorePanel.transform.GetChild(1).GetComponent<Animator>().ResetTrigger("ScorePanelOn");
        ScorePanel.transform.GetChild(1).GetComponent<Button>().enabled = true;
        ScorePanel.transform.GetChild(0).gameObject.SetActive(false);
    }
}
