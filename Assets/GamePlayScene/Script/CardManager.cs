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
    #region 변수
    public PhotonView PV;
    public GameObject myPlayer;

    [Header("게임 시작")]
    public GameObject gameStartButton;          // 게임 시작 버튼
    private bool isGameStart = false;           // 게임이 시작하였는가?
    public static int numberOfPlayersSitting;   // 앉아 있는 플레이어의 수 => 플레이어의 수가 0이 되면 게임 시작 버튼이 보여짐

    [Header("타이머")]
    public GameObject currentPlayerTimer;       // 타이머 오브젝트
    public GameObject OutGameManager;           // GameManager 오브젝트
    private int loundTime = 20;                 // 제한 시간
    public int divingPlayerCount = 3;           // 잠수하는 플레이어의 카운트 => 0이 되면 게임에서 나가진다. 
    private bool TimerOn = false;               // 타이머 오브젝트가 활성화 되고, 타이머의 시간이 줄어들기 시작

    [Header("카드 세팅_라지 티츄")]
    public GameObject largeTichuPanel;          // 라지티츄 할지 안할지 정하는 패널 
    private bool largeTichuPanelActive;         // 라지티츄 패널이 활성화 되어 있는가?
    private bool isLargeTichu = false;          // 라지티츄를 선언 했는가?
    private int numberOfLargeTichuState;        // 라지티츄 상태를 끝낸 사람의 수

    [Header("카드 세팅")]
    public GameObject animationCard;            // 카드 섞는 애니메이션이 있는 오브젝트
    public GameObject handCardPanel;            // 내 손에 있는 카드 패널
    public GameObject tichuBet;                 // 베팅, 패스, 스몰티츄가 있는 UI모음
    public Sprite[] cardSprite;                 // 모든 카드 이미지 스프라이트
    public Sprite cardBlank;                    // 빈 카드 이미지 스프라이트
    public Image[] handCard;                    // 내 손에 있는 카드
    public Material[] cardMaterial;             // 모든 카드 재질
    private List<float> allCard = new List<float>(new float[56]);       // 전체 카드
    public List<float> myCard = new List<float>(new float[14]);         // 내 손의 카드
    private List<string> choiceCardString = new List<string>();         // 카드 텍스트 리스트
    private List<float> choiceCardNumber = new List<float>();           // 카드 숫자 리스트
    private List<int> cardNum = new List<int>();
    private float phynixBufferOfNumber = 0;
    //public static bool SuffleAnimationEnd = false;                    // 카드 섞는 애니메이션이 끝났는가?

    [Header("카드 세팅_카드 분배")]
    public GameObject changeCardPNPanel;        // 카드 분배하는 패널
    public TextMeshProUGUI[] changeCardPN;      // 카드 분배하는 패널의 각 버튼의 UI 텍스트
    public List<int> player1ChangeCardNumber = new List<int>();
    public List<int> player2ChangeCardNumber = new List<int>();
    public List<int> player3ChangeCardNumber = new List<int>();
    public List<int> player4ChangeCardNumber = new List<int>();
    public bool changeCardState = false;        // 카드 분배 시간
    private bool changeCardLoop = true;         // 카드를 다 분배했는지 확인하기 위해 코루틴 안에 루프를 돌리는 변수

    [Header("카드 베팅")]
    public GameObject betCardButton;            // 베팅 버튼
    public TextMeshProUGUI betCardText;         // 베팅 버튼 텍스트
    public TextMeshProUGUI currentTableCardText;        // 현재 테이블에 있는 카드
    public Image currentTableCardTextBackGroundImage;   // 현재 테이블에 있는 카드 이미지
    public MeshRenderer[] tableCard;            // 테이블에 보이는 카드의 렌더러
    private bool isStraight = true;             // 스트레이트 인가?
    private int ContinuePairNum = 0;            // 연속 패어의 숫자
    public static bool isSelectCard = false;    // 카드가 선택되면 true로 바뀌고 SelectedCardButton() 함수 호출 후 false로 바뀐다.
    public static int betCardNum = 0;           // 선택된 카드의 숫자

    [Header("카드 베팅_현무")]
    public GameObject hopeHyunMooPanel;         // 현무의 소원 패널
    public GameObject hopeText;                 // 현무의 소원을 지정하였을 때 모두에게 알려주는 텍스트
    public GameObject hopeError;                // 현무의 소원에 맞지 않는 카드를 제출했을 때 오류 패널
    private bool hopeDelay = false;
    private bool hopeIsOne = false;
    private string hopeString = "";             // 현무의 소원하는 카드의 문자열
    private List<string> hopeList = new List<string>(); // 내가 선택한 카드들을 담는 리스트, 현무의 소원에 맞는 카드가 있는지 확인하기 위한 리스트

    [Header("순서")]
    private bool myOrder = false;               // 내 차례 인가?
    private bool turnExit = false;              // 내 패가 없다면 한 번 실행 후 끝내기
    private bool doTurn = true;                 // 내 패가 없다면 턴 넘기는 곳으로 넘기지 않는다.
    private int allPass = 3;                    // 플레이어가 끝날 때 마다 패스 -1
    private int allPassNum = 3;                 // 플레이어의 턴이 끝날 때 -1, 카드를 내면 3
    private int submitCard = 14;                // 내 패의 개수
    public static string[] playerOrder = new string[4];     // 의자 순서대로 앉은 플레이어를 저장

    [Header("순위")]
    public List<string> ranking = new List<string>();       // 순위
    private bool rankChange = true;                         // 랭크를 추가할 때 한 번만 실행하도록 하는 변수

    [Header("스코어")]
    public GameObject ScorePanel;               // 팀별로 점수를 나타내는 패널
    public TextMeshProUGUI[] RedTeamScore;      // 레드팀 스코어 텍스트
    public TextMeshProUGUI[] BlueTeamScore;     // 블루팀 스코어 텍스트
    public TextMeshProUGUI MyScore;             // 내 점수 텍스트(없어질 가능성 ▲)
    public int CRedTeamScore = 0;               // 레드팀 스코어
    public int CBlueTeamScore = 0;              // 블루팀 스코어
    public int Round = 0;                       // 라운드
    public int TableScore = 0;                  // 테이블에 나온 카드의 점수의 합
    public int Score = 0;                       // 내 점수
    private bool ScoreExit = false;             // 내 점수를 업데이트 할 때 한 번만 추가되도록 하는 변수
    private bool LastScoreApply = true;         // 1, 2 등이 같은 팀일 경우 남은 점수 계산을 안하도록 생략하는 변수
    #endregion

    void Start()
    {
        numberOfPlayersSitting = PhotonNetwork.CurrentRoom.PlayerCount; // 모든 플레이어가 앉았는지 확인하기 위해 플레이어의 숫자를 저장
        numberOfLargeTichuState = PhotonNetwork.PlayerList.Length; // 라지 티츄를 했는지 확인하기 위해 플레이어의 숫자를 저장
        myPlayer = GameObject.Find(PhotonNetwork.LocalPlayer.NickName);
    }

    void Update()
    {
        ActiveGameStart(); // 게임 시작 버튼 보여주는 함수

        //OutLTichu(); // 라지 티츄 상태에서 벗어나면 실행되는 함수

        if (isSelectCard && !changeCardState) // 카드가 선택되면
            SelectedCardButton(); // 카드 선택_텍스트&버튼 활성화 함수 실행

        if(TimerOn)
            TimeDown();

        if (allPassNum == 0) // 게임 끝
            GameEnd();
    }

    #region 타이머
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
                    currentPlayerTimer.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "카드를 빨리 분배해 주세요.";
                else if(divingPlayerCount == 1)
                    currentPlayerTimer.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "카드를 분배하지 않을 경우 게임이 끝납니다.";

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

    #region 게임 끝 => 후처리
    void GameEnd()
    {
        if (LastScoreApply)
        {
            if (!ranking.Contains(PhotonNetwork.LocalPlayer.NickName)) // 꼴지의 경우
            {
                if (ranking[0] == playerOrder[1] || ranking[0] == playerOrder[3]) //1등이 레드
                    PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, Score, 2);
                else if (ranking[0] == playerOrder[0] || ranking[0] == playerOrder[2]) //1등이 블루
                    PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, Score, 3);
            }
            if (PhotonNetwork.LocalPlayer.NickName == ranking[0]) // 1등의 경우
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

    #region 게임 시작
    void ActiveGameStart() //게임 시작 버튼 보여주기
    {
        if (numberOfPlayersSitting == 0 && PhotonNetwork.IsMasterClient && !isGameStart) // 모든 플레이어가 앉고 게임이 시작하지 않았으면 마스터 클라이언트에서
            gameStartButton.SetActive(true); // 게임 시작 버튼 보여주기
    }

    public void OnGameStart() // 게임 시작 버튼을 누르면
    {
        CardShuffle(); // 카드 세팅_카드 섞기 함수 실행

        isGameStart = true; // 게임이 시작했다는 것을 알려줌
        gameStartButton.SetActive(false); // 게임 시작 버튼 숨기기
    }
    #endregion

    #region 카드 세팅
    #region 카드 섞기
    void CardShuffle()
    {
        int ran = 0; // 랜덤 값 받을 변수

        for (int i = 0; i < allCard.Count; i++) // 각 카드에 값 넣기 (순서대로)
            allCard[i] = i;

        for (int i = 0; i < allCard.Count; i++) // 각 카드에 넣어진 값을 서로 바꿔서 랜덤하게 바꾸기
        {
            float temp = allCard[i];
            ran = Random.Range(0, allCard.Count);
            allCard[i] = allCard[ran];
            allCard[ran] = temp;
        }

        PV.RPC("CardAnimation", RpcTarget.AllViaServer); // 카드 섞는 애니메이션을 모두 볼 수 있게 동기화
        Invoke("CardSynchronization", 1f);  // 애니메이션 기다리는 시간 1초 후 실행
    }

    [PunRPC]
    void CardAnimation()
    {
        if (!ScorePanel.activeSelf)
            ScorePanel.SetActive(true);
        animationCard.SetActive(true); //카드 섞는 애니메이션을 위해 카드를 활성화
        handCardPanel.GetComponent<Animator>().SetTrigger("ReCard");
    }

    void CardSynchronization() => PV.RPC("CardSetting", RpcTarget.AllViaServer); //카드 세팅 동기화
    #endregion

    #region 라지티츄
    public void LTichu(int i) // 라지티츄 버튼 클릭시 이벤트
    {
        if (i == 0) // 라티없음 눌렀을 때
            isLargeTichu = false;
        else if (i == 1) // 라지티츄 눌렀을 때
        {
            isLargeTichu = true;
            myPlayer.GetComponent<Animator>().SetBool("LT", true);
        }

        largeTichuPanel.SetActive(false); // 라지티츄 할지 안할지 정하는 판 끄기
        PV.RPC("LargeTichuState", RpcTarget.AllViaServer); // 라지티츄 상태 동기화
        PV.RPC("CardSetting", RpcTarget.AllViaServer); // 다시 카드 세팅
    }

    [PunRPC]
    void LargeTichuState() => numberOfLargeTichuState--;
    #endregion

    #region 카드 나눠주기
    [PunRPC]
    void CardSetting() // 카드 세팅
    {
        handCardPanel.SetActive(true); // 카드 판 활성화
        for (int PlayerNum = 0; PlayerNum < PhotonNetwork.PlayerList.Length; PlayerNum++) // 카드 분배
            if (PhotonNetwork.LocalPlayer.NickName == PhotonNetwork.PlayerList[PlayerNum].NickName) 
                for (int i = (PlayerNum * 14); i < ((PlayerNum + 1) * 14); i++)
                    myCard[i - (PlayerNum * 14)] = allCard[i];

        StartCoroutine("CardDistribution");
    }

    IEnumerator CardDistribution() // 카드 분배한 것에 텍스쳐 입히기 함수
    {
        if (numberOfLargeTichuState != 0) // 라지티츄 상태 전에
        {
            if (!largeTichuPanelActive) // 라지티츄 오브젝트 조정
            {
                largeTichuPanel.SetActive(true);
                largeTichuPanelActive = true;
            }
            for (int i = 0; i < 8; i++) // 카드 8장 분배
            {
                CSM.QuickSort(myCard, 0, myCard.Count - 7);
                handCard[i + 3].color = Color.white;
                handCard[i + 3].sprite = cardSprite[(int)myCard[i]];
            }
        }
        else if (numberOfLargeTichuState == 0) //라지티츄 상태 후에
        {
            animationCard.SetActive(false); // 카드 애니메이션 오브젝트 끄기
            myCard.Sort();
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < 14; i++) // 카드 14장 분배
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

    #region 카드 바꾸기
    void ChangeCard()
    {
        changeCardPNPanel.SetActive(true);
        currentPlayerTimer.SetActive(true);
        TimerOn = true;
        currentPlayerTimer.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "카드를 서로 분배하세요.";
        currentPlayerTimer.transform.GetChild(1).GetComponent<Image>().fillAmount = 1;
        changeCardState = true;
        for (int i = 0; i < 4; i++)
        {
            if (playerOrder[i] == PhotonNetwork.LocalPlayer.NickName)
            {
                changeCardPN[0].text = playerOrder[i - 1 == -1 ? 3 : i - 1]; // 왼쪽 적
                changeCardPN[1].text = playerOrder[i + 2 >= 4 ? i - 2 : i + 2]; // 바라보는 우리편 0 => 2, 1 => 3, 2 => 0(4), 3 => 1(5)
                changeCardPN[2].text = playerOrder[i + 1 == 4 ? 0 : i + 1]; // 오른쪽 적
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
            if (player1ChangeCardNumber.Count == 3 && player2ChangeCardNumber.Count == 3 && player3ChangeCardNumber.Count == 3 && player4ChangeCardNumber.Count == 3) //카드가 다 나눠지면
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
                for (int i = 0; i < 14; i++) // 카드 14장 분배
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
                tichuBet.SetActive(true); // 티츄 베팅 UI 활성화
                divingPlayerCount = 3;
                FirstOrder();
                StopCoroutine("ChangeCardEnd");
            }
        }
    }

    public void ChoiceChangeCard(int c) // 플레이어의 이름이 적힌 버튼을 눌렀을 때
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

    #region 순서 정하기
    void FirstOrder()
    {
        for (int i = 0; i < myCard.Count; i++)
            if (handCard[i].sprite.name == "현무")
                for (int j = 0; j < playerOrder.Length; j++)
                    if (playerOrder[j] == PhotonNetwork.LocalPlayer.NickName) //현무를 가지고 있는 사람
                    {
                        handCardPanel.GetComponent<Image>().color = new Color(0.39f, 0, 0);
                        PV.RPC("WhoTurn", RpcTarget.All, j);
                        myOrder = true;
                    }
    }
    #endregion

    #region 스몰티츄


    #endregion
    #endregion

    #region 카드 선택_텍스트&버튼 활성화
    void SelectedCardButton() // 카드가 선택되면 호출
    {
        choiceCardString.Clear(); // 리스트 초기화
        choiceCardNumber.Clear(); // 리스트 초기화
        float TableCardMatchInt = 0; // 테이블의 카드
        isStraight = true;
        bool Phynix = false;
        bool PhyPosLs = false;
        int ChoiceCardNumberNum = 0;
        //int HSCard = 0;
        betCardText.fontSize = 50; // 베팅 버튼 글자 크기 50
        NoCombination();

        #region 현재 테이블의 카드 숫자 읽어오기
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
        else if (currentTableCardText.text.Split('_')[0] == "현무")
            TableCardMatchInt = 1;
        else if (currentTableCardText.text.Split('_')[0] == "백호")
            TableCardMatchInt = -1;
        else if (currentTableCardText.text.Split('_')[0] == "봉황") //싱글일 경우
            TableCardMatchInt = phynixBufferOfNumber;
        else if (currentTableCardText.text.Split('_')[0] == "청룡")
            TableCardMatchInt = 100;
        else
            TableCardMatchInt = float.Parse(currentTableCardText.text.Split(' ')[0], CultureInfo.InvariantCulture);
        #endregion

        #region 선택한 카드 숫자 읽어오기
        for (int i = 0; i < allCard.Count / 4; i++) // 카드에 이름 및 숫자 추출
        {
            if (handCard[i].gameObject.GetComponent<Card>().SCard) // 선택된 카드만
            {
                if (handCard[i].sprite.name == "현무" || handCard[i].sprite.name == "백호" || handCard[i].sprite.name == "봉황" || handCard[i].sprite.name == "청룡") // 특수카드라면
                    choiceCardString.Add(handCard[i].sprite.name); // 특수카드 이름 그대로
                else // 아니라면
                    choiceCardString.Add(handCard[i].sprite.name.Substring(2)); // 숫자만            

                if (handCard[i].sprite.name.Substring(2) == "J") // J = 11
                    choiceCardNumber.Add(11);
                else if (handCard[i].sprite.name.Substring(2) == "Q") // Q = 12
                    choiceCardNumber.Add(12);
                else if (handCard[i].sprite.name.Substring(2) == "K") // K = 13
                    choiceCardNumber.Add(13);
                else if (handCard[i].sprite.name.Substring(2) == "A") // A = 14
                    choiceCardNumber.Add(14);
                else if (handCard[i].sprite.name == "현무") // 현무(= 참새) = 1
                    choiceCardNumber.Add(1);
                else if (handCard[i].sprite.name == "백호") // 백호 = -1
                    choiceCardNumber.Add(-1);
                else if (handCard[i].sprite.name == "봉황") // 선택한 카드의 이름에 봉황이 있으면
                {
                    Phynix = true;
                }
                else if (handCard[i].sprite.name == "청룡") // 청룡 = 100
                    choiceCardNumber.Add(100);
                else //나머지 각 카드 = 각 카드의 숫자
                    choiceCardNumber.Add(float.Parse(handCard[i].sprite.name.Substring(2), CultureInfo.InvariantCulture));

                //if (handCard[i].sprite.name != "봉황")
                //    HSCard = i;

                ChoiceCardNumberNum++; // 봉황 알고리즘을 위해..
            }
        }
        #endregion

        #region 봉황 알고리즘
        if (Phynix)
        {
            if (ChoiceCardNumberNum == 1) // 싱글일 경우
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
        
        choiceCardNumber.Sort((x, y) => x.CompareTo(y)); // 카드 정렬

        #region 카드 활성화 조건
        if (betCardNum == 0) // 선택된 카드가 0장 이라면
            NoCombination();
        else if (betCardNum == 1)
        {
            if (choiceCardString[0] == "현무" || choiceCardString[0] == "백호" || choiceCardString[0] == "봉황" || choiceCardString[0] == "청룡") // 특수 카드 라면
                betCardText.text = choiceCardString[0]; // 특수 카드 이름 그대로
            else
                betCardText.text = choiceCardString[0] + " 싱글"; // 카드 이름 + 싱글

            if(currentTableCardText.text == "")
                ButtonActivate();
            else if (currentTableCardText.text.Contains("싱글") || currentTableCardText.text.Contains("현무") || currentTableCardText.text.Contains("봉황"))
                if(TableCardMatchInt < choiceCardNumber[0])
                    ButtonActivate();
            else
                NoCombination();
        }
        else if (betCardNum == 2)
        {
            if (choiceCardNumber[0] == choiceCardNumber[1])
            {
                betCardText.text = choiceCardString[0] + " 페어";

                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("페어") && ContinuePairNum == 1)
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
                betCardText.text = choiceCardString[0] + " 트리플";
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("트리플"))
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
                betCardText.text = choiceCardString[0] + " 폭탄(포카드)";
                if (!(TableCardMatchInt > choiceCardNumber[0] && currentTableCardText.text.Contains("폭탄(포카드)")) || !currentTableCardText.text.Contains("스티플"))
                {
                    betCardButton.GetComponent<Button>().interactable = true;
                    betCardButton.GetComponent<Image>().color = new Color(0, 0, 1);
                    betCardText.color = new Color(1, 1, 1);
                }
            }
            else if (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[2] == choiceCardNumber[3] && choiceCardNumber[1] == choiceCardNumber[2] - 1)
            {
                betCardText.text = choiceCardString[2] + " 연속 페어";
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("페어") && ContinuePairNum == 2)
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
                betCardText.text = choiceCardString[0] + " 풀하우스";
                isStraight = false;
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("풀하우스"))
                {
                    if (TableCardMatchInt < choiceCardNumber[0])
                        ButtonActivate();
                }
            }
            else if (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[2] == choiceCardNumber[3] && choiceCardNumber[3] == choiceCardNumber[4])
            {
                betCardText.text = choiceCardString[3] + " 풀하우스";
                isStraight = false;
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("풀하우스"))
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
                betCardText.text = choiceCardString[4] + " 연속 페어";
                isStraight = false;
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("페어") && ContinuePairNum == 3)
                    if (TableCardMatchInt < choiceCardNumber[4])
                        ButtonActivate();
            }
        }
        else if (betCardNum == 8)
        {
            if (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[2] == choiceCardNumber[3] && choiceCardNumber[4] == choiceCardNumber[5] && choiceCardNumber[6] == choiceCardNumber[7] && choiceCardNumber[1] == choiceCardNumber[2] - 1 && choiceCardNumber[3] == choiceCardNumber[4] - 1 && choiceCardNumber[5] == choiceCardNumber[6] - 1)
            {
                betCardText.text = choiceCardString[6] + " 연속 페어";
                isStraight = false; 
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("페어") && ContinuePairNum == 4)
                    if (TableCardMatchInt < choiceCardNumber[6])
                        ButtonActivate();
            }
        }
        else if (betCardNum == 10)
        {
            if (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[2] == choiceCardNumber[3] && choiceCardNumber[4] == choiceCardNumber[5] && choiceCardNumber[6] == choiceCardNumber[7] && choiceCardNumber[8] == choiceCardNumber[9] && choiceCardNumber[1] == choiceCardNumber[2] - 1 && choiceCardNumber[3] == choiceCardNumber[4] - 1 && choiceCardNumber[5] == choiceCardNumber[6] - 1 && choiceCardNumber[7] == choiceCardNumber[8] - 1)
            {
                betCardText.text = choiceCardString[8] + " 연속 페어";
                isStraight = false;
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("페어") && ContinuePairNum == 5)
                    if (TableCardMatchInt < choiceCardNumber[8])
                        ButtonActivate();
            }
        }
        else if (betCardNum == 12)
        {
            if (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[2] == choiceCardNumber[3] && choiceCardNumber[4] == choiceCardNumber[5] && choiceCardNumber[6] == choiceCardNumber[7] && choiceCardNumber[8] == choiceCardNumber[9] && choiceCardNumber[10] == choiceCardNumber[11] && choiceCardNumber[1] == choiceCardNumber[2] - 1 && choiceCardNumber[3] == choiceCardNumber[4] - 1 && choiceCardNumber[5] == choiceCardNumber[6] - 1 && choiceCardNumber[7] == choiceCardNumber[8] - 1 && choiceCardNumber[9] == choiceCardNumber[10] - 1)
            {
                betCardText.text = choiceCardString[10] + " 연속 페어";
                isStraight = false;
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("페어") && ContinuePairNum == 6)
                    if (TableCardMatchInt < choiceCardNumber[10])
                        ButtonActivate();
            }
        }
        else if (betCardNum == 14)
        {
            if (choiceCardNumber[0] == choiceCardNumber[1] && choiceCardNumber[2] == choiceCardNumber[3] && choiceCardNumber[4] == choiceCardNumber[5] && choiceCardNumber[6] == choiceCardNumber[7] && choiceCardNumber[8] == choiceCardNumber[9] && choiceCardNumber[10] == choiceCardNumber[11] && choiceCardNumber[12] == choiceCardNumber[13] && choiceCardNumber[1] == choiceCardNumber[2] - 1 && choiceCardNumber[3] == choiceCardNumber[4] - 1 && choiceCardNumber[5] == choiceCardNumber[6] - 1 && choiceCardNumber[7] == choiceCardNumber[8] - 1 && choiceCardNumber[9] == choiceCardNumber[10] - 1 && choiceCardNumber[11] == choiceCardNumber[12] - 1)
            {
                betCardText.text = choiceCardString[12] + " 연속 페어";
                isStraight = false;
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("페어") && ContinuePairNum == 7)
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
                if (choiceCardString.Contains("봉황"))
                {
                    if (PhyPosLs)
                        betCardText.text = (int.Parse(choiceCardString[betCardNum - 2]) + 1) + " 스트레이트(" + betCardNum + " 개)";
                    else
                        betCardText.text = choiceCardString[betCardNum - 2] + " 스트레이트(" + betCardNum + " 개)";
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
                        betCardText.text = choiceCardString[betCardNum - 1] + " 폭탄(" + betCardNum + " 스티플)";
                        ButtonActivate();
                    }
                    else
                        betCardText.text = choiceCardString[betCardNum - 1] + " 스트레이트(" + betCardNum + " 개)";
                }
                    
                betCardText.fontSize = 30;
                if (currentTableCardText.text == "")
                    ButtonActivate();
                else if (currentTableCardText.text.Contains("스트레이트") && betCardText.text.Split('(')[1].Substring(0, 1) == betCardNum + "")
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

    void NoCombination() // 조합 없음 일 때
    {
        if(betCardNum != 1)
            betCardText.text = "조합 없음";       
        betCardButton.GetComponent<Button>().interactable = false;
        betCardButton.GetComponent<Image>().color = new Color(1, 1, 1);
        betCardText.color = new Color(0, 0, 0);
    }

    void ButtonActivate() // 버튼 활성화
    {
        if (myOrder)
        {
            betCardButton.GetComponent<Button>().interactable = true;
            betCardButton.GetComponent<Image>().color = new Color(0, 0, 1);
            betCardText.color = new Color(1, 1, 1);
        }
    }
    #endregion
    
    #region 카드 내기 & 패스
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
                if (handCard[i].sprite.name == "현무" && !hopeIsOne)
                {
                    hopeHyunMooPanel.SetActive(true);
                    tichuBet.SetActive(false);
                    hopeDelay = true;
                    hopeIsOne = true;
                }
                else if (handCard[i].sprite.name == "백호")
                {
                    for(int j = 0; j < 4; j++)
                    {
                        if (PhotonNetwork.LocalPlayer.NickName == playerOrder[j])
                        {
                            PV.RPC("WhiteTiger", RpcTarget.All);
                            myOrder = false; // 내 차례를 끝낸다.
                            SelectedCardButton();
                            PV.RPC("NextTurn", RpcTarget.AllViaServer, j + 2 < 4 ? j + 2 : j - 2);
                            doTurn = false;
                        }
                    }
                }

                if (!hopeDelay)
                {
                    #region 테이블로 점수 전달
                    if (handCard[i].sprite.name.Substring(2) == "K" || handCard[i].sprite.name.Substring(2) == "10")
                        PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, 10, 0);
                    else if (handCard[i].sprite.name.Substring(2) == "5")
                        PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, 5, 0);
                    else if (handCard[i].sprite.name == "봉황")
                        PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, -25, 0);
                    else if (handCard[i].sprite.name == "청룡")
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
            betCardText.text = "조합 없음";

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

    #region 현무
    #region 현무의 소원 패널로 조건 걸기
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
        hopeText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "  현무의 소원 :  " + h + "  ";
    }
    #endregion

    #region 현무가 원하는 카드가 있는지 확인 후 오류
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
        else // 현무 텍스트가 비어 있지 않다면
        {
            hopeList.Clear();
            for (int i = 0; i < 14; i++) 
                if (handCard[i].sprite.name[2..] == hopeString) // 내 손에 현무가 원하는 카드가 있다면
                    haveHope = true;

            if (haveHope)
            {
                for(int i = 0; i < 14; i++)
                    if (handCard[i].gameObject.GetComponent<Card>().SCard) // 선택된 카드들을 hopeList에 추가
                        hopeList.Add(handCard[i].sprite.name[2..]);

                if(!hopeList.Contains(hopeString)) // hopeList에 현무가 원하는 카드가 포함되어 있지 않다면 ( +추가 되어야할 것: 테이블의 카드가 스트레이트의 경우 낼 수 없다면 패스가 가능하게 )
                    wantHyunMoo = false;

                //if (currentTableCardText.text.Contains("스트레이트"))
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

    #region 백호
    [PunRPC]
    void WhiteTiger() => StartCoroutine("WhiteTigerNextTurn");

    IEnumerator WhiteTigerNextTurn()
    {
        doTurn = true;
        yield return new WaitForSeconds(1f);
        Debug.Log("실행");
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
            if ((ranking[0] == playerOrder[0] && ranking[1] == playerOrder[2]) || (ranking[0] == playerOrder[2] && ranking[1] == playerOrder[0])) // 블루 블루
            {
                LastScoreApply = false;
                doTurn = false;
                PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, 0, 6);
                PV.RPC("AllPassRPC", RpcTarget.All, 2);
            }
            else if ((ranking[0] == playerOrder[1] && ranking[1] == playerOrder[3]) || (ranking[0] == playerOrder[3] && ranking[1] == playerOrder[1])) // 레드 레드
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

    #region 턴 넘기기
    [PunRPC]
    void TurnEnd(int isPass) // 카드를 내면 호출
    {
        allPass -= isPass;
        for(int i = 0; i<4; i++)
        {
            if (PhotonNetwork.LocalPlayer.NickName == playerOrder[i] && myOrder == true) // 내가 의자 1에 앉아있고, 내 차례였다면 - 1접속, 1의 myOrder 끄기
            {
                myOrder = false; // 내 차례를 끝낸다.
                SelectedCardButton();
                PV.RPC("NextTurn", RpcTarget.AllViaServer, i + 1 < 4 ? i + 1 : i - 3);
            }
        }

        if (allPass == 0)
        {
            if(tableCard[0].materials[1].name == "청룡")
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
    void WhoTurn(int j) // 내 턴일 때 내 이름과 타이머가 굴러가게 함
    {
        currentPlayerTimer.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = playerOrder[j] + "(경고" + (3 - divingPlayerCount) + "회)";
        currentPlayerTimer.transform.GetChild(1).GetComponent<Image>().fillAmount = 1;
        
        if (j % 2 == 0)
            currentPlayerTimer.transform.GetChild(1).GetComponent<Image>().color = new Color(0, 0, 1, 0.35f);
        else
            currentPlayerTimer.transform.GetChild(1).GetComponent<Image>().color = new Color(1, 0, 0, 0.35f);
    }
    #endregion

    #region 순위 업데이트
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

    #region 점수 업데이트
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

    void PPS() //Post Process Score, 점수 후처리
    {
        if ((ranking[1] == playerOrder[0] && ranking[2] == playerOrder[2]) || (ranking[1] == playerOrder[2] && ranking[2] == playerOrder[0])) // 레드 블루 블루 레드
            PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, CRedTeamScore, 4);
        else if ((ranking[1] == playerOrder[1] && ranking[2] == playerOrder[3]) || (ranking[1] == playerOrder[3] && ranking[2] == playerOrder[1])) // 블루 레드 레드 블루
            PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, CBlueTeamScore, 5);
        else if ((ranking[1] == playerOrder[1] && ranking[2] == playerOrder[0]) || (ranking[1] == playerOrder[1] && ranking[2] == playerOrder[2]) || (ranking[1] == playerOrder[3] && ranking[2] == playerOrder[0]) || (ranking[1] == playerOrder[3] && ranking[2] == playerOrder[2])) // 블루 레드 블루 레드
            PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, CRedTeamScore, 4);
        else if ((ranking[1] == playerOrder[0] && ranking[2] == playerOrder[1]) || (ranking[1] == playerOrder[0] && ranking[2] == playerOrder[3]) || (ranking[1] == playerOrder[2] && ranking[2] == playerOrder[1]) || (ranking[1] == playerOrder[2] && ranking[2] == playerOrder[3])) // 레드 블루 레드 블루
            PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, CBlueTeamScore, 5);

        if (isLargeTichu && (ranking[0] == playerOrder[1] || ranking[0] == playerOrder[3])) // 레드가 1등인데 라지티츄
            PV.RPC("ScoreManager", RpcTarget.AllBufferedViaServer, 200, 2);
        else if(isLargeTichu && (ranking[0] == playerOrder[0] || ranking[0] == playerOrder[2])) // 블루가 1등인데 라지티츄
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
