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
    GameObject globalUI;
    GameObject playerSpawnPoints;

    // �÷��̾� ��ȯ ��ǥ
    Transform playerSpawnA;
    Transform playerSpawnB;

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

    public struct OccupyingTeam
    {
        public string name;
        public float rate;
    }

    public struct Occupation
    {
        public float rate;
    }

    public GameState gameState = GameState.WaitingAllPlayer;

    // �Ͻ����� ���� ���� string ������
    string gameStateString;

    // ���� ����
    int roundA = 0;
    int roundB = 0;

    // ���� ���� �����ϴ� ����
    int teamAOccupying = 0;
    int teamBOccupying = 0;

    // ��ü Ÿ�̸�
    float globalTimer;
    // ���� ��ȯ Ÿ�̸�
    float occupyingReturnTimer;
    // �߰��ð� Ÿ�̸�
    float roundEndTimer;
    // ���� �������� ��
    string currentOccupationTeam = "";
    // A���� ���ɵ�
    Occupation occupyingA;
    // B���� ���ɵ�
    Occupation occupyingB;
    // ���� ������ ��
    OccupyingTeam occupyingTeam;

    public Photon.Realtime.Player[] allPlayers;
    public List<GameObject> playersA = new List<GameObject>(); // Red
    public List<GameObject> playersB = new List<GameObject>(); // Blue

    // �� ���ϴ� �׽�Ʈ�� �Ͻ��� �����̴�.
    // Scriptable Object�� ������ ����

    // ��, ĳ���� �ε� Ÿ��
    float loadingTime = 3f;
    // ĳ���� ���� Ÿ��
    float characterSelectTime = 5f;
    // ���� �غ� �ð�
    float readyTime = 5f;
    // ���� ��ȯ �� �Դ� ����
    float occupyingGaugeRate = 100f;
    // ���� ��ȯ�ϴ� �ð�
    float occupyingReturnTime = 3f;
    // ���� % �Դ� ����
    float occupyingRate = 10f;
    // �߰��ð��� �߻��ϴ� ���� ������
    float occupyingComplete = 99f;
    //�߰� �ð�
    float roundEndTime = 5f;

    string teamA = "A";
    string teamB = "B";

    void Start()
    {
        
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            TodoMaster();

            if (globalUI != null)
                GiveDataToUI();
        }

    }

    void TodoMaster()
    {
        switch (gameState)
        {
            case GameState.WaitingAllPlayer:
                WaitingPlayers();
                break;
            case GameState.MatchStart:
                DataLoading();
                break;
            case GameState.CharacterSelect:
                CharacterSelect();
                break;
            case GameState.Ready:
                Ready();
                break;
            case GameState.Round:
                Round();
                break;
            case GameState.RoundEnd:
                RoundEnd();
                break;
            case GameState.MatchEnd:
                MatchEnd();
                break;
            case GameState.Result:
                Result();
                break;
            default:
                Debug.LogError("GameState Error");
                break;
        }
    }

    void WaitingPlayers()
    {
        gameStateString = "�ٸ� �÷��̾� ��ٸ��� ��...";

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
            globalUI = PhotonNetwork.Instantiate("TaeWoo/Prefabs/UI/GlobalUI", Vector3.zero, Quaternion.identity);
            playerSpawnPoints = PhotonNetwork.Instantiate("TaeWoo/Prefabs/PlayerSpawnPoints", Vector3.zero, Quaternion.identity);
            MakeSpawnPoint();
            gameState = GameState.MatchStart;
        }

    }
    void MakeSpawnPoint()
    {
        playerSpawnA = FindObject(playerSpawnPoints, "TeamA").transform;
        playerSpawnB = FindObject(playerSpawnPoints, "TeamB").transform;
    }

    void DataLoading()
    {
        gameStateString = "������ �ҷ����� ��...";

        // �� �� ĳ���� ������ �ε�

        globalTimer -= Time.deltaTime;
        if (globalTimer <= 0.0f)
        {
            gameState = GameState.CharacterSelect;
            globalTimer = characterSelectTime;
        }
    }

    void CharacterSelect()
    {
        gameStateString = "ĳ���� ����";

        // ĳ���� ����
        globalTimer -= Time.deltaTime;

        if (globalTimer <= 0.0f)
        {
            // ������ ĳ���ͷ� ��ȯ �� �±� ����
            // �� �÷��̾�� ���� ���̴� ����
            MakeCharacter();
            globalTimer = readyTime;
            gameState = GameState.Ready;
            globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "inGameUI", true);
        }
    }

    void Ready()
    {
        gameStateString = "Ready";

        globalTimer -= Time.deltaTime;
        if (globalTimer <= 0.0f)
        {
            gameState = GameState.Round;
            ResetRound();
            globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "etcUI", false);
        }
    }

    void Round()
    {
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

    void RoundEnd()
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

    void MatchEnd()
    {
        gameState = GameState.Result;
    }

    void Result()
    {
        // ����
    }

    void GiveDataToUI()
    {
        globalUI.GetComponent<GlobalUI>().gameStateString = gameStateString;
        globalUI.GetComponent<GlobalUI>().globalTimerUI = globalTimer;
        globalUI.GetComponent<GlobalUI>().roundEndTimerUI = roundEndTimer;
        globalUI.GetComponent<GlobalUI>().roundEndTimeUI = roundEndTime;
        globalUI.GetComponent<GlobalUI>().occupyingAUI.rate = occupyingA.rate;
        globalUI.GetComponent<GlobalUI>().occupyingBUI.rate = occupyingB.rate;
        globalUI.GetComponent<GlobalUI>().occupyingTeamUI.name = occupyingTeam.name;
        globalUI.GetComponent<GlobalUI>().occupyingTeamUI.rate = occupyingTeam.rate;
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
            globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "extraObj", true);
            globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "redExtraUI", false);
            globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "redExtraObj", true);
            roundEndTimer -= Time.deltaTime;

        }
        else if (occupyingB.rate >= occupyingComplete && currentOccupationTeam == teamB && teamAOccupying <= 0)
        {
            globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "extraObj", true);
            globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "blueExtraUI", false);
            globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "blueExtraObj", true);
            roundEndTimer -= Time.deltaTime;
        }
        else
            roundEndTimer = roundEndTime;

        if (roundEndTimer <= 0.0f)
        {
            //���� ����
            if (currentOccupationTeam == teamA)
            {
                if (roundA == 0)
                {
                    globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "redFirstPoint", true);
                }

                else if (roundA == 1)
                {
                    globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "redSecondPoint", true);
                }

                occupyingA.rate = 100;
                roundA++;
            }
            else if (currentOccupationTeam == teamB)
            {
                if (roundB == 0)
                {
                    globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "blueFirstPoint", true);
                }

                else if (roundB == 1)
                {
                    globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "blueSecondPoint", true);
                }

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

                if (currentOccupationTeam == "A")
                {
                    globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "captured_Red", true);
                    globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "captured_Blue", false);
                }

                else if (currentOccupationTeam == "B")
                {
                    globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "captured_Red", false);
                    globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "captured_Blue", true);
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
        roundEndTimer = 0;
        globalTimer = 0f;
        teamAOccupying = 0;
        teamBOccupying = 0;

        globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "captured_Red", false);
        globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "captured_Blue", false);
        globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "extraObj", false);
        globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "redExtraUI", true);
        globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "redExtraObj", false);
        globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "blueExtraUI", true);
        globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "blueExtraObj", false);
        globalUI.GetComponent<PhotonView>().RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "etcUI", true);

    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(gameState);
            stream.SendNext(gameStateString);
            stream.SendNext(roundA);
            stream.SendNext(roundB);
            stream.SendNext(teamAOccupying);
            stream.SendNext(teamBOccupying);
            stream.SendNext(globalTimer);
            stream.SendNext(occupyingReturnTimer);
            stream.SendNext(roundEndTimer);
            stream.SendNext(currentOccupationTeam);
            stream.SendNext(occupyingA.rate);
            stream.SendNext(occupyingB.rate);
            stream.SendNext(occupyingTeam.name);
            stream.SendNext(occupyingTeam.rate);
        }
        else
        {
            gameState = (GameState)stream.ReceiveNext();
            gameStateString = (string)stream.ReceiveNext();
            roundA = (int)stream.ReceiveNext();
            roundB = (int)stream.ReceiveNext();
            teamAOccupying = (int)stream.ReceiveNext();
            teamBOccupying = (int)stream.ReceiveNext();
            globalTimer = (float)stream.ReceiveNext();
            occupyingReturnTimer = (float)stream.ReceiveNext();
            roundEndTimer = (float)stream.ReceiveNext();
            currentOccupationTeam = (string)stream.ReceiveNext();
            occupyingA.rate = (float)stream.ReceiveNext();
            occupyingB.rate = (float)stream.ReceiveNext();
            occupyingTeam.name = (string)stream.ReceiveNext();
            occupyingTeam.rate = (float)stream.ReceiveNext();
        }
    }
}
