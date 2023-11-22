using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameCenterTest : MonoBehaviourPunCallbacks
{
    public GameObject angleStatue;
    public GameObject playerSpawnPoints;
    public GameObject characterSelect;
    public GameObject inGameUIObj;

    [HideInInspector]
    public PhotonView inGameUIView;
    [HideInInspector]
    public InGameUI inGameUI;

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

    //// �Ͻ����� ���� ���� string ������
    //[HideInInspector]
    //public string gameStateString;

    // �Ͻ����� ���� �÷��̾�
    [HideInInspector]
    public string tempVictim = "";

    // ���� ����
    [HideInInspector]
    public int roundA = 0;
    [HideInInspector]
    public int roundB = 0;

    // ���� ���� �����ϴ� ����
    [HideInInspector]
    public int teamAOccupying = 0;
    [HideInInspector]
    public int teamBOccupying = 0;

    // ��ü Ÿ�̸�
    [HideInInspector]
    public float globalTimer;
    // ��ǥ ��ü Ÿ�̸� ��
    [HideInInspector]
    public float globalDesiredTimer;
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

    [Tooltip("��, ĳ���� �ε� Ÿ��")]
    public float loadingTime = 1f;
    [Tooltip("ĳ���� ���� Ÿ��")]
    public float characterSelectTime = 1f;
    [Tooltip("���� �غ� �ð�")]
    public float readyTime = 1f;
    [Tooltip("�÷��̾� ������ Ÿ��")]
    public float playerRespawnTime = 6;
    [Tooltip("��ý�Ʈ Ÿ��")]
    public float assistTime = 10;

    [Tooltip("�κ��� ��Ÿ��")]
    public float angelStatueCoolTime = 30.0f;
    [Tooltip("�κ��� �ʴ� ü�� ������")]
    public int angelStatueHpPerTime = 10;
    [Tooltip("�κ��� ȿ�� ���� �ð�")]
    public int angelStatueContinueTime = 10;

    [Tooltip("���� ��ȯ �� �Դ� ����")]
    public float occupyingGaugeRate = 40f;
    [Tooltip("���� ��ȯ�ϴ� �ð�")]
    public float occupyingReturnTime = 3f;
    [Tooltip("���� % �Դ� ����")]
    public float occupyingRate = 10f;
    [Tooltip("�߰��ð��� �߻��ϴ� ���� ������")]
    public float occupyingComplete = 99f;
    [Tooltip("�߰� �ð�")]
    public float roundEndTime = 5f;
    [Tooltip("���� ��� Ȯ�� �ð�")]
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
            Debug.Log(currentGameState);
            currentCenterState.StateExecution();
            currentCenterState = centerStates[currentGameState];

            photonView.RPC("SerializeGameCenterDatas", RpcTarget.AllBufferedViaServer, ToDoSerlize());

            if (inGameUI != null)
            {
                GiveDataToUI();
            }
        }

    }

    object[] ToDoSerlize()
    {
        object[] datas = new object[14];

        datas[0] = currentGameState;
        datas[2] = roundA;
        datas[3] = roundB;
        datas[4] = teamAOccupying;
        datas[5] = teamBOccupying;
        datas[6] = globalTimer;
        datas[7] = occupyingReturnTimer;
        datas[8] = roundEndTimer;
        datas[9] = currentOccupationTeam;
        datas[10] = occupyingA.rate;
        datas[11] = occupyingB.rate;
        datas[12] = occupyingTeam.name;
        datas[13] = occupyingTeam.rate;

        return datas;
    }

    [PunRPC]
    public void SerializeGameCenterDatas(object[] datas)
    {
        currentGameState = (GameState)datas[0];
        roundA = (int)datas[2];
        roundB = (int)datas[3];
        teamAOccupying = (int)datas[4];
        teamBOccupying = (int)datas[5];
        globalTimer = (float)datas[6];
        occupyingReturnTimer = (float)datas[7];
        roundEndTimer = (float)datas[8];
        currentOccupationTeam = (string)datas[9];
        occupyingA.rate = (float)datas[10];
        occupyingB.rate = (float)datas[11];
        occupyingTeam.name = (string)datas[12];
        occupyingTeam.rate = (float)datas[13];
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

        if (temp[key] == null)
        {
            temp.Add(key, value);
        }

        else
        {
            temp[key] = value;
        }

        player.SetCustomProperties(temp);
    }

    public void ShowTeamMateDead(string team, string deadName)
    {
        if(team=="A")
        {
            foreach(var player in playersA)
            {
                inGameUIView.RPC("ShowTeamLifeDead", player, deadName,true);
            }
        }

        else if(team=="B")
        {
            foreach (var player in playersB)
            {
                inGameUIView.RPC("ShowTeamLifeDead", player, deadName,true);
            }
        }
    }

    public static GameObject FindObject(GameObject parrent ,string name)
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
                    inGameUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "captured_Red", true);
                    inGameUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "captured_Blue", false);
                    inGameUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "extraObj", false);
                    inGameUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "blueExtraObj", false);
                    inGameUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "blueExtraUI", true);
                    angleStatue.GetComponent<PhotonView>().RPC("ChangeTeam", RpcTarget.AllBufferedViaServer, "A");
                }

                else if (currentOccupationTeam == "B")
                {
                    inGameUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "captured_Red", false);
                    inGameUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "captured_Blue", true);
                    inGameUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "extraObj", false);
                    inGameUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "redExtraObj", false);
                    inGameUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "redExtraUI", true);
                    angleStatue.GetComponent<PhotonView>().RPC("ChangeTeam", RpcTarget.AllBufferedViaServer, "B");
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

    public void GiveDataToUI()
    {
        if (inGameUI == null) return;
        inGameUI.globalTimerUI = globalDesiredTimer - globalTimer;
        inGameUI.roundEndTimerUI = roundEndTimer;
        inGameUI.roundEndTimeUI = roundEndTime;
        inGameUI.occupyingAUI.rate = occupyingA.rate;
        inGameUI.occupyingBUI.rate = occupyingB.rate;
        inGameUI.occupyingTeamUI.name = occupyingTeam.name;
        inGameUI.occupyingTeamUI.rate = occupyingTeam.rate;
    }

    //�ΰ��� ������ Ȯ��
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "FPS: " + (1.0f / Time.smoothDeltaTime));
    }
}
