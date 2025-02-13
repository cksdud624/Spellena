using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using FSM;
using GameCenterDataType;

public class GameCenter0 : StateMachine
{
    [HideInInspector]
    public Dictionary<string, GameObject> gameCenterObjs 
        = new Dictionary<string, GameObject>();

    [HideInInspector]
    public Dictionary<GameState, BaseState> GameStates
    = new Dictionary<GameState, BaseState>();

    [HideInInspector]
    public Dictionary<string, PlayerStat> players
        = new Dictionary<string, PlayerStat>();

    [HideInInspector]
    public PhotonView gameManagerView;
    [HideInInspector]
    public PhotonView bgmManagerView;
    [HideInInspector]
    public BGMManager bgmManager;

    [HideInInspector]
    public GlobalTimer globalTimer;
    [HideInInspector]
    public RoundData roundData;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        LinkingCenterObjects(this);
        LinkingGameStates(this);
        LinkingProperties(this);
    }

    protected override void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            base.FixedUpdate();
            globalTimer.globalTime += Time.fixedDeltaTime;
            Debug.Log(currentState.name);
        }

    }

    protected override BaseState GetInitalState()
    {
        return GameStates[GameState.InTheRoom];
    }

    void LinkingCenterObjects(StateMachine stateMachine)
    {
        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameCenterObjs[gameObject.transform.GetChild(i).name] 
                = gameObject.transform.GetChild(i).gameObject;
            gameCenterObjs[gameObject.transform.GetChild(i).name].SetActive(false);
        }
    }

    void LinkingGameStates(StateMachine stateMachine)
    {
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
        gameManagerView = ((GameCenter0)stateMachine).GetComponent<PhotonView>();
        if (gameManagerView == null) Debug.LogError("no gameManagerView");

        bgmManager = ((GameCenter0)stateMachine).gameCenterObjs["BGMManager"].GetComponent<BGMManager>();
        if (bgmManager == null) Debug.LogError("no bgmManager");
        bgmManagerView = ((GameCenter0)stateMachine).gameCenterObjs["BGMManager"].GetComponent<PhotonView>();
        if (bgmManagerView == null) Debug.LogError("no bgmManagerView");
    }

    [PunRPC]
    public void ActiveObject(string name, bool isActive)
    {
        if (gameCenterObjs[name] == null)
            Debug.LogError("Not match " + name + " in gameCenterObjs");
        else
            gameCenterObjs[name].SetActive(isActive);
    }
}
