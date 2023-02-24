using CardSort;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region ����
    public PhotonView PV;
    public GameObject myPlayer;

    [Header("���� ����")]
    public GameObject gameStartButton;          // ���� ���� ��ư
    private bool isGameStart = false;           // ������ �����Ͽ��°�?
    public static int numberOfPlayersSitting;   // �ɾ� �ִ� �÷��̾��� �� => �÷��̾��� ���� 0�� �Ǹ� ���� ���� ��ư�� ������

    [Header("Ÿ�̸�")]
    public GameObject currentPlayerTimer;       // Ÿ�̸� ������Ʈ
    public GameObject OutGameManager;           // GameManager ������Ʈ
    private int loundTime = 20;                 // ���� �ð�
    public int divingPlayerCount = 3;           // ����ϴ� �÷��̾��� ī��Ʈ => 0�� �Ǹ� ���ӿ��� ��������. 
    private bool TimerOn = false;               // Ÿ�̸� ������Ʈ�� Ȱ��ȭ �ǰ�, Ÿ�̸��� �ð��� �پ��� ����

    [Header("ī�� ����_���� Ƽ��")]
    public GameObject largeTichuPanel;          // ����Ƽ�� ���� ������ ���ϴ� �г� 
    private bool largeTichuPanelActive;         // ����Ƽ�� �г��� Ȱ��ȭ �Ǿ� �ִ°�?
    private bool isLargeTichu = false;          // ����Ƽ�� ���� �ߴ°�?
    private int numberOfLargeTichuState;        // ����Ƽ�� ���¸� ���� ����� ��

    [Header("ī�� ����")]
    public GameObject animationCard;            // ī�� ���� �ִϸ��̼��� �ִ� ������Ʈ
    public GameObject handCardPanel;            // �� �տ� �ִ� ī�� �г�
    public GameObject tichuBet;                 // ����, �н�, ����Ƽ�� �ִ� UI����
    public Sprite[] cardSprite;                 // ��� ī�� �̹��� ��������Ʈ
    public Sprite cardBlank;                    // �� ī�� �̹��� ��������Ʈ
    public Image[] handCard;                    // �� �տ� �ִ� ī��
    public Material[] cardMaterial;             // ��� ī�� ����
    private List<float> allCard = new List<float>(new float[56]);       // ��ü ī��
    public List<float> myCard = new List<float>(new float[14]);         // �� ���� ī��
    private List<string> choiceCardString = new List<string>();         // ī�� �ؽ�Ʈ ����Ʈ
    private List<float> choiceCardNumber = new List<float>();           // ī�� ���� ����Ʈ
    private List<int> cardNum = new List<int>();
    private float phynixBufferOfNumber = 0;
    //public static bool SuffleAnimationEnd = false;                    // ī�� ���� �ִϸ��̼��� �����°�?

    [Header("ī�� ����_ī�� �й�")]
    public GameObject changeCardPNPanel;        // ī�� �й��ϴ� �г�
    public TextMeshProUGUI[] changeCardPN;      // ī�� �й��ϴ� �г��� �� ��ư�� UI �ؽ�Ʈ
    public List<int> player1ChangeCardNumber = new List<int>();
    public List<int> player2ChangeCardNumber = new List<int>();
    public List<int> player3ChangeCardNumber = new List<int>();
    public List<int> player4ChangeCardNumber = new List<int>();
    public bool changeCardState = false;        // ī�� �й� �ð�
    private bool changeCardLoop = true;         // ī�带 �� �й��ߴ��� Ȯ���ϱ� ���� �ڷ�ƾ �ȿ� ������ ������ ����

    [Header("ī�� ����")]
    public GameObject betCardButton;            // ���� ��ư
    public TextMeshProUGUI betCardText;         // ���� ��ư �ؽ�Ʈ
    public TextMeshProUGUI currentTableCardText;        // ���� ���̺� �ִ� ī��
    public Image currentTableCardTextBackGroundImage;   // ���� ���̺� �ִ� ī�� �̹���
    public MeshRenderer[] tableCard;            // ���̺� ���̴� ī���� ������
    private bool isStraight = true;             // ��Ʈ����Ʈ �ΰ�?
    private int ContinuePairNum = 0;            // ���� �о��� ����
    public static bool isSelectCard = false;    // ī�尡 ���õǸ� true�� �ٲ�� SelectedCardButton() �Լ� ȣ�� �� false�� �ٲ��.
    public static int betCardNum = 0;           // ���õ� ī���� ����

    [Header("ī�� ����_����")]
    public GameObject hopeHyunMooPanel;         // ������ �ҿ� �г�
    public GameObject hopeText;                 // ������ �ҿ��� �����Ͽ��� �� ��ο��� �˷��ִ� �ؽ�Ʈ
    public GameObject hopeError;                // ������ �ҿ��� ���� �ʴ� ī�带 �������� �� ���� �г�
    private bool hopeDelay = false;
    private bool hopeIsOne = false;
    private string hopeString = "";             // ������ �ҿ��ϴ� ī���� ���ڿ�
    private List<string> hopeList = new List<string>(); // ���� ������ ī����� ��� ����Ʈ, ������ �ҿ��� �´� ī�尡 �ִ��� Ȯ���ϱ� ���� ����Ʈ

    [Header("����")]
    private bool myOrder = false;               // �� ���� �ΰ�?
    private bool turnExit = false;              // �� �а� ���ٸ� �� �� ���� �� ������
    private bool doTurn = true;                 // �� �а� ���ٸ� �� �ѱ�� ������ �ѱ��� �ʴ´�.
    private int allPass = 3;                    // �÷��̾ ���� �� ���� �н� -1
    private int allPassNum = 3;                 // �÷��̾��� ���� ���� �� -1, ī�带 ���� 3
    private int submitCard = 14;                // �� ���� ����
    public static string[] playerOrder = new string[4];     // ���� ������� ���� �÷��̾ ����

    [Header("����")]
    public List<string> ranking = new List<string>();       // ����
    private bool rankChange = true;                         // ��ũ�� �߰��� �� �� ���� �����ϵ��� �ϴ� ����

    [Header("���ھ�")]
    public GameObject ScorePanel;               // ������ ������ ��Ÿ���� �г�
    public TextMeshProUGUI[] RedTeamScore;      // ������ ���ھ� �ؽ�Ʈ
    public TextMeshProUGUI[] BlueTeamScore;     // ����� ���ھ� �ؽ�Ʈ
    public TextMeshProUGUI MyScore;             // �� ���� �ؽ�Ʈ(������ ���ɼ� ��)
    public int CRedTeamScore = 0;               // ������ ���ھ�
    public int CBlueTeamScore = 0;              // ����� ���ھ�
    public int Round = 0;                       // ����
    public int TableScore = 0;                  // ���̺� ���� ī���� ������ ��
    public int Score = 0;                       // �� ����
    private bool ScoreExit = false;             // �� ������ ������Ʈ �� �� �� ���� �߰��ǵ��� �ϴ� ����
    private bool LastScoreApply = true;         // 1, 2 ���� ���� ���� ��� ���� ���� ����� ���ϵ��� �����ϴ� ����
    #endregion

    void Start()
    {
        numberOfPlayersSitting = PhotonNetwork.CurrentRoom.PlayerCount; // ��� �÷��̾ �ɾҴ��� Ȯ���ϱ� ���� �÷��̾��� ���ڸ� ����
        numberOfLargeTichuState = PhotonNetwork.PlayerList.Length; // ���� Ƽ�� �ߴ��� Ȯ���ϱ� ���� �÷��̾��� ���ڸ� ����
        myPlayer = GameObject.Find(PhotonNetwork.LocalPlayer.NickName);
    }

    void Update()
    {
        ActiveGameStart(); // ���� ���� ��ư �����ִ� �Լ�

        //OutLTichu(); // ���� Ƽ�� ���¿��� ����� ����Ǵ� �Լ�

        if (isSelectCard && !changeCardState) // ī�尡 ���õǸ�
            SelectedCardButton(); // ī�� ����_�ؽ�Ʈ&��ư Ȱ��ȭ �Լ� ����

        if(TimerOn)
            TimeDown();

        if (allPassNum == 0) // ���� ��
            GameEnd();
    }

    #region Ÿ�̸�
    void TimeDown()
    {
        if (currentPlayerTimer.transform.GetChild(1).GetComponent<Image>().fillAmount > 0)
            currentPlayerTimer.transform.GetChild(1).GetComponent<Image>().fillAmount -= (Time.deltaTime / loundTime);
        if (currentPlayerTimer.transform.GetChild(1).GetComponent<Image>().fillAmount == 0)
        {
            //Debug.Log(divingPlayerCount);
            if (changeCardState)
            {
                divingPlayerCount--;
                if (divingPlayerCount == 2)
                    currentPlayerTimer.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "ī�带 ���� �й��� �ּ���.";
                else if(divingPlayerCount == 1)
                    currentPlayerTimer.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "ī�带 �й����� ���� ��� ������ �����ϴ�.";

                //currentPlayerTimer.transform.GetChild(1).GetComponent<Image>().fillAmount = 1;
            }
            else
                PassBet();

            currentPlayerTimer.transform.GetChild(1).GetComponent<Image>().fillAmount = 1;


            if (divingPlayerCount == 0)
            {
                OutGameManager.GetComponent<GameManager>().OutRoom();
                divingPlayerCount = 3;
            }
        }
    }
    #endregion

    #region ���� �� => ��ó��
    void GameEnd()
    {
        if (LastScoreApply)
        {
            if (!ranking.Contains(PhotonNetwork.LocalPlayer.NickName)) // ������ ���
            {
                if (ranking[0] == playerOrder[1] || ranking[0] == playerOrder[3]) //1���� ����
                    PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, Score, 2);
                else if (ranking[0] == playerOrder[0] || ranking[0] == playerOrder[2]) //1���� ���
                    PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, Score, 3);
            }
            if (PhotonNetwork.LocalPlayer.NickName == ranking[0]) // 1���� ���
            {
                Invoke("PPS", 0.5f);
            }
            LastScoreApply = false;
        }
        StartCoroutine("GameReStart");
    }

    IEnumerator GameReStart()
    {
        for (int l = 0; l < 14; l++)
        {
            tableCard[l].enabled = false;
            handCard[l].gameObject.GetComponent<Image>().enabled = true;
            handCard[l].gameObject.GetComponent<Button>().enabled = true;
        }

        yield return new WaitForSeconds(0.5f);
        allPassNum = 3;
        numberOfLargeTichuState = PhotonNetwork.PlayerList.Length;

        yield return new WaitForSeconds(3f);
        divingPlayerCount = 3;

        //largeTichuPanel.SetActive(true);
        largeTichuPanelActive = false;
        isLargeTichu = false;
        //numberOfLargeTichuState = PhotonNetwork.PlayerList.Length;
        myPlayer.GetComponent<Animator>().SetBool("LT", false);

        changeCardPNPanel.SetActive(false);
        for (int i = 0; i < 3; i++) { changeCardPNPanel.transform.GetChild(i).gameObject.SetActive(true); }

        tichuBet.SetActive(false);
        currentTableCardTextBackGroundImage.color = new Color(1, 1, 1, 0);
        currentTableCardText.text = "";
        handCardPanel.GetComponent<Animator>().SetTrigger("ClosePanel");
        for (int i = 0; i < 14; i++)
        {
            handCard[i].sprite = cardBlank;
            handCard[i].color = Color.black;
            handCard[i].gameObject.SetActive(true);
        }
        phynixBufferOfNumber = 0;

        hopeDelay = false;
        hopeIsOne = false;

        myOrder = false;
        turnExit = false;
        doTurn = true;
        allPass = 3;
        //allPassNum = 3;
        submitCard = 14;

        ranking.Clear();
        rankChange = true;

        RedTeamScore[Round].text = CRedTeamScore.ToString();
        BlueTeamScore[Round].text = CBlueTeamScore.ToString();
        RedTeamScore[10].text = ((RedTeamScore[10].text == "" ? 0 : int.Parse(RedTeamScore[10].text)) + int.Parse(RedTeamScore[Round].text)).ToString();
        BlueTeamScore[10].text = ((BlueTeamScore[10].text == "" ? 0 : int.Parse(BlueTeamScore[10].text)) + int.Parse(BlueTeamScore[Round].text)).ToString();
        CRedTeamScore = 0;
        CBlueTeamScore = 0;
        Round++;
        Score = 0;
        LastScoreApply = true;

        if (PhotonNetwork.IsMasterClient)
            CardShuffle();
        StopCoroutine("GameReStart");
    }
    #endregion

    #region ���� ����
    void ActiveGameStart() //���� ���� ��ư �����ֱ�
    {
        if (numberOfPlayersSitting == 0 && PhotonNetwork.IsMasterClient && !isGameStart) // ��� �÷��̾ �ɰ� ������ �������� �ʾ����� ������ Ŭ���̾�Ʈ����
            gameStartButton.SetActive(true); // ���� ���� ��ư �����ֱ�
    }

    public void OnGameStart() // ���� ���� ��ư�� ������
    {
        CardShuffle(); // ī�� ����_ī�� ���� �Լ� ����

        isGameStart = true; // ������ �����ߴٴ� ���� �˷���
        gameStartButton.SetActive(false); // ���� ���� ��ư �����
    }
    #endregion

    #region ī�� ����
    #region ī�� ����
    void CardShuffle()
    {
        int ran = 0; // ���� �� ���� ����

        for (int i = 0; i < allCard.Count; i++) // �� ī�忡 �� �ֱ� (�������)
            allCard[i] = i;

        for (int i = 0; i < allCard.Count; i++) // �� ī�忡 �־��� ���� ���� �ٲ㼭 �����ϰ� �ٲٱ�
        {
            float temp = allCard[i];
            ran = Random.Range(0, allCard.Count);
            allCard[i] = allCard[ran];
            allCard[ran] = temp;
        }

        PV.RPC("CardAnimation", RpcTarget.AllViaServer); // ī�� ���� �ִϸ��̼��� ��� �� �� �ְ� ����ȭ
        Invoke("CardSynchronization", 1f);  // �ִϸ��̼� ��ٸ��� �ð� 1�� �� ����
    }

    [PunRPC]
    void CardAnimation()
    {
        if (!ScorePanel.activeSelf)
            ScorePanel.SetActive(true);
        animationCard.SetActive(true); //ī�� ���� �ִϸ��̼��� ���� ī�带 Ȱ��ȭ
        handCardPanel.GetComponent<Animator>().SetTrigger("ReCard");
    }

    void CardSynchronization() => PV.RPC("CardSetting", RpcTarget.AllViaServer); //ī�� ���� ����ȭ
    #endregion

    #region ����Ƽ��
    public void LTichu(int i) // ����Ƽ�� ��ư Ŭ���� �̺�Ʈ
    {
        if (i == 0) // ��Ƽ���� ������ ��
            isLargeTichu = false;
        else if (i == 1) // ����Ƽ�� ������ ��
        {
            isLargeTichu = true;
            myPlayer.GetComponent<Animator>().SetBool("LT", true);
        }

        largeTichuPanel.SetActive(false); // ����Ƽ�� ���� ������ ���ϴ� �� ����
        PV.RPC("LargeTichuState", RpcTarget.AllViaServer); // ����Ƽ�� ���� ����ȭ
        PV.RPC("CardSetting", RpcTarget.AllViaServer); // �ٽ� ī�� ����
    }

    [PunRPC]
    void LargeTichuState() => numberOfLargeTichuState--;
    #endregion

    #region ī�� �����ֱ�
    [PunRPC]
    void CardSetting() // ī�� ����
    {
        handCardPanel.SetActive(true); // ī�� �� Ȱ��ȭ
        for (int PlayerNum = 0; PlayerNum < PhotonNetwork.PlayerList.Length; PlayerNum++) // ī�� �й�
            if (PhotonNetwork.LocalPlayer.NickName == PhotonNetwork.PlayerList[PlayerNum].NickName) 
                for (int i = (PlayerNum * 14); i < ((PlayerNum + 1) * 14); i++)
                    myCard[i - (PlayerNum * 14)] = allCard[i];

        StartCoroutine("CardDistribution");
    }

    IEnumerator CardDistribution() // ī�� �й��� �Ϳ� �ؽ��� ������ �Լ�
    {
        if (numberOfLargeTichuState != 0) // ����Ƽ�� ���� ����
        {
            if (!largeTichuPanelActive) // ����Ƽ�� ������Ʈ ����
            {
                largeTichuPanel.SetActive(true);
                largeTichuPanelActive = true;
            }
            for (int i = 0; i < 8; i++) // ī�� 8�� �й�
            {
                CSM.QuickSort(myCard, 0, myCard.Count - 7);
                handCard[i + 3].color = Color.white;
                handCard[i + 3].sprite = cardSprite[(int)myCard[i]];
            }
        }
        else if (numberOfLargeTichuState == 0) //����Ƽ�� ���� �Ŀ�
        {
            animationCard.SetActive(false); // ī�� �ִϸ��̼� ������Ʈ ����
            myCard.Sort();
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < 14; i++) // ī�� 14�� �й�
            {
                handCard[i].color = Color.white;
                handCard[i].sprite = cardSprite[(int)myCard[i]];
                handCard[i].gameObject.GetComponent<Card>().CardNum = (int)myCard[i];
                handCard[i].gameObject.GetComponent<Button>().enabled = true;
            }
            ChangeCard();
        }
        yield return null;
    }
    #endregion

    #region ī�� �ٲٱ�
    void ChangeCard()
    {
        changeCardPNPanel.SetActive(true);
        currentPlayerTimer.SetActive(true);
        TimerOn = true;
        currentPlayerTimer.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "ī�带 ���� �й��ϼ���.";
        currentPlayerTimer.transform.GetChild(1).GetComponent<Image>().fillAmount = 1;
        changeCardState = true;
        for (int i = 0; i < 4; i++)
        {
            if (playerOrder[i] == PhotonNetwork.LocalPlayer.NickName)
            {
                changeCardPN[0].text = playerOrder[i - 1 == -1 ? 3 : i - 1]; // ���� ��
                changeCardPN[1].text = playerOrder[i + 2 >= 4 ? i - 2 : i + 2]; // �ٶ󺸴� �츮�� 0 => 2, 1 => 3, 2 => 0(4), 3 => 1(5)
                changeCardPN[2].text = playerOrder[i + 1 == 4 ? 0 : i + 1]; // ������ ��
            }
        }
        changeCardLoop = true;
        StartCoroutine("ChangeCardEnd");
    }

    IEnumerator ChangeCardEnd()
    {
        while(changeCardLoop)
        {
            yield return null;
            if (player1ChangeCardNumber.Count == 3 && player2ChangeCardNumber.Count == 3 && player3ChangeCardNumber.Count == 3 && player4ChangeCardNumber.Count == 3) //ī�尡 �� ��������
            {
                betCardNum = 0;
                changeCardState = false;
                changeCardLoop = false;
                myCard.RemoveAll(x => x == -10);
                for(int i = 0; i < 4; i++)
                {
                    if (playerOrder[i] == PhotonNetwork.LocalPlayer.NickName)
                    {
                        myCard.Add(i == 0 ? player1ChangeCardNumber[0] : i == 1 ? player2ChangeCardNumber[0] : i == 2 ? player3ChangeCardNumber[0] : player4ChangeCardNumber[0]);
                        myCard.Add(i == 0 ? player1ChangeCardNumber[1] : i == 1 ? player2ChangeCardNumber[1] : i == 2 ? player3ChangeCardNumber[1] : player4ChangeCardNumber[1]);
                        myCard.Add(i == 0 ? player1ChangeCardNumber[2] : i == 1 ? player2ChangeCardNumber[2] : i == 2 ? player3ChangeCardNumber[2] : player4ChangeCardNumber[2]);
                    }
                }
                yield return new WaitForSeconds(1f);
                myCard.Sort();
                yield return new WaitForSeconds(0.1f);
                for (int i = 0; i < 14; i++) // ī�� 14�� �й�
                {
                    handCard[i].color = Color.white;
                    handCard[i].sprite = cardSprite[(int)myCard[i]];
                    handCard[i].gameObject.GetComponent<Card>().CardNum = (int)myCard[i];
                    handCard[i].gameObject.GetComponent<Button>().enabled = true;
                }
                player1ChangeCardNumber.Clear();
                player2ChangeCardNumber.Clear();
                player3ChangeCardNumber.Clear();
                player4ChangeCardNumber.Clear();
                tichuBet.SetActive(true); // Ƽ�� ���� UI Ȱ��ȭ
                divingPlayerCount = 3;
                FirstOrder();
                StopCoroutine("ChangeCardEnd");
            }
        }
    }

    public void ChoiceChangeCard(int c) // �÷��̾��� �̸��� ���� ��ư�� ������ ��
    {
        int scardCount = 0;
        int jTemp = 0;
        for (int j = 0; j < 14; j++)
            if (handCard[j].gameObject.GetComponent<Card>().SCard)
            {
                scardCount++;
                jTemp = j;
            }
                

        if(scardCount == 1)
        {
            changeCardPNPanel.transform.GetChild(c).gameObject.SetActive(false);

            if (changeCardPN[c].text == playerOrder[0])
                PV.RPC("playerChangeCardNum", RpcTarget.All, 1, handCard[jTemp].gameObject.GetComponent<Card>().CardNum);
            else if (changeCardPN[c].text == playerOrder[1])
                PV.RPC("playerChangeCardNum", RpcTarget.All, 2, handCard[jTemp].gameObject.GetComponent<Card>().CardNum);
            else if (changeCardPN[c].text == playerOrder[2])
                PV.RPC("playerChangeCardNum", RpcTarget.All, 3, handCard[jTemp].gameObject.GetComponent<Card>().CardNum);
            else if (changeCardPN[c].text == playerOrder[3])
                PV.RPC("playerChangeCardNum", RpcTarget.All, 4, handCard[jTemp].gameObject.GetComponent<Card>().CardNum);

            myCard[jTemp] = -10;
            handCard[jTemp].color = Color.black;
            handCard[jTemp].gameObject.GetComponent<Card>().SCard = false;
            handCard[jTemp].gameObject.GetComponent<Button>().enabled = false;
        }
    }

    [PunRPC]
    void playerChangeCardNum(int i, int j)
    {
        if (i == 1)
            player1ChangeCardNumber.Add(j);
        else if (i == 2)
            player2ChangeCardNumber.Add(j);
        else if (i == 3)
            player3ChangeCardNumber.Add(j);
        else if (i == 4)
            player4ChangeCardNumber.Add(j);
    }
    #endregion

    #region ���� ���ϱ�
    void FirstOrder()
    {
        for (int i = 0; i < myCard.Count; i++)
            if (handCard[i].sprite.name == "����")
                for (int j = 0; j < playerOrder.Length; j++)
                    if (playerOrder[j] == PhotonNetwork.LocalPlayer.NickName) //������ ������ �ִ� ���
                    {
                        handCardPanel.GetComponent<Image>().color = new Color(0.39f, 0, 0);
                        PV.RPC("WhoTurn", RpcTarget.All, j);
                        myOrder = true;
                    }
    }
    #endregion

    #region ����Ƽ��


    #endregion
    #endregion

    #region ī�� ����_�ؽ�Ʈ&��ư Ȱ��ȭ
    void SelectedCardButton() // ī�尡 ���õǸ� ȣ��
    {
        choiceCardString.Clear(); // ����Ʈ �ʱ�ȭ
        choiceCardNumber.Clear(); // ����Ʈ �ʱ�ȭ
        float TableCardMatchInt = 0; // ���̺��� ī��
        isStraight = true;
        bool Phynix = false;
        bool PhyPosLs = false;
        int ChoiceCardNumberNum = 0;
        //int HSCard = 0;
        betCardText.fontSize = 50; // ���� ��ư ���� ũ�� 50
        NoCombination();

        #region ���� ���̺��� ī�� ���� �о����
        if (currentTableCardText.text == "")
            TableCardMatchInt = 0;
        else if (currentTableCardText.text.Split(' ')[0] == "J")
            TableCardMatchInt = 11;
        else if (currentTableCardText.text.Split(' ')[0] == "Q")
            TableCardMatchInt = 12;
        else if (currentTableCardText.text.Split(' ')[0] == "K")
            TableCardMatchInt = 13;
        else if (currentTableCardText.text.Split(' ')[0] == "A")
            TableCardMatchInt = 14;
        else if (currentTableCardText.text.Split('_')[0] == "����")
            TableCardMatchInt = 1;
        else if (currentTableCardText.text.Split('_')[0] == "��ȣ")
            TableCardMatchInt = -1;
        else if (currentTableCardText.text.Split('_')[0] == "��Ȳ") //�̱��� ���
            TableCardMatchInt = phynixBufferOfNumber;
        else if (currentTableCardText.text.Split('_')[0] == "û��")
            TableCardMatchInt = 100;
        else
            TableCardMatchInt = float.Parse(currentTableCardText.text.Split(' ')[0], CultureInfo.InvariantCulture);
        #endregion

        #region ������ ī�� ���� �о����
        for (int i = 0; i < allCard.Count / 4; i++) // ī�忡 �̸� �� ���� ����
        {
            if (handCard[i].gameObject.GetComponent<Card>().SCard) // ���õ� ī�常
            {
                if (handCard[i].sprite.name == "����" || handCard[i].sprite.name == "��ȣ" || handCard[i].sprite.name == "��Ȳ" || handCard[i].sprite.name == "û��") // Ư��ī����
                    choiceCardString.Add(handCard[i].sprite.name); // Ư��ī�� �̸� �״��
                else // �ƴ϶��
                    choiceCardString.Add(handCard[i].sprite.name.Substring(2)); // ���ڸ�            

                if (handCard[i].sprite.name.Substring(2) == "J") // J = 11
                    choiceCardNumber.Add(11);
                else if (handCard[i].sprite.name.Substring(2) == "Q") // Q = 12
                    choiceCardNumber.Add(12);
                else if (handCard[i].sprite.name.Substring(2) == "K") // K = 13
                    choiceCardNumber.Add(13);
                else if (handCard[i].sprite.name.Substring(2) == "A") // A = 14
                    choiceCardNumber.Add(14);
                else if (handCard[i].sprite.name == "����") // ����(= ����) = 1
                    choiceCardNumber.Add(1);
                else if (handCard[i].sprite.name == "��ȣ") // ��ȣ = -1
                    choiceCardNumber.Add(-1);
                else if (handCard[i].sprite.name == "��Ȳ") // ������ ī���� �̸��� ��Ȳ�� ������
                {
                    Phynix = true;
                }
                else if (handCard[i].sprite.name == "û��") // û�� = 100
                    choiceCardNumber.Add(100);
                else //������ �� ī�� = �� ī���� ����
                    choiceCardNumber.Add(float.Parse(handCard[i].sprite.name.Substring(2), CultureInfo.InvariantCulture));

                //if (handCard[i].sprite.name != "��Ȳ")
                //    HSCard = i;

                ChoiceCardNumberNum++; // ��Ȳ �˰����� ����..
            }
        }
        #endregion

        #region ��Ȳ �˰���
        if (Phynix)
        {
            if (ChoiceCardNumberNum == 1) // �̱��� ���
            {
                PV.RPC("PhynixBuffer", RpcTarget.AllBuffered, TableCardMatchInt);
                choiceCardNumber.Add(TableCardMatchInt + 0.5f);
            }
            else if (ChoiceCardNumberNum == 2 || ChoiceCardNumberNum == 3)
            {
                choiceCardNumber.Add(choiceCardNumber[0]);
            }
            else if (ChoiceCardNumberNum == 4)
            {
                var pairCard = choiceCardNumber.GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                if (pairCard.Count == (ChoiceCardNumberNum / 2 - 1))
                    choiceCardNumber.Add(choiceCardNumber.Except(pairCard).ToList()[0]);
            }
            else if(ChoiceCardNumberNum == 5)
            {
                if ((choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[2] == choiceCardNumber[3]) || (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[1] == choiceCardNumber[2])) 
                {
                    choiceCardNumber.Add(choiceCardNumber[3]);
                    
                }
                else if(choiceCardNumber[1] == choiceCardNumber[2] && choiceCardNumber[2] == choiceCardNumber[3])
                {
                    choiceCardNumber.Add(choiceCardNumber[0]);
                    
                }
                else
                {
                    int isStraight = ChoiceCardNumberNum - 1;
                    int temp = 0;
                    for (int i = 0; i < ChoiceCardNumberNum - 2; i++)
                    {
                        if (choiceCardNumber[i] == choiceCardNumber[i + 1] - 1)
                            isStraight--;
                        else
                            temp = i;
                    }
                    if (isStraight == 2)
                    {
                        choiceCardNumber.Insert(temp + 1, choiceCardNumber[temp] + 1);
                        
                    }
                    else if(isStraight == 1)
                    {
                        choiceCardNumber.Add(choiceCardNumber[temp] + ChoiceCardNumberNum - 1);
                        
                        PhyPosLs = true;
                    }
                    else
                    {
                        choiceCardNumber.Add(50);
                    }
                }
            }
            else if (ChoiceCardNumberNum > 5)
            {
                int isStraight = ChoiceCardNumberNum - 1;
                int temp = 0;
                for (int i = 0; i < ChoiceCardNumberNum - 2; i++)
                {
                    if (choiceCardNumber[i] == choiceCardNumber[i + 1] - 1)
                        isStraight--;
                    else
                        temp = i;
                }
                if (isStraight == 2)
                {
                    choiceCardNumber.Insert(temp + 1, choiceCardNumber[temp] + 1);
                    
                }
                else if (isStraight == 1)
                {
                    choiceCardNumber.Add(choiceCardNumber[temp] + ChoiceCardNumberNum - 1);
                    
                    PhyPosLs = true;
                }
                else
                {
                    if (ChoiceCardNumberNum % 2 == 0)
                    {
                        var pairCard = choiceCardNumber.GroupBy(i => i).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                        if(pairCard.Count == (ChoiceCardNumberNum/2 - 1))
                        {
                            choiceCardNumber.Add(choiceCardNumber.Except(pairCard).ToList()[0]);
                            
                        }
                    }
                    else
                    {
                        choiceCardNumber.Add(50);
                    }
                }
            }
            else
            {
                choiceCardNumber.Add(50);
            }
            Phynix = false;
        }
        #endregion
        
        choiceCardNumber.Sort((x, y) => x.CompareTo(y)); // ī�� ����

        #region ī�� Ȱ��ȭ ����
        if (betCardNum == 0) // ���õ� ī�尡 0�� �̶��
            NoCombination();
        else if (betCardNum == 1)
        {
            if (choiceCardString[0] == "����" || choiceCardString[0] == "��ȣ" || choiceCardString[0] == "��Ȳ" || choiceCardString[0] == "û��") // Ư�� ī�� ���
                betCardText.text = choiceCardString[0]; // Ư�� ī�� �̸� �״��
            else
                betCardText.text = choiceCardString[0] + " �̱�"; // ī�� �̸� + �̱�

            if(currentTableCardText.text == "")
                ButtonActivate();
            else if (currentTableCardText.text.Contains("�̱�") || currentTableCardText.text.Contains("����") || currentTableCardText.text.Contains("��Ȳ"))
                if(TableCardMatchInt < choiceCardNumber[0])
                    ButtonActivate();
            else
                NoCombination();
        }
        else if (betCardNum == 2)
        {
            if (choiceCardNumber[0] == choiceCardNumber[1])
            {
                betCardText.text = choiceCardString[0] + " ���";

                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("���") && ContinuePairNum == 1)
                    if (TableCardMatchInt < choiceCardNumber[0])
                        ButtonActivate();
                else
                    NoCombination();
            }
            else
                NoCombination();
        }
        else if (betCardNum == 3)
        {
            if (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[1] == choiceCardNumber[2])
            {
                betCardText.text = choiceCardString[0] + " Ʈ����";
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("Ʈ����"))
                {
                    if (TableCardMatchInt < choiceCardNumber[0])
                        ButtonActivate();
                }
            }
            else
                NoCombination();
        }
        else if (betCardNum == 4)
        {
            if (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[1] == choiceCardNumber[2] && choiceCardNumber[2] == choiceCardNumber[3])
            {
                betCardText.text = choiceCardString[0] + " ��ź(��ī��)";
                if (!(TableCardMatchInt > choiceCardNumber[0] && currentTableCardText.text.Contains("��ź(��ī��)")) || !currentTableCardText.text.Contains("��Ƽ��"))
                {
                    betCardButton.GetComponent<Button>().interactable = true;
                    betCardButton.GetComponent<Image>().color = new Color(0, 0, 1);
                    betCardText.color = new Color(1, 1, 1);
                }
            }
            else if (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[2] == choiceCardNumber[3] && choiceCardNumber[1] == choiceCardNumber[2] - 1)
            {
                betCardText.text = choiceCardString[2] + " ���� ���";
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("���") && ContinuePairNum == 2)
                {
                    if (TableCardMatchInt < choiceCardNumber[2])
                        ButtonActivate();
                }
            }
            else
                NoCombination();
        }
        else if (betCardNum == 5)
        {
            if (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[1] == choiceCardNumber[2] && choiceCardNumber[3] == choiceCardNumber[4])
            {
                betCardText.text = choiceCardString[0] + " Ǯ�Ͽ콺";
                isStraight = false;
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("Ǯ�Ͽ콺"))
                {
                    if (TableCardMatchInt < choiceCardNumber[0])
                        ButtonActivate();
                }
            }
            else if (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[2] == choiceCardNumber[3] && choiceCardNumber[3] == choiceCardNumber[4])
            {
                betCardText.text = choiceCardString[3] + " Ǯ�Ͽ콺";
                isStraight = false;
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("Ǯ�Ͽ콺"))
                {
                    if (TableCardMatchInt < choiceCardNumber[3])
                        ButtonActivate();
                }
            }
            else
                NoCombination();
        }
        else if (betCardNum == 6)
        {
            if (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[2] == choiceCardNumber[3] && choiceCardNumber[4] == choiceCardNumber[5] && choiceCardNumber[1] == choiceCardNumber[2] - 1 && choiceCardNumber[3] == choiceCardNumber[4] - 1)
            {
                betCardText.text = choiceCardString[4] + " ���� ���";
                isStraight = false;
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("���") && ContinuePairNum == 3)
                    if (TableCardMatchInt < choiceCardNumber[4])
                        ButtonActivate();
            }
        }
        else if (betCardNum == 8)
        {
            if (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[2] == choiceCardNumber[3] && choiceCardNumber[4] == choiceCardNumber[5] && choiceCardNumber[6] == choiceCardNumber[7] && choiceCardNumber[1] == choiceCardNumber[2] - 1 && choiceCardNumber[3] == choiceCardNumber[4] - 1 && choiceCardNumber[5] == choiceCardNumber[6] - 1)
            {
                betCardText.text = choiceCardString[6] + " ���� ���";
                isStraight = false; 
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("���") && ContinuePairNum == 4)
                    if (TableCardMatchInt < choiceCardNumber[6])
                        ButtonActivate();
            }
        }
        else if (betCardNum == 10)
        {
            if (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[2] == choiceCardNumber[3] && choiceCardNumber[4] == choiceCardNumber[5] && choiceCardNumber[6] == choiceCardNumber[7] && choiceCardNumber[8] == choiceCardNumber[9] && choiceCardNumber[1] == choiceCardNumber[2] - 1 && choiceCardNumber[3] == choiceCardNumber[4] - 1 && choiceCardNumber[5] == choiceCardNumber[6] - 1 && choiceCardNumber[7] == choiceCardNumber[8] - 1)
            {
                betCardText.text = choiceCardString[8] + " ���� ���";
                isStraight = false;
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("���") && ContinuePairNum == 5)
                    if (TableCardMatchInt < choiceCardNumber[8])
                        ButtonActivate();
            }
        }
        else if (betCardNum == 12)
        {
            if (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[2] == choiceCardNumber[3] && choiceCardNumber[4] == choiceCardNumber[5] && choiceCardNumber[6] == choiceCardNumber[7] && choiceCardNumber[8] == choiceCardNumber[9] && choiceCardNumber[10] == choiceCardNumber[11] && choiceCardNumber[1] == choiceCardNumber[2] - 1 && choiceCardNumber[3] == choiceCardNumber[4] - 1 && choiceCardNumber[5] == choiceCardNumber[6] - 1 && choiceCardNumber[7] == choiceCardNumber[8] - 1 && choiceCardNumber[9] == choiceCardNumber[10] - 1)
            {
                betCardText.text = choiceCardString[10] + " ���� ���";
                isStraight = false;
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("���") && ContinuePairNum == 6)
                    if (TableCardMatchInt < choiceCardNumber[10])
                        ButtonActivate();
            }
        }
        else if (betCardNum == 14)
        {
            if (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[2] == choiceCardNumber[3] && choiceCardNumber[4] == choiceCardNumber[5] && choiceCardNumber[6] == choiceCardNumber[7] && choiceCardNumber[8] == choiceCardNumber[9] && choiceCardNumber[10] == choiceCardNumber[11] && choiceCardNumber[12] == choiceCardNumber[13] && choiceCardNumber[1] == choiceCardNumber[2] - 1 && choiceCardNumber[3] == choiceCardNumber[4] - 1 && choiceCardNumber[5] == choiceCardNumber[6] - 1 && choiceCardNumber[7] == choiceCardNumber[8] - 1 && choiceCardNumber[9] == choiceCardNumber[10] - 1 && choiceCardNumber[11] == choiceCardNumber[12] - 1)
            {
                betCardText.text = choiceCardString[12] + " ���� ���";
                isStraight = false;
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("���") && ContinuePairNum == 7)
                    if (TableCardMatchInt < choiceCardNumber[12])
                        ButtonActivate();
            }
        }

        if (betCardNum > 4 && isStraight)
        {
            int isStraight = betCardNum - 1;
            for (int i = 0; i < betCardNum - 1; i++)
            {
                if (choiceCardNumber[i] == choiceCardNumber[i + 1] - 1)
                {
                    isStraight--;
                }
            }
            if (isStraight == 0)
            {
                betCardText.text = "";
                if (choiceCardString.Contains("��Ȳ"))
                {
                    if (PhyPosLs)
                        betCardText.text = (int.Parse(choiceCardString[betCardNum - 2]) + 1) + " ��Ʈ����Ʈ(" + betCardNum + " ��)";
                    else
                        betCardText.text = choiceCardString[betCardNum - 2] + " ��Ʈ����Ʈ(" + betCardNum + " ��)";
                }
                else
                {
                    bool StraightFlush = true;
                    for (int i = 0; i < betCardNum - 1; i++)
                    {
                        if (choiceCardString[i].Substring(0, 1) != choiceCardString[i + 1].Substring(0, 1))
                            StraightFlush = false;
                    }
                    if (StraightFlush)
                    {
                        betCardText.text = choiceCardString[betCardNum - 1] + " ��ź(" + betCardNum + " ��Ƽ��)";
                        ButtonActivate();
                    }
                    else
                        betCardText.text = choiceCardString[betCardNum - 1] + " ��Ʈ����Ʈ(" + betCardNum + " ��)";
                }
                    
                betCardText.fontSize = 30;
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("��Ʈ����Ʈ") && betCardText.text.Split('(')[1].Substring(0, 1) == betCardNum + "")
                {
                    if (TableCardMatchInt < choiceCardNumber[betCardNum - 1])
                        ButtonActivate();
                }
                //else
                //    NoCombination();
            }
            else
                NoCombination();
        }

        isSelectCard = false;
        #endregion
    }

    [PunRPC]
    void PhynixBuffer(float TCMI) => phynixBufferOfNumber = TCMI + 0.5f;

    void NoCombination() // ���� ���� �� ��
    {
        if(betCardNum != 1)
            betCardText.text = "���� ����";       
        betCardButton.GetComponent<Button>().interactable = false;
        betCardButton.GetComponent<Image>().color = new Color(1, 1, 1);
        betCardText.color = new Color(0, 0, 0);
    }

    void ButtonActivate() // ��ư Ȱ��ȭ
    {
        if (myOrder)
        {
            betCardButton.GetComponent<Button>().interactable = true;
            betCardButton.GetComponent<Image>().color = new Color(0, 0, 1);
            betCardText.color = new Color(1, 1, 1);
        }
    }
    #endregion
    
    #region ī�� ���� & �н�
    public void CardBet()
    {
        cardNum.Clear();
        divingPlayerCount = 3;
        HopeErrorTest(0);
    }

    void RealCardBetting()
    {
        string BettingPlayer = PhotonNetwork.LocalPlayer.NickName;

        for (int i = 0; i < 14; i++)
        {
            if (handCard[i].gameObject.GetComponent<Card>().SCard)
            {
                if (handCard[i].sprite.name == "����" && !hopeIsOne)
                {
                    hopeHyunMooPanel.SetActive(true);
                    tichuBet.SetActive(false);
                    hopeDelay = true;
                    hopeIsOne = true;
                }
                else if (handCard[i].sprite.name == "��ȣ")
                {
                    for(int j = 0; j < 4; j++)
                    {
                        if (PhotonNetwork.LocalPlayer.NickName == playerOrder[j])
                        {
                            PV.RPC("WhiteTiger", RpcTarget.All);
                            myOrder = false; // �� ���ʸ� ������.
                            SelectedCardButton();
                            PV.RPC("NextTurn", RpcTarget.AllViaServer, j + 2 < 4 ? j + 2 : j - 2);
                            doTurn = false;
                        }
                    }
                }

                if (!hopeDelay)
                {
                    #region ���̺�� ���� ����
                    if (handCard[i].sprite.name.Substring(2) == "K" || handCard[i].sprite.name.Substring(2) == "10")
                        PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, 10, 0);
                    else if (handCard[i].sprite.name.Substring(2) == "5")
                        PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, 5, 0);
                    else if (handCard[i].sprite.name == "��Ȳ")
                        PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, -25, 0);
                    else if (handCard[i].sprite.name == "û��")
                        PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, 25, 0);
                    #endregion

                    cardNum.Add(handCard[i].gameObject.GetComponent<Card>().CardNum);
                    handCard[i].gameObject.GetComponent<Card>().SCard = false;
                    handCard[i].gameObject.GetComponent<Button>().enabled = false;
                    handCard[i].gameObject.GetComponent<Image>().sprite = cardBlank;
                    handCard[i].gameObject.GetComponent<Image>().enabled = false;
                }
            }
        }

        if (!hopeDelay)
        {
            switch (betCardNum)
            {
                case 1:
                    submitCard--;
                    PV.RPC("TableInCard", RpcTarget.AllViaServer, cardNum[0], 7, betCardText.text, 0, BettingPlayer);
                    break;
                case 2:
                    submitCard -= 2;
                    for (int k = 6, j = 0; k < 9; k += 2, j++)
                        PV.RPC("TableInCard", RpcTarget.AllViaServer, cardNum[j], k, betCardText.text, 1, BettingPlayer);
                    break;
                case 3:
                    submitCard -= 3;
                    for (int k = 5, j = 0; k < 10; k += 2, j++)
                        PV.RPC("TableInCard", RpcTarget.AllViaServer, cardNum[j], k, betCardText.text, 0, BettingPlayer);
                    break;
                case 4:
                    submitCard -= 4;
                    for (int k = 4, j = 0; k < 11; k += 2, j++)
                        PV.RPC("TableInCard", RpcTarget.AllViaServer, cardNum[j], k, betCardText.text, 2, BettingPlayer);
                    break;
                case 5:
                    submitCard -= 5;
                    for (int k = 3, j = 0; k < 12; k += 2, j++)
                        PV.RPC("TableInCard", RpcTarget.AllViaServer, cardNum[j], k, betCardText.text, 0, BettingPlayer);
                    break;
                case 6:
                    submitCard -= 6;
                    for (int k = 2, j = 0; k < 13; k += 2, j++)
                        PV.RPC("TableInCard", RpcTarget.AllViaServer, cardNum[j], k, betCardText.text, 3, BettingPlayer);
                    break;
                case 7:
                    submitCard -= 7;
                    for (int k = 4, j = 0; k < 11; k++, j++)
                        PV.RPC("TableInCard", RpcTarget.AllViaServer, cardNum[j], k, betCardText.text, 0, BettingPlayer);
                    break;
                case 8:
                    submitCard -= 8;
                    for (int k = 3, j = 0; k < 11; k++, j++)
                        PV.RPC("TableInCard", RpcTarget.AllViaServer, cardNum[j], k, betCardText.text, 4, BettingPlayer);
                    break;
                case 9:
                    submitCard -= 9;
                    for (int k = 3, j = 0; k < 12; k++, j++)
                        PV.RPC("TableInCard", RpcTarget.AllViaServer, cardNum[j], k, betCardText.text, 0, BettingPlayer);
                    break;
                case 10:
                    submitCard -= 10;
                    for (int k = 2, j = 0; k < 12; k++, j++)
                        PV.RPC("TableInCard", RpcTarget.AllViaServer, cardNum[j], k, betCardText.text, 5, BettingPlayer);
                    break;
                case 11:
                    submitCard -= 11;
                    for (int k = 2, j = 0; k < 13; k++, j++)
                        PV.RPC("TableInCard", RpcTarget.AllViaServer, cardNum[j], k, betCardText.text, 0, BettingPlayer);
                    break;
                case 12:
                    submitCard -= 12;
                    for (int k = 1, j = 0; k < 13; k++, j++)
                        PV.RPC("TableInCard", RpcTarget.AllViaServer, cardNum[j], k, betCardText.text, 6, BettingPlayer);
                    break;
                case 13:
                    submitCard -= 13;
                    for (int k = 1, j = 0; k < 14; k++, j++)
                        PV.RPC("TableInCard", RpcTarget.AllViaServer, cardNum[j], k, betCardText.text, 0, BettingPlayer);
                    break;
                case 14:
                    submitCard -= 14;
                    for (int k = 0, j = 0; k < 14; k++, j++)
                        PV.RPC("TableInCard", RpcTarget.AllViaServer, cardNum[j], k, betCardText.text, 7, BettingPlayer);
                    break;
            }

            if (submitCard == 0)
            {
                RankingUpdate();

                if (allPassNum == 1)
                {
                    Score += TableScore;
                    MyScore.text = Score + "";
                    handCardPanel.GetComponent<Image>().color = new Color(0, 0, 0);
                    PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, 0, 1);
                    doTurn = false;

                    if (PhotonNetwork.LocalPlayer.NickName == playerOrder[1] || PhotonNetwork.LocalPlayer.NickName == playerOrder[3])
                        PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, Score, 2);
                    else if (PhotonNetwork.LocalPlayer.NickName == playerOrder[0] || PhotonNetwork.LocalPlayer.NickName == playerOrder[2])
                        PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, Score, 3);
                }
                Invoke("TakeOneTwoRank", 1f);
            }


            ScoreExit = false;

            betCardNum = 0;
            betCardButton.GetComponent<Button>().interactable = false;
            betCardButton.GetComponent<Image>().color = new Color(1, 1, 1);
            betCardText.color = new Color(0, 0, 0);
            betCardText.text = "���� ����";

            if (doTurn)
            {
                PV.RPC("TurnEnd", RpcTarget.AllViaServer, 0);
            }
        }
    }

    public void PassBet()
    {
        if (myOrder && currentTableCardText.text != "")
            HopeErrorTest(1);
    }

    #region ����
    #region ������ �ҿ� �гη� ���� �ɱ�
    public void HopeNumber(string h)
    {
        hopeHyunMooPanel.SetActive(false);
        hopeDelay = false;
        tichuBet.SetActive(true);
        RealCardBetting();
        PV.RPC("HopeNumberPublish", RpcTarget.AllViaServer, h);
    }

    [PunRPC]
    void HopeNumberPublish(string h)
    {
        hopeString = h;
        hopeText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "  ������ �ҿ� :  " + h + "  ";
    }
    #endregion

    #region ������ ���ϴ� ī�尡 �ִ��� Ȯ�� �� ����
    void HopeErrorTest(int betOrPass)
    {
        bool haveHope = false;
        bool wantHyunMoo = true;
        
        if(hopeString == "")
        {
            if(betOrPass == 0)
                RealCardBetting();
            else if(betOrPass == 1)
                PV.RPC("TurnEnd", RpcTarget.AllViaServer, 1);
        }
        else // ���� �ؽ�Ʈ�� ��� ���� �ʴٸ�
        {
            hopeList.Clear();
            for (int i = 0; i < 14; i++) 
                if (handCard[i].sprite.name[2..] == hopeString) // �� �տ� ������ ���ϴ� ī�尡 �ִٸ�
                    haveHope = true;

            if (haveHope)
            {
                for(int i = 0; i < 14; i++)
                    if (handCard[i].gameObject.GetComponent<Card>().SCard) // ���õ� ī����� hopeList�� �߰�
                        hopeList.Add(handCard[i].sprite.name[2..]);

                if(!hopeList.Contains(hopeString)) // hopeList�� ������ ���ϴ� ī�尡 ���ԵǾ� ���� �ʴٸ� ( +�߰� �Ǿ���� ��: ���̺��� ī�尡 ��Ʈ����Ʈ�� ��� �� �� ���ٸ� �н��� �����ϰ� )
                    wantHyunMoo = false;

                //if (currentTableCardText.text.Contains("��Ʈ����Ʈ"))
                //{
                //    for(int i = 0; i < 14; i++)
                //    {
                //        if(int.Parse(currentTableCardText.text.Split('(')[1].Substring(0, 1)) == i)
                //        {
                //            for (int j = 0; j < i; j++) 
                //            { 
                //            }
                //        }
                //    }
                //}
            }

            if (!wantHyunMoo)
            {
                hopeError.SetActive(true);
                CancelInvoke("HopeErrorEnd");
                Invoke("HopeErrorEnd", 3f);
            }
            else
            {
                if (betOrPass == 0)
                {
                    if(haveHope == true)
                        PV.RPC("HopeErrorTestPublish", RpcTarget.AllViaServer);
                    RealCardBetting();
                }
                else if (betOrPass == 1)
                    PV.RPC("TurnEnd", RpcTarget.AllViaServer, 1);
            }
        }
    }

    [PunRPC]
    void HopeErrorTestPublish()
    {
        hopeString = "";
        hopeText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
    }

    void HopeErrorEnd() => hopeError.SetActive(false);
    #endregion
    #endregion

    #region ��ȣ
    [PunRPC]
    void WhiteTiger() => StartCoroutine("WhiteTigerNextTurn");

    IEnumerator WhiteTigerNextTurn()
    {
        doTurn = true;
        yield return new WaitForSeconds(1f);
        Debug.Log("����");
        tableCard[7].enabled = false;
        currentTableCardTextBackGroundImage.color = new Color(1, 1, 1, 0);
        currentTableCardText.text = "";
        allPass = allPassNum;
    }
    #endregion

    [PunRPC]
    void AllPassRPC(int p) => allPassNum -= p;

    [PunRPC]
    void TableInCard(int CDNM, int l, string CurrentBetCardText, int CPN, string BettingPlayer)
    {
        Material[] CDMT = tableCard[l].materials;
        CDMT[1] = cardMaterial[CDNM];
        tableCard[l].materials = CDMT;
        tableCard[l].enabled = true;
        ContinuePairNum = CPN;
        currentTableCardTextBackGroundImage.color = new Color(1, 1, 1, 1);
        currentTableCardText.text = CurrentBetCardText + "_" + BettingPlayer;
        allPass = allPassNum;
    }

    void TakeOneTwoRank()
    {
        if (ranking.Count >= 2)
        {
            if ((ranking[0] == playerOrder[0] && ranking[1] == playerOrder[2]) || (ranking[0] == playerOrder[2] && ranking[1] == playerOrder[0])) // ��� ���
            {
                LastScoreApply = false;
                doTurn = false;
                PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, 0, 6);
                PV.RPC("AllPassRPC", RpcTarget.All, 2);
            }
            else if ((ranking[0] == playerOrder[1] && ranking[1] == playerOrder[3]) || (ranking[0] == playerOrder[3] && ranking[1] == playerOrder[1])) // ���� ����
            {
                LastScoreApply = false;
                doTurn = false;
                PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, 0, 7);
                PV.RPC("AllPassRPC", RpcTarget.All, 2);
            }
            else
                PV.RPC("AllPassRPC", RpcTarget.All, 1);
        }
        else
            PV.RPC("AllPassRPC", RpcTarget.All, 1);
    }
    #endregion

    #region �� �ѱ��
    [PunRPC]
    void TurnEnd(int isPass) // ī�带 ���� ȣ��
    {
        allPass -= isPass;
        for(int i = 0; i<4; i++)
        {
            if (PhotonNetwork.LocalPlayer.NickName == playerOrder[i] && myOrder == true) // ���� ���� 1�� �ɾ��ְ�, �� ���ʿ��ٸ� - 1����, 1�� myOrder ����
            {
                myOrder = false; // �� ���ʸ� ������.
                SelectedCardButton();
                PV.RPC("NextTurn", RpcTarget.AllViaServer, i + 1 < 4 ? i + 1 : i - 3);
            }
        }

        if (allPass == 0)
        {
            if(tableCard[0].materials[1].name == "û��")
            {
                changeCardPNPanel.transform.GetChild(0).gameObject.SetActive(true);
                changeCardPNPanel.transform.GetChild(2).gameObject.SetActive(true);
            }
            for (int l = 0; l < 14; l++)
                tableCard[l].enabled = false;
            currentTableCardTextBackGroundImage.color = new Color(1, 1, 1, 0);
            currentTableCardText.text = "";
            allPass = allPassNum;
        }
    }

    [PunRPC]
    void NextTurn(int Players_Orders)
    {
        for(int i = 0; i < 4; i++)
        {
            if (PhotonNetwork.LocalPlayer.NickName == playerOrder[i] && Players_Orders == i)
            {
                TableToMyScore();

                if (submitCard == 0)
                {
                    PV.RPC("NextTurn", RpcTarget.AllViaServer, i + 1 < 4 ? i + 1 : i - 3);
                    if (!turnExit)
                    {
                        PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, Score, i % 2 == 0 ? 3 : 2);
                        turnExit = true;
                    }
                }
                else
                {
                    PV.RPC("WhoTurn", RpcTarget.AllViaServer, i);
                    SelectedCardButton();
                    myOrder = true;
                }
            }
        }

        if (myOrder)
            handCardPanel.GetComponent<Image>().color = new Color(0.39f, 0, 0);
        else
            handCardPanel.GetComponent<Image>().color = new Color(0, 0, 0);
    }

    [PunRPC]
    void WhoTurn(int j) // �� ���� �� �� �̸��� Ÿ�̸Ӱ� �������� ��
    {
        currentPlayerTimer.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = playerOrder[j] + "(���" + (3 - divingPlayerCount) + "ȸ)";
        currentPlayerTimer.transform.GetChild(1).GetComponent<Image>().fillAmount = 1;
        
        if (j % 2 == 0)
            currentPlayerTimer.transform.GetChild(1).GetComponent<Image>().color = new Color(0, 0, 1, 0.35f);
        else
            currentPlayerTimer.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 0, 0, 0.35f);
    }
    #endregion

    #region ���� ������Ʈ
    void RankingUpdate()
    {
        if (rankChange)
        {
            PV.RPC("RankingApply", RpcTarget.AllBufferedViaServer, PhotonNetwork.LocalPlayer.NickName);
            rankChange = false;
        }
    }

    [PunRPC]
    void RankingApply(string PlayerStr) => ranking.Add(PlayerStr);
    #endregion

    #region ���� ������Ʈ
    void TableToMyScore()
    {
        if (currentTableCardText.text == "" && !ScoreExit)
        {
            Score += TableScore;
            MyScore.text = Score + "";
            PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, 0, 1);
            ScoreExit = true;
        }
    }

    void PPS() //Post Process Score, ���� ��ó��
    {
        if ((ranking[1] == playerOrder[0] && ranking[2] == playerOrder[2]) || (ranking[1] == playerOrder[2] && ranking[2] == playerOrder[0])) // ���� ��� ��� ����
            PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, CRedTeamScore, 4);
        else if ((ranking[1] == playerOrder[1] && ranking[2] == playerOrder[3]) || (ranking[1] == playerOrder[3] && ranking[2] == playerOrder[1])) // ��� ���� ���� ���
            PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, CBlueTeamScore, 5);
        else if ((ranking[1] == playerOrder[1] && ranking[2] == playerOrder[0]) || (ranking[1] == playerOrder[1] && ranking[2] == playerOrder[2]) || (ranking[1] == playerOrder[3] && ranking[2] == playerOrder[0]) || (ranking[1] == playerOrder[3] && ranking[2] == playerOrder[2])) // ��� ���� ��� ����
            PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, CRedTeamScore, 4);
        else if ((ranking[1] == playerOrder[0] && ranking[2] == playerOrder[1]) || (ranking[1] == playerOrder[0] && ranking[2] == playerOrder[3]) || (ranking[1] == playerOrder[2] && ranking[2] == playerOrder[1]) || (ranking[1] == playerOrder[2] && ranking[2] == playerOrder[3])) // ���� ��� ���� ���
            PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, CBlueTeamScore, 5);

        if (isLargeTichu && (ranking[0] == playerOrder[1] || ranking[0] == playerOrder[3])) // ���尡 1���ε� ����Ƽ��
            PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, 200, 2);
        else if(isLargeTichu && (ranking[0] == playerOrder[0] || ranking[0] == playerOrder[2])) // ��簡 1���ε� ����Ƽ��
            PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, 200, 3);
    }

    [PunRPC]
    void ScoreManager(int TS, int v)
    {
        if (v == 0)
            TableScore += TS;
        else if (v == 1)
            TableScore = 0;
        else if (v == 2)
            CRedTeamScore += TS;
        else if (v == 3)
            CBlueTeamScore += TS;
        else if (v == 4)
            CBlueTeamScore = 100 - TS;
        else if (v == 5)
            CRedTeamScore = 100 - TS;
        else if (v == 6)
        {
            CBlueTeamScore = 200;
            CRedTeamScore = 0;
        }
        else if (v == 7)
        {
            CBlueTeamScore = 0;
            CRedTeamScore = 200;
        }
    }
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            for (int i = 0; i < allCard.Count; i++)
            {
                stream.SendNext(allCard[i]);
            }
        }
        else
        {
            for (int i = 0; i < allCard.Count; i++)
            {
                allCard[i] = (float)stream.ReceiveNext();
            }
        }
    }
}
