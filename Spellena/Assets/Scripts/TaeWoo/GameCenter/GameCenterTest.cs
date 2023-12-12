using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameCenterTest : MonoBehaviourPunCallbacks
{
    public GameObject angleStatue;
    public GameObject playerSpawnPoints;
    public GameObject bgmManagerObj;

    public GameObject characterSelectObj;
    public GameObject inGameUIObj;
    public GameObject deathUIObj;
    public GameObject gameResultObj;
    public GameObject playerStatObj;

    public GameCenterTestData gameCenterTestData;

    [HideInInspector]
    public PhotonView characterSelectView;
    [HideInInspector]
    public SelectingCharacter characterSelect;
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

    // �÷��̾� ��ȯ ��ǥ
    [HideInInspector]
    public Transform[] playerSpawnA;
    [HideInInspector]
    public Transform[] playerSpawnB;

    public enum GameState
    {
        CharacterSelect,
        GameReady,
        DuringRound,
        RoundEnd,
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

    // �Ͻ����� ���� �÷��̾�
    [HideInInspector]
    public string tempVictim = "";

    // ���� ����
    [HideInInspector]
    public static int roundA = 0;
    [HideInInspector]
    public static int roundB = 0;

    // ���� ���� �����ϴ� ����
    [HideInInspector]
    public int teamAOccupying = 0;
    [HideInInspector]
    public int teamBOccupying = 0;

    // ��ü Ÿ�̸�
    [HideInInspector]
    public static float globalTimer = 0.0f;
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

    //[HideInInspector]
    public List<Photon.Realtime.Player> playersA = new List<Photon.Realtime.Player>(); // Red
    //[HideInInspector]
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

    void Start()
    {
        InitManager();
    }

    void InitManager()
    {
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

        GameResult temp7 = gameObject.AddComponent<GameResult>();
        temp7.ConnectCenter(this);
        centerStates.Add(GameState.GameResult, temp7);

        bgmManager = bgmManagerObj.GetComponent<BGMManager>();
        bgmManagerView = bgmManagerObj.GetComponent<PhotonView>();
        deathUI = deathUIObj.GetComponent<DeathCamUI>();
        deathUIView = deathUIObj.GetComponent<PhotonView>();
        playerStat = playerStatObj.GetComponent<PlayerStats>();
        inGameUIView = inGameUIObj.GetComponent<PhotonView>();

        ConnectBetweenBGM("LoadingCharacterBGM");

        currentGameState = GameState.CharacterSelect;
        currentCenterState = centerStates[currentGameState];

        loadingTime = gameCenterTestData.loadingTime;
        characterSelectTime = gameCenterTestData.characterSelectTime;
        readyTime = gameCenterTestData.readyTime;
        playerRespawnTime = gameCenterTestData.playerRespawnTime;
        assistTime = gameCenterTestData.assistTime;

        angleStatue.GetComponent<AngelStatue>().angelStatueCoolTime = angelStatueCoolTime = gameCenterTestData.angelStatueCoolTime;
        angelStatueHpPerTime = gameCenterTestData.angelStatueHpPerTime;
        angelStatueContinueTime = gameCenterTestData.angelStatueContinueTime;

        occupyingGaugeRate = gameCenterTestData.occupyingGaugeRate;
        occupyingReturnTime = gameCenterTestData.occupyingReturnTime;
        occupyingRate = gameCenterTestData.occupyingRate;
        occupyingComplete = gameCenterTestData.occupyingComplete;
        roundEndTime = gameCenterTestData.roundEndTime;
        roundEndResultTime = gameCenterTestData.roundEndResultTime;

    }

    void ConnectBetweenBGM(string objName)
    {
        betweenBGMObj = GameObject.Find(objName);
        if(betweenBGMObj == null)
        {
            Debug.LogError("�� ���� BGM ���� ������Ʈ�� ã�� �� ����");
            return;
        }

        betweenBGMSource = betweenBGMObj.GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentCenterState.StateExecution();
            currentCenterState = centerStates[currentGameState];

            photonView.RPC("SerializeGameCenterDatas", RpcTarget.AllBufferedViaServer, ToDoSerlize());

            if (inGameUI != null)
                GiveDataToUI();
        }

        if (currentGameState == GameState.CharacterSelect)
        {
            BGMVolControl();
        }
    }

    float soundDecreaseTime = 5;
    float soundDecreaseSpeed = 1.5f;

    void BGMVolControl()
    {
        // ���� �ð��� ������ �Ҹ��� ���� ���ҵ�
        if (globalDesiredTimer - globalTimer <= soundDecreaseTime)
        {
            if (betweenBGMSource != null)
            {
                BetweenBGMVolumControl(soundDecreaseSpeed * Time.deltaTime / 10, false);
            }
        }

        else
        {
            if (betweenBGMSource != null)
            {
                betweenBGMSource.volume = 1.0f * SettingManager.Instance.bgmVal * SettingManager.Instance.soundVal;
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

    [PunRPC]
    public void ActiveObject(string name, bool isActive)
    {
        switch(name)
        {
            case "characterSelectObj":
                characterSelectObj.SetActive(isActive);
                break;
            case "inGameUIObj":
                inGameUIObj.transform.GetChild(0).gameObject.SetActive(isActive);
                break;
            case "betweenBGMObj":
                betweenBGMObj.SetActive(isActive);
                break;
            case "gameResultObj":
                gameResultObj.SetActive(isActive);
                break;
            case "playerStatObj":
                playerStatObj.SetActive(isActive);
                break;
            case "deathUIObj":
                deathUIObj.SetActive(isActive);
                break;
            default:
                Debug.LogWarning("�߸��� ���� ������Ʈ �̸� ���");
                break;
        }
    }

    [PunRPC]
    public void ShowGameResult()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            PhotonView view = PhotonView.Find((int)player.CustomProperties["CharacterViewID"]);
            if (view == null) continue;

            view.RPC("DisActiveMe", RpcTarget.AllBuffered);
        }

        playerStatObj.SetActive(false);
        deathUIObj.SetActive(false);
        inGameUIObj.SetActive(false);
        betweenBGMObj.SetActive(true);
        betweenBGMSource.Stop();

        gameResultObj.SetActive(true);
    }

    [PunRPC]
    public void BetweenBGMPlay(bool isPlay)
    {
        if (betweenBGMSource.clip == null) return;

        if(isPlay)
        {
            betweenBGMSource.Play();
        }

        else
        {
            betweenBGMSource.Stop();
        }
    }

    [PunRPC]
    public void BetweenBGMVolumControl(float size, bool isIncrease)
    {
        if (betweenBGMSource.clip == null) return;

        if (isIncrease)
        {
            betweenBGMSource.volume += size * SettingManager.Instance.bgmVal * SettingManager.Instance.soundVal;
        }

        else
        {
            betweenBGMSource.volume -= size * SettingManager.Instance.bgmVal * SettingManager.Instance.soundVal;
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

    public static List<GameObject> FindObjects(GameObject parrent, string name)
    {
        List<GameObject> foundObject = new List<GameObject>();
        Transform[] array = parrent.GetComponentsInChildren<Transform>(true);

        foreach (Transform transform in array)
        {
            if (transform.name == name)
            {
                foundObject.Add(transform.gameObject);
            }
        }

        if (foundObject == null)
        {
            Debug.LogError("�ش� �̸��� ���� ������Ʈ�� ã�� ���߽��ϴ� : " + name);
        }

        return foundObject;
    }

    public static GameObject FindObjectWithViewID(int viewID)
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

    public void GiveDataToUI()
    {
        if (inGameUI == null) return;

        inGameUI.globalTimerUI = globalTimer;
        inGameUI.roundEndTimerUI = roundEndTimer;
        inGameUI.roundEndTimeUI = roundEndTime;
        inGameUI.occupyingAUI.rate = occupyingA.rate;
        inGameUI.occupyingBUI.rate = occupyingB.rate;
        inGameUI.occupyingTeamUI.name = occupyingTeam.name;
        inGameUI.occupyingTeamUI.rate = occupyingTeam.rate;
    }

    //private void OnApplicationFocus(bool focus)
    //{
    //    if (!focus)
    //    {
    //        if(FirebaseLoginManager.Instance == null)
    //        {
    //            FirebaseLoginManager temp = FirebaseLoginManager.Instance;
    //        }

    //        FirebaseLoginManager.Instance.SignOut();

    //    }

    //}

    //private void OnApplicationQuit()
    //{
    //    // �α� �ƿ�

    //    if (FirebaseLoginManager.Instance == null)
    //    {
    //        FirebaseLoginManager temp = FirebaseLoginManager.Instance;
    //    }

    //    FirebaseLoginManager.Instance.SignOut();
    //}

    ////�ΰ��� ������ Ȯ��
    //void OnGUI()
    //{
    //    GUI.Label(new Rect(10, 10, 100, 20), "FPS: " + (1.0f / Time.smoothDeltaTime));
    //}

    //�ΰ��� ������ Ȯ��

    //long temp = 0;
    //void OnGUI()
    //{
    //    GUI.Label(new Rect(10, 10, 100, 20), "Traffic : " + (PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesOut));
    //}
}
