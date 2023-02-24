using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class TichuNetworkManager : MonoBehaviourPunCallbacks
{
    private string[] RandomPlayerName = { "���", "����", "����", "��ȭ��", "����", "������", "����", "����", "����", "������", "��纣��", "�ڵ�", "����", "Ű��", "�ٳ���", "���", "���ξ���"};

    List<RoomInfo> MyList = new List<RoomInfo>();
    int currentPage = 1;
    int maxPage, multiple;
    bool Ready = false;
    public int ReadyNum = 3;

    [Header("AllUI")]
    public Text StateText;
    public InputField PlayerNameInput;
    public PhotonView PV;

    [Header("OnlineUI")]
    public GameObject OnlineUI;
    public Text LobbyInfoText;
    public Text PlayerInfoView;
    public Button[] JoinRoomButton;
    public Button PrevButton;
    public Button NextButton;

    [Header("CreateRoomUI")]
    public InputField RoomName;

    [Header("StayRoomUI")]
    public GameObject StayRoomUI;
    public Text InRoomName;
    public InputField WillSendChat;
    public Text[] ChatText;
    public Button[] InRoomPlayerNameBlankButton;
    public Text[] InRoomPlayerNameBlank;
    public Button ReadyORStartButton;
    public Text ReadyORStartText;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Screen.SetResolution(960, 540, false);
        if (PhotonNetwork.LocalPlayer.NickName == "")
            PlayerNameInput.text = RandomPlayerName[Random.Range(0, RandomPlayerName.Length - 1)] + Random.Range(0, 100);
        else
            PlayerNameInput.text = PhotonNetwork.LocalPlayer.NickName;
        PhotonNetwork.LocalPlayer.NickName = PlayerNameInput.text;
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PhotonNetwork.InRoom)
        {
            RoomRenewal();
            OnlineUI.SetActive(true);
            StayRoomUI.SetActive(true);
        }
        //DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        StateText.text = PhotonNetwork.NetworkClientState.ToString();
        LobbyInfoText.text = "�κ�: " + (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "�� / ����: " + PhotonNetwork.CountOfPlayers + "��";

        if (Input.GetKeyDown(KeyCode.Return) && WillSendChat.text != "")
        {
            Send();
        }
    }

    public void SaveSetting()
    {
        if (PlayerNameInput.text != "") PhotonNetwork.LocalPlayer.NickName = PlayerNameInput.text;
        PlayerInfoView.text = PhotonNetwork.LocalPlayer.NickName + "�� ȯ���մϴ�.";
    }

    #region ���� ����
    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        OnlineUI.SetActive(true);
        StayRoomUI.SetActive(false);
        PlayerInfoView.text = PhotonNetwork.LocalPlayer.NickName + "�� ȯ���մϴ�.";
        MyList.Clear();
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        //StayRoomUI.SetActive(false);
    }
    #endregion

    #region �� ����Ʈ ����
    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(MyList[multiple + num].Name);
        MyListRenewal();
    }

    void MyListRenewal()
    {
        maxPage = (MyList.Count % JoinRoomButton.Length == 0) ? MyList.Count / JoinRoomButton.Length : MyList.Count / JoinRoomButton.Length + 1;

        PrevButton.interactable = (currentPage <= 1) ? false : true;
        NextButton.interactable = (currentPage >= maxPage) ? false : true;
        multiple = (currentPage - 1) * JoinRoomButton.Length;
        for(int i = 0; i < JoinRoomButton.Length; i++)
        {
            JoinRoomButton[i].interactable = (multiple + i < MyList.Count) ? true : false;
            JoinRoomButton[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < MyList.Count) ? MyList[multiple + i].PlayerCount + "/" + MyList[multiple + i].MaxPlayers : "";
            JoinRoomButton[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < MyList.Count) ? MyList[multiple + i].Name : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!MyList.Contains(roomList[i]))
                    MyList.Add(roomList[i]);
                else
                    MyList[MyList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (MyList.IndexOf(roomList[i]) != -1)
                MyList.RemoveAt(MyList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion

    #region �� �����, ����, ����
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(RoomName.text == "" ? PhotonNetwork.LocalPlayer.NickName + "���� ��" : RoomName.text, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        RoomName.text = "";
        CreateRoom();
    }

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomName.text = "";
        CreateRoom();
    }

    public override void OnJoinedRoom()
    {
        StayRoomUI.SetActive(true);
        RoomRenewal();
        WillSendChat.text = "";
        for(int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        Chating("<color=yellow>" + newPlayer.NickName + "���� �����ϼ̽��ϴ�.</color>");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        Chating("<color=yellow>" + otherPlayer.NickName + "���� �����ϼ̽��ϴ�.</color>");
    }

    public void LeaveRoom()
    {
        WillSendChat.text = "";
        if(Ready == true)
        {
            ReadyORStartButton.GetComponent<Image>().color = new Color(245, 245, 245, 255);
            PV.RPC("OffReady", RpcTarget.AllBuffered);
            Ready = false;
        }
        PhotonNetwork.LeaveRoom();
    }

    void RoomRenewal()
    {
        InRoomName.text = "�� �̸�: " + PhotonNetwork.CurrentRoom.Name;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            InRoomPlayerNameBlank[i].text = PhotonNetwork.PlayerList[i].NickName;
            InRoomPlayerNameBlankButton[i].interactable = true;
        }
        for(int i = PhotonNetwork.PlayerList.Length; i < 4; i++)
        {
            InRoomPlayerNameBlank[i].text = "";
            InRoomPlayerNameBlankButton[i].interactable = false;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            if (Ready == true)
            {
                ReadyORStartButton.GetComponent<Image>().color = new Color(245, 245, 245, 255);
                PV.RPC("OffReady", RpcTarget.AllBuffered);
                Ready = false;
            }
            ReadyORStartText.text = "��  ��";
        }
        else
            ReadyORStartText.text = "��  ��";
    }

    public void ReadyORStart()
    {
        if (PhotonNetwork.IsMasterClient)// && ReadyNum == 0
        {
            PhotonNetwork.LoadLevel("Game_Play");
        }
        if(!PhotonNetwork.IsMasterClient)
        {
            if (Ready == false)
            {
                ReadyORStartButton.GetComponent<Image>().color = Color.black;
                PV.RPC("OnReady", RpcTarget.AllBuffered);
                Ready = true;
            }
            else
            {
                ReadyORStartButton.GetComponent<Image>().color = new Color(245, 245, 245, 255);
                PV.RPC("OffReady", RpcTarget.AllBuffered);
                Ready = false;
            }
        }
    }
    [PunRPC]
    void OnReady() => ReadyNum--;
    [PunRPC]
    void OffReady() => ReadyNum++;
    #endregion

    #region ä�� ����
    public void Send()
    {
        PV.RPC("Chating", RpcTarget.All, PhotonNetwork.NickName + " : " + WillSendChat.text);
        WillSendChat.text = "";
        WillSendChat.ActivateInputField();
    }

    [PunRPC]
    void Chating(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
        {
            if(ChatText[i].text == "")
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
}
