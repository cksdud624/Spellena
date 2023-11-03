using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;
using System.Collections.Generic;
using Player;
using UnityEngine.UI;
using System;

public class GameCenterTest : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform playerSpawnPoints;
    public GameObject inGameUI;
    public GameObject etcUI;

    Transform[] spawnPoint = new Transform[2];

    GameObject occupy;
    GameObject[] occupyState;
    GameObject payLoad;
    Image[] teamPayLoad;
    GameObject percentage;
    Text[] teamPercentage;
    GameObject extraUI;
    Image[] teamExtraUI;
    GameObject extraTime;
    GameObject[] teamExtraTime;
    GameObject getPoint;
    GameObject[] teamGetPoint;

    GameObject gameStateText;
    Text gameStateTextUI;
    GameObject timerText;
    Text timerTextUI;

   
    public enum GameState
    {
        WaitingAllPlayer,
        MatchStart,
        CharacterSelect,
        Ready,
        Round,
        RoundEnd,
        MatchEnd,
        Result
    }

    public GameState gameState;

    struct OccupyingTeam
    {
        public string name;
        public float rate;
    }

    struct Occupation
    {
        public float rate;
    }

    // ���� ����
    int roundA = 0;
    int roundB = 0;

    //
    int teamAOccupying = 0;
    int teamBOccupying = 0;

    float globalTimer;
    float occupyingReturnTimer;
    float roundEndTimer;

    string currentOccupationTeam = "";  //���� �������� ��

    Occupation occupyingA;              //A���� ���ɵ�
    Occupation occupyingB;              //B���� ���ɵ�
    OccupyingTeam occupyingTeam;        //���� ������ ��

    Character[] players;

    //�� ���ϴ� �׽�Ʈ�� �Ͻ��� �����̴�.

    float loadingTime = 3f;
    float characterSelectTime = 5f; //���߿� ������ �Ŵ������� ���� �����������Ѵ�.
    float readyTime = 5f;

    //�߰� �ð�
    float roundEndTime = 5f;

    float occupyingGaugeRate = 100f;
    float occupyingReturnTime = 3f;
    float occupyingRate = 5f;
    float occupyingComplete = 100f;

    string teamA = "A";
    string teamB = "B";

    public Photon.Realtime.Player[] allPlayers;
    public List<GameObject> playersA = new List<GameObject>(); // Red
    public List<GameObject> playersB = new List<GameObject>(); // Blue

    void Start()
    {
        gameState = GameState.WaitingAllPlayer;
        otherUI = GameObject.Find("OtherUI");
        inGameUI = GameObject.Find("InGameUI");



        gameStateText = GameObject.Find("GameState");
        gameStateTextUI = gameStateText.GetComponent<Text>();
        timerText = GameObject.Find("Timer");
        timerTextUI = timerText.GetComponent<Text>();
    }

    void InitInGameUI()
    {
        occupy = GameObject.Find("Occupy");
        payLoad = GameObject.Find("Payload");
        percentage = GameObject.Find("Percentage");
        extraUI = GameObject.Find("ExtraUI");
        extraTime = GameObject.Find("ExtraTime");
        getPoint = GameObject.Find("GetPoint");

        for (int i = 0; i < 3; i++)
        {
            occupyState[i] = occupy.transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < 2; i++)
        {
            teamPayLoad[i] = payLoad.transform.GetChild(i).gameObject.GetComponent<Image>();
        }

        for (int i = 0; i < 2; i++)
        {
            teamPercentage[i] = percentage.transform.GetChild(i).gameObject.GetComponent<Text>();
        }

        for (int i = 0; i < 2; i++)
        {
            teamExtraUI[i] = extraUI.transform.GetChild(i).gameObject.GetComponent<Image>();
        }

        for (int i = 0; i < 4; i++)
        {
            teamExtraTime[i] = extraTime.transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < 3; i++)
        {
            teamGetPoint[i] = getPoint.transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < playerSpawnPoints.childCount; i++)
        {
            spawnPoint[i] = playerSpawnPoints.GetChild(i);
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            timerTextUI.text = ((int)globalTimer).ToString();

            if (gameState == GameState.WaitingAllPlayer)
            {
                gameStateTextUI.text = "�ٸ� �÷��̾� ��ٸ��� ��...";

                // ������ ���� ��ư ������ ����
                // �÷��̾�� �� ���� ����
                // ActorNumber�� �÷��̾� �ĺ�
                // allPlayers = PhotonNetwork.PlayerList;
                // custom property�� �� ���� (ActorNumber, name, team)

                int tempNum = 2;
                if(PhotonNetwork.CurrentRoom.PlayerCount>=tempNum)
                {
                    allPlayers = PhotonNetwork.PlayerList;
                    gameState = GameState.MatchStart;
                }

            }

            else if (gameState == GameState.MatchStart)
            {
                gameStateTextUI.text = "������ �ҷ����� ��...";

                // �� �� ĳ���� ������ �ε�

                globalTimer += Time.deltaTime;
                if (globalTimer >= loadingTime)
                {
                    globalTimer = 0;
                    gameState = GameState.CharacterSelect;
                }

            }
            else if (gameState == GameState.CharacterSelect)
            {
                gameStateTextUI.text = "Character Select";

                // ĳ���� ����

                globalTimer += Time.deltaTime;
                if (globalTimer >= characterSelectTime)
                {
                    globalTimer = 0;

                    // ������ ĳ���ͷ� ��ȯ �� �±� ����
                    // �� �÷��̾�� ���� ���̴� ����
                    MakeCharacter();
                    gameState = GameState.Ready;
                }

            }
            else if (gameState == GameState.Ready)
            {
                gameStateTextUI.text = "Ready";

                globalTimer += Time.deltaTime;
                if (globalTimer >= readyTime)
                {
                    gameState = GameState.Round;
                    gameStateText.SetActive(false);
                    ResetRound();
                }
            }
            else if (gameState == GameState.Round)
            {
                teamPayLoad[0].fillAmount = (int)occupyingA.rate;
                teamPayLoad[1].fillAmount = (int)occupyingB.rate;

                //occupyingGaugeTextUI.text = occupyingTeam.name + " : " + ((int)occupyingTeam.rate).ToString();

                teamExtraTime[1].GetComponent<Text>().text = string.Format("{0:F2}", roundEndTimer);
                //timerTextUI.text = ((int)roundEndTimer).ToString();

                gameStateTextUI.text = "Round";
                //������ ���ɵǾ������� ������ ���� ���ɺ����� ��������.
                if (currentOccupationTeam == teamA)
                {
                    occupyingA.rate += Time.deltaTime * occupyingRate;//�� 1.8�ʴ� 1�� ����
                    if (occupyingA.rate >= occupyingComplete)
                        occupyingA.rate = occupyingComplete;
                }
                else if (currentOccupationTeam == teamB)
                {
                    occupyingB.rate += Time.deltaTime * occupyingRate;
                    if (occupyingB.rate >= occupyingComplete)
                        occupyingB.rate = occupyingComplete;
                }

                OccupyAreaCounts();
                CheckRoundEnd();
            }
            else if (gameState == GameState.RoundEnd)
            {
                gameStateTextUI.text = "Round End";
                if (roundA >= 2 || roundB >= 2)
                {
                    gameState = GameState.MatchEnd;
                }
                else
                {
                    gameState = GameState.CharacterSelect;
                    ResetRound();
                }
            }
            else if (gameState == GameState.MatchEnd)
            {
                gameStateTextUI.text = "Match End";
                gameState = GameState.Result;
            }
            else if (gameState == GameState.Result)
            {
                gameStateTextUI.text = "Result";
                //����
            }
        }
    }

    void MakeCharacter()
    {
        for(int i = 0; i < allPlayers.Length; i++)
        {
            //ActorNumber�� custom property ����, ĳ���� viewID ������ �߰�
            //���� ĳ���ʹ� ���׸��� ����, ���� ������ ���� A,B �� ����

            if (i % 2 == 1)     // A �� (Red)
            {
                GameObject playerCharacter = PhotonNetwork.Instantiate("TaeWoo/Prefabs/Aeterna", spawnPoint[0].position, Quaternion.identity);
                playerCharacter.GetComponent<PhotonView>().TransferOwnership(allPlayers[i].ActorNumber);
                playerCharacter.GetComponent<PhotonView>().RPC("IsLocalPlayer", allPlayers[i]);
                playerCharacter.GetComponent<Character>().SetTagServer("TeamA");
                playersA.Add(playerCharacter);
            }

            else                // B �� (Blue)
            {
                GameObject playerCharacter = PhotonNetwork.Instantiate("TaeWoo/Prefabs/Aeterna", spawnPoint[1].position, Quaternion.identity);
                playerCharacter.GetComponent<PhotonView>().TransferOwnership(allPlayers[i].ActorNumber);
                playerCharacter.GetComponent<PhotonView>().RPC("IsLocalPlayer", allPlayers[i]);
                playerCharacter.GetComponent<Character>().SetTagServer("TeamB");
                playersB.Add(playerCharacter);
            }
        }

        for(int i = 0; i < playersA.Count;i++)
        {
            PhotonView photonView = PhotonView.Get(playersA[i]);
            photonView.gameObject.GetComponent<Character>().SetEnemyLayer();
        }

        for (int i = 0; i < playersB.Count; i++)
        {
            PhotonView photonView = PhotonView.Get(playersB[i]);
            photonView.gameObject.GetComponent<Character>().SetEnemyLayer();
        }


    }

    void CheckRoundEnd()
    {
        if (occupyingA.rate >= occupyingComplete && currentOccupationTeam == teamA && teamBOccupying <= 0)
        {
            roundEndTimer += Time.deltaTime;
        }
        else if (occupyingB.rate >= occupyingComplete && currentOccupationTeam == teamB && teamAOccupying <= 0)
        {
            roundEndTimer += Time.deltaTime;
        }
        else
            roundEndTimer = 0f;

        if (roundEndTimer >= roundEndTime)
        {
            //���� ����
            if (currentOccupationTeam == teamA)
            {
                if (roundA == 0) teamGetPoint[1].transform.GetChild(0).gameObject.SetActive(true);
                else if(roundA==1) teamGetPoint[1].transform.GetChild(1).gameObject.SetActive(true);
                roundA++;
            }
            else if (currentOccupationTeam == teamB)
            {
                if (roundB == 0) teamGetPoint[2].transform.GetChild(0).gameObject.SetActive(true);
                else if (roundB == 1) teamGetPoint[2].transform.GetChild(1).gameObject.SetActive(true);
                roundB++;
            }
            gameState = GameState.RoundEnd;//���� ����
        }
    }
    void OccupyAreaCounts()//���� ������ �÷��̾ �� �� �����ϰ� �ִ��� Ȯ��
    {
        teamAOccupying = 0;
        teamBOccupying = 0;

        for (int i = 0; i < playersA.Count; i++)
        {
            if (playersA[i].GetComponent<Character>().isOccupying == true)
            {
                teamAOccupying++;
            }
        }

        for (int i = 0; i < playersB.Count; i++)
        {
            if (playersB[i].GetComponent<Character>().isOccupying == true)
            {
                teamBOccupying++;
            }
        }

        if (teamAOccupying > 0 && teamBOccupying > 0)
        {
            //���� ���� ���̶�� ���� �˸�
            occupyingReturnTimer = 0f;
        }
        else if (teamAOccupying > 0)//A�� ����
        {
            ChangeOccupyingRate(teamAOccupying, teamA);
            occupyingReturnTimer = 0f;
        }
        else if (teamBOccupying > 0)//B�� ����
        {
            ChangeOccupyingRate(teamBOccupying, teamB);
            occupyingReturnTimer = 0f;
        }
        else
        {
            occupyingReturnTimer += Time.deltaTime;
        }

        if (occupyingReturnTimer >= occupyingReturnTime)
        {
            if (occupyingTeam.rate > 0f)
            {
                occupyingTeam.rate -= Time.deltaTime;
                if (occupyingTeam.rate < 0f)
                {
                    occupyingTeam.rate = 0f;
                    occupyingTeam.name = "";
                }
            }
        }

    }
    void ChangeOccupyingRate(int num, string name) //���� ������ ��ȭ
    {
        if (occupyingTeam.name == name)
        {
            if (currentOccupationTeam == name)
                return;
            occupyingTeam.rate += occupyingGaugeRate * Time.deltaTime * num;
            if (occupyingTeam.rate >= 100)
            {
                currentOccupationTeam = name;
                occupyingTeam.name = "";
                occupyingTeam.rate = 0f;
            }
        }
        else if (occupyingTeam.name == "")
        {
            if (currentOccupationTeam == name)
                return;
            occupyingTeam.name = name;
            occupyingTeam.rate += occupyingGaugeRate * Time.deltaTime * num;
        }
        else
        {
            occupyingTeam.rate -= occupyingGaugeRate * Time.deltaTime * num;
            if (occupyingTeam.rate < 0)
            {
                occupyingTeam.name = "";
                occupyingTeam.rate = 0;
            }
        }
    }

    void ResetRound()
    {
        currentOccupationTeam = "";
        occupyingA = new Occupation();
        occupyingB = new Occupation();
        occupyingTeam = new OccupyingTeam();
        occupyingReturnTimer = 0f;
        roundEndTimer = 0f;
        globalTimer = 0f;
        teamAOccupying = 0;
        teamBOccupying = 0;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(gameState);
            stream.SendNext(gameStateTextUI.text);
            stream.SendNext(timerTextUI.text);

        }
        else
        {
            gameState = (GameState)stream.ReceiveNext();
            gameStateTextUI.text = (string)stream.ReceiveNext();
            timerTextUI.text = (string)stream.ReceiveNext();
        }
    }
}
