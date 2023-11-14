using Player;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameCenterTest : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    public GameObject globalUIObj;
    [HideInInspector]
    public PhotonView globalUIView;
    [HideInInspector]
    public GlobalUI globalUI;

    [HideInInspector]
    public GameObject playerSpawnPoints;

    // �÷��̾� ��ȯ ��ǥ
    [HideInInspector]
    public Transform[] playerSpawnA;
    [HideInInspector]
    public Transform[] playerSpawnB;

    public enum GameState
    {
        WaitingAllPlayer,
        DataLoading,
        CharacterSelect,
        GameReady,
        DuringRound,
        RoundEnd,
        MatchEnd,
        GameResult
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

    [HideInInspector]
    public Dictionary<GameState, CenterState> centerStates = new Dictionary<GameState, CenterState>();

    [HideInInspector]
    public GameState currentGameState;
    [HideInInspector]
    public CenterState currentCenterState;

    // �Ͻ����� ���� ���� string ������
    [HideInInspector]
    public string gameStateString;

    // �Ͻ����� ���� �÷��̾�
    [HideInInspector]
    public string tempVictim = "";

    // ���� ����
    [HideInInspector]
    public int roundA = 0;
    [HideInInspector]
    public int roundB = 0;

    // ���� ���� �����ϴ� ����
    public int teamAOccupying = 0;
    public int teamBOccupying = 0;

    // ��ü Ÿ�̸�
    [HideInInspector]
    public float globalTimer;
    // ���� ��ȯ Ÿ�̸�
    [HideInInspector]
    public float occupyingReturnTimer;
    // �߰��ð� Ÿ�̸�
    [HideInInspector]
    public float roundEndTimer;
    // ���� �������� ��
    [HideInInspector]
    public string currentOccupationTeam = "";
    // A���� ���ɵ�
    [HideInInspector]
    public Occupation occupyingA;
    // B���� ���ɵ�
    [HideInInspector]
    public Occupation occupyingB;
    // ���� ������ ��
    [HideInInspector]
    public OccupyingTeam occupyingTeam;

    [HideInInspector]
    public List<Photon.Realtime.Player> playersA = new List<Photon.Realtime.Player>(); // Red
    [HideInInspector]
    public List<Photon.Realtime.Player> playersB = new List<Photon.Realtime.Player>(); // Blue 

    // �� ���ϴ� �׽�Ʈ�� �Ͻ��� �����̴�.
    // Scriptable Object�� ������ ����

    // ��, ĳ���� �ε� Ÿ��
    [HideInInspector]
    public float loadingTime = 1f;
    // ĳ���� ���� Ÿ��
    [HideInInspector]
    public float characterSelectTime = 1f;
    // ���� �غ� �ð�
    [HideInInspector]
    public float readyTime = 1f;
    // �÷��̾� ������ Ÿ��
    [HideInInspector]
    public float playerRespawnTime = 6;
    // ���� ��ȯ �� �Դ� ����
    [HideInInspector]
    public float occupyingGaugeRate = 300f;
    // ���� ��ȯ�ϴ� �ð�
    [HideInInspector]
    public float occupyingReturnTime = 3f;
    // ���� % �Դ� ����
    [HideInInspector]
    public float occupyingRate = 2f;
    // �߰��ð��� �߻��ϴ� ���� ������
    [HideInInspector]
    public float occupyingComplete = 99f;
    //�߰� �ð�
    [HideInInspector]
    public float roundEndTime = 5f;
    // ���� ��� Ȯ�� �ð�
    [HideInInspector]
    public float roundEndResultTime = 6f;

    // �� �̸�
    [HideInInspector]
    public string teamA = "A";
    [HideInInspector]
    public string teamB = "B";

    private void Awake()
    {
        WaitingPlayers temp = gameObject.AddComponent<WaitingPlayers>();
        temp.ConnectCenter(this);
        centerStates.Add(GameState.WaitingAllPlayer, temp);

        DataLoading temp1 = gameObject.AddComponent<DataLoading>();
        temp1.ConnectCenter(this);
        centerStates.Add(GameState.DataLoading, temp1);

        CharacterSelect temp2 = gameObject.AddComponent<CharacterSelect>();
        temp2.ConnectCenter(this);
        centerStates.Add(GameState.CharacterSelect, temp2);

        GameReady temp3 = gameObject.AddComponent<GameReady>();
        temp3.ConnectCenter(this);
        centerStates.Add(GameState.GameReady, temp3);

        DuringRound temp4 = gameObject.AddComponent<DuringRound>();
        temp4.ConnectCenter(this);
        centerStates.Add(GameState.DuringRound, temp4);

        RoundEnd temp5 = gameObject.AddComponent<RoundEnd>();
        temp5.ConnectCenter(this);
        centerStates.Add(GameState.RoundEnd, temp5);

        MatchEnd temp6 = gameObject.AddComponent<MatchEnd>();
        temp6.ConnectCenter(this);
        centerStates.Add(GameState.MatchEnd, temp6);

        GameResult temp7 = gameObject.AddComponent<GameResult>();
        temp7.ConnectCenter(this);
        centerStates.Add(GameState.GameResult, temp7);

        currentGameState = GameState.WaitingAllPlayer;
        currentCenterState = centerStates[currentGameState];
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentCenterState.StateExecution();
            currentCenterState = centerStates[currentGameState];

            if (globalUI != null)
                GiveDataToUI();
        }

    }

    public static Photon.Realtime.Player FindPlayerWithCustomProperty(string key, string value)
    {
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (player.CustomProperties.ContainsKey(key) && player.CustomProperties[key].ToString() == value)
            {
                return player;
            }
        }

        return null;
    }

    // �� �Լ��� ����ؾ� Ŭ���̾�Ʈ�� ��� ������ ����ȭ �ȴ�. / �׳� ������ ����ȭ �ȵ�
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

    public void ShowTeamMateDead(string team, string deadName)
    {
        if(team=="A")
        {
            foreach(var player in playersA)
            {
                globalUIView.RPC("ShowTeamLifeDead", player, deadName,true);
            }
        }

        else if(team=="B")
        {
            foreach (var player in playersB)
            {
                globalUIView.RPC("ShowTeamLifeDead", player, deadName,true);
            }
        }
    }

    public GameObject FindObject(GameObject parrent ,string name)
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

    public GameObject FindObjectWithViewID(int viewID)
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

    
    public void ChangeOccupyingRate(int num, string name) //���� ������ ��ȭ
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

    [PunRPC]
    public void TimeScaling(float ratio)
    {
        Time.timeScale = ratio;
    }

    public void GiveDataToUI()
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentGameState);
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
            currentGameState = (GameState)stream.ReceiveNext();
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

    //�ΰ��� ������ Ȯ��
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "FPS: " + (1.0f / Time.smoothDeltaTime));
    }
}
