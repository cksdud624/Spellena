using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LoadSceneManager : MonoBehaviourPunCallbacks
{
    public static string nextScene;
    public static List<int> redTeamActorNums = new List<int>();
    public static List<int> blueTeamActorNums = new List<int>();
    public float loadingTime = 5.0f;
    private GameObject betweenBGMObj;
    private SettingManager settingManager;

    public static void LoadNextScene(string sceneName, List<int> redTeam, List<int> blueTeam)
    {
        nextScene = sceneName;
        redTeamActorNums = redTeam;
        blueTeamActorNums = blueTeam;

        PhotonNetwork.LoadLevel("TaeWoo_LoadingScene");
    }

    public static void LoadNextSceneAI(string sceneName)
    {
        nextScene = sceneName;
        redTeamActorNums.Add(PhotonNetwork.LocalPlayer.ActorNumber);
        PhotonNetwork.LoadLevel("TaeWoo_LoadingScene");
    }

    public static void GoBackToMenu(string sceneName)
    {
        nextScene = sceneName;

        PhotonNetwork.LoadLevel("TaeWoo_BackToMainLoadingScene");
    }

    void OnEnable()
    {
        if (PhotonNetwork.IsMasterClient) ToDoMaster();
        StartCoroutine(LoadSceneProcess());

        betweenBGMObj = GameObject.Find("LoadingCharacterBGM");
        if (betweenBGMObj == null) return;

        if (nextScene == "TaeWooScene_3")
        {
            LinkSettingManager();
            betweenBGMObj.GetComponent<AudioSource>().volume = 1.0f * settingManager.soundVal * settingManager.bgmVal;
            betweenBGMObj.GetComponent<AudioSource>().Play();
        }
    }

    void LinkSettingManager()
    {
        GameObject temp = GameObject.Find("SettingManager");

        if (temp == null)
        {
            Debug.LogError("SettingManager을 찾을 수 없습니다.");
            return;
        }

        settingManager = temp.GetComponent<SettingManager>();

        if (settingManager == null)
        {
            Debug.LogError("SettingManager의 Component을 찾을 수 없습니다.");
            return;
        }
    }

    void ToDoMaster()
    {
        photonView.RPC("SyncNextScene", RpcTarget.AllBufferedViaServer, nextScene);
        SetPlayerDatas();
    }

    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0.0f;

        while (!op.isDone)
        {
            timer += Time.deltaTime;
            yield return null;

            if (op.progress < 0.9f)
            {
                // 게이지 바 채움
                //progressbar.fillamount = op.progress
            }

            else
            {
                //progressbar.fillamount = Mathf.Lerp(0.9f,1f,timer)
                if (timer >= loadingTime)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    [PunRPC]
    public void SyncNextScene(string sceneName)
    {
        nextScene = sceneName;
    }

    void SetPlayerDatas()
    {
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            // 플레이어 이름, 캐릭터의 게임 오브젝트, 팀, 총 데미지 수, 킬 수, 죽은 수
            Hashtable playerData = new Hashtable();

            // 보여주는 데이터

            foreach (int num in redTeamActorNums)
            {
                if (player.ActorNumber == num)
                {
                    playerData.Add("Team", "A");
                    break;
                }
            }

            foreach (int num in blueTeamActorNums)
            {
                if (player.ActorNumber == num)
                {
                    playerData.Add("Team", "B");
                    break;
                }
            }

            playerData.Add("Name", player.NickName);
            playerData.Add("Character", null);

            playerData.Add("KillCount", 0);
            playerData.Add("DeadCount", 0);
            playerData.Add("AsisstCount", 0);
            playerData.Add("UltimateCount", 0);

            playerData.Add("TotalDamage", 0);
            playerData.Add("TotalHeal", 0);
            playerData.Add("IsAlive", true);
            playerData.Add("AngelStatueCoolTime", 0.0f);
            playerData.Add("KillerName", null);

            // 보여주지 않는 데이터
            playerData.Add("CharacterViewID", 0);
            playerData.Add("ReSpawnTime", 100000000.0f);
            playerData.Add("SpawnPoint", new Vector3(0, 0, 0));

            // 동기화 되지 않고 마스터 클라이언트만 가지는 Parameter / 플레이어 사망시 사용
            playerData.Add("ParameterName", null);

            playerData.Add("DamagePart", null);
            playerData.Add("DamageDirection", null);
            playerData.Add("DamageForce", null);

            playerData.Add("PlayerAssistViewID", null);

            playerData.Add("RoomInfo", null);

            // 마스터 클라이언트에 방 정보를 넣는다.
            if (player.IsMasterClient)
            {
                playerData["RoomInfo"] = "UserGame";
            }

            player.SetCustomProperties(playerData);
        }

    }
}
