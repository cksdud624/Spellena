using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using Player;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameCenterTest : MonoBehaviourPunCallbacks, IPunObservable
{
    GameObject globalUIObj;
    PhotonView globalUIView;
    GlobalUI globalUI;

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

    int masterActorNum;
    public List<Photon.Realtime.Player> playersA = new List<Photon.Realtime.Player>(); // Red
    public List<Photon.Realtime.Player> playersB = new List<Photon.Realtime.Player>(); // Blue 

    // �� ���ϴ� �׽�Ʈ�� �Ͻ��� �����̴�.
    // Scriptable Object�� ������ ����

    // ��, ĳ���� �ε� Ÿ��
    float loadingTime = 0f;
    // ĳ���� ���� Ÿ��
    float characterSelectTime = 0f;
    // ���� �غ� �ð�
    float readyTime = 0f;
    // ���� ��ȯ �� �Դ� ����
    float occupyingGaugeRate = 250f;
    // ���� ��ȯ�ϴ� �ð�
    float occupyingReturnTime = 3f;
    // ���� % �Դ� ����
    float occupyingRate = 2f;
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

        int tempNum = 2;
        if (PhotonNetwork.CurrentRoom.PlayerCount >= tempNum)
        {
            globalTimer = loadingTime;

            SetPlayerDatas();

            globalUIObj = PhotonNetwork.Instantiate("TaeWoo/Prefabs/UI/GlobalUI", Vector3.zero, Quaternion.identity);
            globalUIView = globalUIObj.GetComponent<PhotonView>();
            globalUI = globalUIObj.GetComponent<GlobalUI>();

            playerSpawnPoints = PhotonNetwork.Instantiate("TaeWoo/Prefabs/PlayerSpawnPoints", Vector3.zero, Quaternion.identity);
            MakeSpawnPoint();

            gameState = GameState.MatchStart;
        }

    }

    void SetPlayerDatas()
    {
        masterActorNum = PhotonNetwork.CurrentRoom.MasterClientId;

        foreach(Photon.Realtime.Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            // �÷��̾� �̸�, ĳ������ ���� ������Ʈ, ��, �� ������ ��, ų ��, ���� ��
            Hashtable playerData = new Hashtable();

            playerData.Add("Name", player.ActorNumber);
            playerData.Add("CharacterViewID", 0);
            playerData.Add("Team", "none");
            playerData.Add("TotalDamage", 0);
            playerData.Add("Kills", 0);
            playerData.Add("Dead", 0);
            playerData.Add("Parameter", "none");

            player.SetCustomProperties(playerData);
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
            globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "inGameUI", true);
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
            globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "etcUI", false);
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

    public static Photon.Realtime.Player FindPlayerWithCustomProperty(string key, string value)
    {
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey(key) && player.CustomProperties[key].ToString() == value)
            {
                return player; 
            }
        }

        return null; 
    }

    public static void ChangePlayerCustomProperties(Photon.Realtime.Player player, string key, object value)
    {
        Hashtable temp = player.CustomProperties;

        if (temp[key] != null)
        {
            temp[key] = value;
            player.SetCustomProperties(temp);
        }

        else
        {
            Debug.LogError("�ش� �÷��̾��� Ű ���� ã�� �� �����ϴ�.");
            return;
        }
        
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        if (gameState == GameState.Round)
        {
            if (targetPlayer != null && changedProps != null)
            {
                //pararmeter�� ����� key���� ã�´�.

                string param = (string)targetPlayer.CustomProperties["Parameter"];

                switch (param)
                {
                    case "TotalDamage":
                        Debug.Log("Update Total Damage " + targetPlayer.CustomProperties["Name"]);
                        break;
                    case "Kills":
                        Debug.Log("Update Kills " + targetPlayer.CustomProperties["Name"]);
                        break;
                    case "Dead":
                        Debug.Log("Update Dead " + targetPlayer.CustomProperties["Name"]);
                        break;
                    default:
                        break;
                }

                targetPlayer.CustomProperties["Parameter"] = "none";

            }
        }
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
        globalUI.gameStateString = gameStateString;
        globalUI.globalTimerUI = globalTimer;
        globalUI.roundEndTimerUI = roundEndTimer;
        globalUI.roundEndTimeUI = roundEndTime;
        globalUI.occupyingAUI.rate = occupyingA.rate;
        globalUI.occupyingBUI.rate = occupyingB.rate;
        globalUI.occupyingTeamUI.name = occupyingTeam.name;
        globalUI.occupyingTeamUI.rate = occupyingTeam.rate;
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
        foreach(var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            //���� ĳ���ʹ� ���׸��� ����, ���� ������ ���� A,B �� ����
            //if((string)player.CustomProperties["Team"]=="A")
            string choseCharacter = "Aeterna";

            if (player.ActorNumber % 2 == 0)     // A �� (Red)
            {
                GameObject playerCharacter 
                    = PhotonNetwork.Instantiate("TaeWoo/Prefabs/" + choseCharacter, playerSpawnA.position, Quaternion.identity);
                playerCharacter.GetComponent<PhotonView>().TransferOwnership(player.ActorNumber);
                playerCharacter.GetComponent<PhotonView>().RPC("IsLocalPlayer", player);
                playerCharacter.GetComponent<Character>().SetTagServer("TeamA");
                ChangePlayerCustomProperties(player, "CharacterViewID", playerCharacter.GetComponent<PhotonView>().ViewID);
                playersA.Add(player);
            }

            else                // B �� (Blue)
            {
                GameObject playerCharacter 
                    = PhotonNetwork.Instantiate("TaeWoo/Prefabs/" + choseCharacter, playerSpawnB.position, Quaternion.identity);
                playerCharacter.GetComponent<PhotonView>().TransferOwnership(player.ActorNumber);
                playerCharacter.GetComponent<PhotonView>().RPC("IsLocalPlayer", player);
                playerCharacter.GetComponent<Character>().SetTagServer("TeamB");
                ChangePlayerCustomProperties(player, "CharacterViewID", playerCharacter.GetComponent<PhotonView>().ViewID);
                playersB.Add(player);
            }
        }

        ////�� ���̴� ����
        //for(int i = 0; i < playersA.Count;i++)
        //{
        //    if (PhotonNetwork.LocalPlayer.IsLocal)
        //    {
        //        GameObject temp = (GameObject)playersA[i].CustomProperties["CharacterGameObject"];
        //        PhotonView photonView = temp.GetComponent<PhotonView>();
        //        photonView.gameObject.GetComponent<Character>().SetEnemyLayer();
        //    }
        //}

        //for (int i = 0; i < playersB.Count; i++)
        //{
        //    if (PhotonNetwork.LocalPlayer.IsLocal)
        //    {
        //        GameObject temp = (GameObject)playersB[i].CustomProperties["CharacterGameObject"];
        //        PhotonView photonView = temp.GetComponent<PhotonView>();
        //        photonView.gameObject.GetComponent<Character>().SetEnemyLayer();
        //    }
        //}
    }

    void CheckRoundEnd()
    {
        if (occupyingA.rate >= occupyingComplete && currentOccupationTeam == teamA && teamBOccupying <= 0)
        {
            globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "extraObj", true);
            globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "redExtraUI", false);
            globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "redExtraObj", true);
            roundEndTimer -= Time.deltaTime;

        }
        else if (occupyingB.rate >= occupyingComplete && currentOccupationTeam == teamB && teamAOccupying <= 0)
        {
            globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "extraObj", true);
            globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "blueExtraUI", false);
            globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "blueExtraObj", true);
            roundEndTimer -= Time.deltaTime;
        }
        else
        {
            roundEndTimer = roundEndTime;
        }

        if (roundEndTimer <= 0.0f)
        {
            //���� ����
            if (currentOccupationTeam == teamA)
            {
                if (roundA == 0)
                {
                    globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "redFirstPoint", true);
                }

                else if (roundA == 1)
                {
                    globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "redSecondPoint", true);
                }

                occupyingA.rate = 100;
                roundA++;
            }
            else if (currentOccupationTeam == teamB)
            {
                if (roundB == 0)
                {
                    globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "blueFirstPoint", true);
                }

                else if (roundB == 1)
                {
                    globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "blueSecondPoint", true);
                }

                occupyingB.rate = 100;
                roundB++;
            }

            gameState = GameState.RoundEnd;//���� ����
        }
    }

    GameObject FindObjectWithViewID(int viewID)
    {
        PhotonView photonView = PhotonView.Find(viewID);

        if (photonView == null)
        {
            Debug.LogError("�ش� " + viewID + "�� ���� ������Ʈ�� ã�� �� �����ϴ�.");
            return null;
        }

        else
        {
            return photonView.gameObject;
        }
    }

    void OccupyAreaCounts()//���� ������ �÷��̾ �� �� �����ϰ� �ִ��� Ȯ��
    {
        teamAOccupying = 0;
        teamBOccupying = 0;

        GameObject temp;

        for (int i = 0; i < playersA.Count; i++)
        {
            temp = FindObjectWithViewID((int)playersA[i].CustomProperties["CharacterViewID"]);

            if (temp.GetComponent<Character>().isOccupying == true)
            {
                teamAOccupying++;
            }
        }

        for (int i = 0; i < playersB.Count; i++)
        {
            temp = FindObjectWithViewID((int)playersB[i].CustomProperties["CharacterViewID"]);

            if (temp.GetComponent<Character>().isOccupying == true)
            {
                teamBOccupying++;
            }
        }

        if (teamAOccupying > 0 && teamBOccupying > 0)
        {
            //���� ���� ���̶�� ���� �˸�
            occupyingReturnTimer = 0f;
            //globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "fighting", true);
        }
        else if (teamAOccupying > 0)//A�� ����
        {
            ChangeOccupyingRate(teamAOccupying, teamA);
            occupyingReturnTimer = 0f;
            //globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "fighting", false);
        }
        else if (teamBOccupying > 0)//B�� ����
        {
            ChangeOccupyingRate(teamBOccupying, teamB);
            occupyingReturnTimer = 0f;
            //globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "fighting", false);
        }
        else
        {
            occupyingReturnTimer += Time.deltaTime;
            //globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "fighting", false);
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
            occupyingTeam.rate += occupyingGaugeRate * Time.deltaTime;
            if (occupyingTeam.rate >= 100)
            {
                currentOccupationTeam = name;
                occupyingTeam.name = "";
                occupyingTeam.rate = 0f;

                if (currentOccupationTeam == "A")
                {
                    globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "captured_Red", true);
                    globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "captured_Blue", false);
                    globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "extraObj", false);
                    globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "blueExtraObj", false);
                    globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "blueExtraUI", true);
                }

                else if (currentOccupationTeam == "B")
                {
                    globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "captured_Red", false);
                    globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "captured_Blue", true);
                    globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "extraObj", false);
                    globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "redExtraObj", false);
                    globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "redExtraUI", true);
                }
            }
        }
        else if (occupyingTeam.name == "")
        {
            if (currentOccupationTeam == name)
                return;
            occupyingTeam.name = name;
            occupyingTeam.rate += occupyingGaugeRate * Time.deltaTime;
        }
        else
        {
            occupyingTeam.rate -= occupyingGaugeRate * Time.deltaTime;
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

        globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "captured_Red", false);
        globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "captured_Blue", false);
        globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "extraObj", false);
        globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "redExtraUI", true);
        globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "redExtraObj", false);
        globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "blueExtraUI", true);
        globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "blueExtraObj", false);
        globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "etcUI", true);

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
