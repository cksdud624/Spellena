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
    [HideInInspector]
    public GameObject playerSpawnPoints;
    [HideInInspector]
    public GameObject inGameUI;
    [HideInInspector]
    public GameObject etcUI;

    // �÷��̾� ��ȯ ��ǥ
    Transform playerSpawnA;
    Transform playerSpawnB;

    // inGameUI ���
    GameObject unContested;
    GameObject captured_Red;
    GameObject captured_Blue;
    Image redFillCircle;
    Image blueFillCircle;
    Image redPayload;
    Image bluePayload;
    GameObject redExtraUI;
    GameObject blueExtraUI;
    Text redPercentage;
    Text bluePercentage;
    GameObject extraObj;
    Text extraTimer;
    GameObject redExtraObj;
    GameObject blueExtraObj;
    Image redCTF;
    Image blueCTF;
    GameObject redFirstPoint;
    GameObject redSecondPoint;
    GameObject blueFirstPoint;
    GameObject blueSecondPoint;

    //EtcUI
    Text gameStateUI;
    Text timer;

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

    // �� ���ϴ� �׽�Ʈ�� �Ͻ��� �����̴�.

    float loadingTime = 3f;
    float characterSelectTime = 5f; //���߿� ������ �Ŵ������� ���� �����������Ѵ�.
    float readyTime = 5f;

    float occupyingGaugeRate = 100f;
    float occupyingReturnTime = 3f;
    float occupyingRate = 10f;

    // �߰��ð��� �߻��ϴ� ���� ������
    float occupyingComplete = 99f;

    //�߰� �ð�
    float roundEndTime = 5f;

    string teamA = "A";
    string teamB = "B";

    public Photon.Realtime.Player[] allPlayers;
    public List<GameObject> playersA = new List<GameObject>(); // Red
    public List<GameObject> playersB = new List<GameObject>(); // Blue

    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
            Init();
    }

    void Init()
    {
        gameState = GameState.WaitingAllPlayer;

        playerSpawnPoints = PhotonNetwork.Instantiate("TaeWoo/Prefabs/PlayerSpawnPoints", new Vector3(0,0,0), Quaternion.identity);
        inGameUI = PhotonNetwork.Instantiate("TaeWoo/Prefabs/InGameUI", new Vector3(0,0,0), Quaternion.identity);
        etcUI = PhotonNetwork.Instantiate("TaeWoo/Prefabs/EtcUI", new Vector3(0,0,0), Quaternion.identity);

        playerSpawnA = FindObject(playerSpawnPoints, "TeamA").transform;
        playerSpawnB = FindObject(playerSpawnPoints, "TeamB").transform;

        unContested = FindObject(inGameUI, "UnContested");
        captured_Red = FindObject(inGameUI, "RedCapture");
        captured_Blue = FindObject(inGameUI, "BlueCapture");
        redFillCircle = FindObject(inGameUI, "RedOutline").GetComponent<Image>();
        blueFillCircle = FindObject(inGameUI, "BlueOutline").GetComponent<Image>();
        redPayload = FindObject(inGameUI, "RedPayload_Filled").GetComponent<Image>();
        bluePayload = FindObject(inGameUI, "BluePayload_Filled").GetComponent<Image>();
        redExtraUI = FindObject(inGameUI, "RedCTF");
        blueExtraUI = FindObject(inGameUI, "BlueCTF");
        redPercentage = FindObject(inGameUI, "RedOccupyingPercent").GetComponent<Text>();
        bluePercentage = FindObject(inGameUI, "BlueOccupyingPercent").GetComponent<Text>();
        extraObj = FindObject(inGameUI, "Extra");
        extraTimer = FindObject(inGameUI, "ExtaTimer").GetComponent<Text>();
        redExtraObj = FindObject(inGameUI, "Red");
        blueExtraObj = FindObject(inGameUI, "Blue");
        redCTF = FindObject(inGameUI, "RedCTF_Filled").GetComponent<Image>();
        blueCTF = FindObject(inGameUI, "BlueCTF_Filled").GetComponent<Image>();
        redFirstPoint = FindObject(inGameUI, "RedFirstPoint");
        redSecondPoint = FindObject(inGameUI, "RedSecondPoint");
        blueFirstPoint = FindObject(inGameUI, "BlueFirstPoint");
        blueSecondPoint = FindObject(inGameUI, "BlueSecondPoint");

        gameStateUI = FindObject(etcUI, "GameState").GetComponent<Text>();
        timer = FindObject(etcUI, "Timer").GetComponent<Text>();

        inGameUI.SetActive(false);
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            timer.text = ((int)globalTimer + 1).ToString();

            if (gameState == GameState.WaitingAllPlayer)
            {
                gameStateUI.text = "�ٸ� �÷��̾� ��ٸ��� ��...";

                // ������ ���� ��ư ������ ����
                // �÷��̾�� �� ���� ����
                // ActorNumber�� �÷��̾� �ĺ�
                // allPlayers = PhotonNetwork.PlayerList;
                // custom property�� �� ���� (ActorNumber, name, team)


                int tempNum = 1;
                if (PhotonNetwork.CurrentRoom.PlayerCount >= tempNum)
                {
                    globalTimer = loadingTime;
                    allPlayers = PhotonNetwork.PlayerList;
                    gameState = GameState.MatchStart;
                }

            }

            else if (gameState == GameState.MatchStart)
            {
                gameStateUI.text = "������ �ҷ����� ��...";

                // �� �� ĳ���� ������ �ε�
               
                globalTimer -= Time.deltaTime;
                if (globalTimer <= 0.0f)
                {
                    gameState = GameState.CharacterSelect;
                    globalTimer = characterSelectTime;
                }

            }
            else if (gameState == GameState.CharacterSelect)
            {
                gameStateUI.text = "Character Select";

                // ĳ���� ����

                globalTimer -= Time.deltaTime;
                if (globalTimer <= 0.0f)
                {
                    // ������ ĳ���ͷ� ��ȯ �� �±� ����
                    // �� �÷��̾�� ���� ���̴� ����
                    MakeCharacter();
                    globalTimer = readyTime;
                    gameState = GameState.Ready;
                    inGameUI.SetActive(true);
                }

            }
            else if (gameState == GameState.Ready)
            {
                gameStateUI.text = "Ready";

                globalTimer -= Time.deltaTime;
                if (globalTimer <= 0.0f)
                {
                    gameState = GameState.Round;
                    ResetRound();
                    etcUI.SetActive(false);
                }
            }
            else if (gameState == GameState.Round)
            {
                redPayload.fillAmount = occupyingA.rate * 0.01f;
                bluePayload.fillAmount = occupyingB.rate * 0.01f;
                redPercentage.text = string.Format((int)occupyingA.rate + "%");
                bluePercentage.text = string.Format((int)occupyingB.rate + "%");
                extraTimer.text = string.Format("{0:F2}", roundEndTimer);

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
                if (roundA >= 2 || roundB >= 2)
                {
                    gameState = GameState.MatchEnd;
                }
                else
                {
                    globalTimer = readyTime;
                    gameState = GameState.Ready;
                    ResetRound();
                }
            }
            else if (gameState == GameState.MatchEnd)
            {
                gameState = GameState.Result;
            }
            else if (gameState == GameState.Result)
            {
                //����
            }
        }
    }

    void WaitingPlayers()
    {

    }

    void DataLoading()
    {

    }

    void CharacterSelect()
    {

    }



    GameObject FindObject(GameObject parrent ,string name)
    {
        GameObject foundObject = null;
        Transform[] array = parrent.GetComponentsInChildren<Transform>(true);

        foreach (Transform transform in array)
        {
            if (transform.name == name)
            {
                foundObject = transform.gameObject;
                break; // ã������ ������ ����.
            }
        }

        if(foundObject == null)
        {
            Debug.LogError("�ش� �̸��� ���� ������Ʈ�� ã�� ���߽��ϴ� : " + name);
        }

        return foundObject;
    }

    void MakeCharacter()
    {
        for(int i = 0; i < allPlayers.Length; i++)
        {
            //ActorNumber�� custom property ����, ĳ���� viewID ������ �߰�
            //���� ĳ���ʹ� ���׸��� ����, ���� ������ ���� A,B �� ����

            if (i % 2 == 1)     // A �� (Red)
            {
                GameObject playerCharacter = PhotonNetwork.Instantiate("TaeWoo/Prefabs/Aeterna", playerSpawnA.position, Quaternion.identity);
                playerCharacter.GetComponent<PhotonView>().TransferOwnership(allPlayers[i].ActorNumber);
                playerCharacter.GetComponent<PhotonView>().RPC("IsLocalPlayer", allPlayers[i]);
                playerCharacter.GetComponent<Character>().SetTagServer("TeamA");
                playersA.Add(playerCharacter);
            }

            else                // B �� (Blue)
            {
                GameObject playerCharacter = PhotonNetwork.Instantiate("TaeWoo/Prefabs/Aeterna", playerSpawnB.position, Quaternion.identity);
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
            extraObj.SetActive(true);
            redExtraUI.SetActive(false);
            redExtraObj.SetActive(true);
            redCTF.fillAmount = roundEndTimer / roundEndTime;
            roundEndTimer -= Time.deltaTime;

        }
        else if (occupyingB.rate >= occupyingComplete && currentOccupationTeam == teamB && teamAOccupying <= 0)
        {
            extraObj.SetActive(true);
            blueExtraUI.SetActive(false);
            blueExtraObj.SetActive(true);
            blueCTF.fillAmount = roundEndTimer / roundEndTime;
            roundEndTimer -= Time.deltaTime;
        }
        else
            roundEndTimer = roundEndTime;

        if (roundEndTimer <= 0.0f)
        {
            //���� ����
            if (currentOccupationTeam == teamA)
            {
                if (roundA == 0) redFirstPoint.SetActive(true);
                else if(roundA==1) redSecondPoint.SetActive(true);
                occupyingA.rate = 100;
                roundA++;
            }
            else if (currentOccupationTeam == teamB)
            {
                if (roundB == 0) blueFirstPoint.SetActive(true);
                else if (roundB == 1) blueSecondPoint.SetActive(true);
                occupyingB.rate = 100;
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
            Debug.Log("������..");
        }
        else if (teamAOccupying > 0)//A�� ����
        {
            ChangeOccupyingRate(teamAOccupying, teamA);
            redFillCircle.fillAmount = occupyingTeam.rate * 0.01f;
            occupyingReturnTimer = 0f;
        }
        else if (teamBOccupying > 0)//B�� ����
        {
            ChangeOccupyingRate(teamBOccupying, teamB);
            blueFillCircle.fillAmount = occupyingTeam.rate * 0.01f;
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

                if (currentOccupationTeam == "A")
                {
                    captured_Red.SetActive(true);
                    captured_Blue.SetActive(false);
                }

                else if (currentOccupationTeam == "B")
                {
                    captured_Red.SetActive(false);
                    captured_Blue.SetActive(true);
                }
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
        roundEndTimer = roundEndTime;
        globalTimer = 0f;
        teamAOccupying = 0;
        teamBOccupying = 0;

        captured_Red.SetActive(false);
        captured_Blue.SetActive(false);

        extraObj.SetActive(false);
        redExtraUI.SetActive(true);
        redExtraObj.SetActive(false);
        blueExtraUI.SetActive(true);
        blueExtraObj.SetActive(false);

        redPayload.fillAmount = 0.0f;
        bluePayload.fillAmount = 0.0f;

        etcUI.SetActive(true);
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(gameState);

        }
        else
        {
            gameState = (GameState)stream.ReceiveNext();

        }
    }
}
