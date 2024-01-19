using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using FSM;

public class GameManagerStat
{
    public GameManagerStat(StateMachine stateMachine) 
    {
        LinKingGameStates(stateMachine);
        LinkingProperties(stateMachine);
    }

    void LinKingGameStates(StateMachine stateMachine)
    {
        GameStates[GameState.InTheLobby] = null;
        GameStates[GameState.InTheRoom] = new InTheRoom(stateMachine);
        GameStates[GameState.LoadingScene] = new LoadingScene(stateMachine);
        GameStates[GameState.CharacterSelect] = new CharacterSelect(stateMachine);
        GameStates[GameState.GameReady] = new GameReady(stateMachine);
        GameStates[GameState.DuringRound] = new DuringRound(stateMachine);
        GameStates[GameState.RoundEnd] = new RoundEnd(stateMachine);
        GameStates[GameState.GameResult] = new GameResult(stateMachine);
    }

    void LinkingProperties(StateMachine stateMachine)
    {
        playerSpawnA = Helper.FindObject(((GameManagerFSM)stateMachine).playerSpawnPoints, "TeamA").GetComponentsInChildren<Transform>(true);
        playerSpawnB = Helper.FindObject(((GameManagerFSM)stateMachine).playerSpawnPoints, "TeamB").GetComponentsInChildren<Transform>(true);

        gameManagerPhotonView = ((GameManagerFSM)stateMachine).GetComponent<PhotonView>();
        //gameCenter.characterSelect = gameCenter.characterSelectObj.GetComponent<SelectingCharacter>();
        characterSelectView = ((GameManagerFSM)stateMachine).characterSelectObj.GetComponent<PhotonView>();
        bgmManager = ((GameManagerFSM)stateMachine).bgmManagerObj.GetComponent<BGMManager>();
        bgmManagerView = ((GameManagerFSM)stateMachine).bgmManagerObj.GetComponent<PhotonView>();
        deathUI = ((GameManagerFSM)stateMachine).deathUIObj.GetComponent<DeathCamUI>();
        deathUIView = ((GameManagerFSM)stateMachine).deathUIObj.GetComponent<PhotonView>();
        playerStat = ((GameManagerFSM)stateMachine).playerStatObj.GetComponent<PlayerStats>();
        inGameUI = ((GameManagerFSM)stateMachine).inGameUIObj.GetComponent<InGameUI>();
        inGameUIView = ((GameManagerFSM)stateMachine).inGameUIObj.GetComponent<PhotonView>();
    }

    public enum GameState
    {
        InTheLobby,
        InTheRoom,
        LoadingScene,
        CharacterSelect,
        GameReady,
        DuringRound,
        RoundEnd,
        GameResult
    }

    public struct PlayerData
    {
        public int index;
        public string name;
        public Photon.Realtime.Player player;
        public string character;
        public int characterViewID;
        public string team;

        public int killCount;
        public int deadCount;
        public int assistCount;
        public int ultimateCount;

        public int totalDamage;
        public int totalHeal;
        public bool isAlive;

        public float angleStatueCoolTime;
        public string killerName;

        public float respawnTime;
        public Vector3 spawnPoint;

        public string damagePart;
        public Vector3 damageDirection;
        public float damageForce;

        public int playerAssistViewID;
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

    // ���� ���� ���� Dictionary
    [HideInInspector]
    public Dictionary<GameState, BaseState> GameStates 
        = new Dictionary<GameState, BaseState>();

    // ���� ����
    [HideInInspector]
    public int roundCount_A = 1;
    [HideInInspector]
    public int roundCount_B = 1;

    // ���� ���� �����ϴ� ����
    [HideInInspector]
    public int teamAOccupying = 0;
    [HideInInspector]
    public int teamBOccupying = 0;

    // ��ü Ÿ�̸�
    [HideInInspector]
    public float globalTimer = 0.0f;
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
    public List<PlayerData> playersA = new List<PlayerData>(); // Red
    [HideInInspector]
    public List<PlayerData> playersB = new List<PlayerData>(); // Blue 

    // �÷��̾� ��ȯ ��ǥ
    [HideInInspector]
    public Transform[] playerSpawnA;
    [HideInInspector]
    public Transform[] playerSpawnB;

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
    [Tooltip("���� ���� ����")]
    public int roundEndNumber = 2;

    // �� �̸�
    [HideInInspector]
    public string teamA = "A";
    [HideInInspector]
    public string teamB = "B";

    [HideInInspector]
    public PhotonView gameManagerPhotonView;
    [HideInInspector]
    public PhotonView characterSelectView;
    //[HideInInspector]
    //public SelectingCharacter characterSelect;
    [HideInInspector]
    public PhotonView inGameUIView;
    [HideInInspector]
    public InGameUI inGameUI;
    [HideInInspector]
    public BGMManager bgmManager;
    [HideInInspector]
    public PhotonView bgmManagerView;

    [HideInInspector]
    public GameObject betweenBGMObj;
    [HideInInspector]
    public AudioSource betweenBGMSource;

    [HideInInspector]
    public DeathCamUI deathUI;
    [HideInInspector]
    public PhotonView deathUIView;

    [HideInInspector]
    public PlayerStats playerStat;

}
