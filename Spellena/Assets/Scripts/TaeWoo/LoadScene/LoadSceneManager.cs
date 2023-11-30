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

    public static void LoadNextScene(string sceneName, List<int> redTeam, List<int> blueTeam)
    {
        nextScene = sceneName;
        redTeamActorNums = redTeam;
        blueTeamActorNums = blueTeam;

        PhotonNetwork.LoadLevel("TaeWoo_LoadingScene");
    }

    public static void GoBackToMenu(string sceneName)
    {
        nextScene = sceneName;

        PhotonNetwork.LoadLevel("TaeWoo_BackToMainLoadingScene");
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient) ToDoMaster();
        StartCoroutine(LoadSceneProcess());
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

        while(!op.isDone)
        {
            timer += Time.deltaTime;
            yield return null;

            if(op.progress < 0.9f)
            {
                // ������ �� ä��
                //progressbar.fillamount = op.progress
            }

            else
            {
                //progressbar.fillamount = Mathf.Lerp(0.9f,1f,timer)
                if(timer >= loadingTime)
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
            // �÷��̾� �̸�, ĳ������ ���� ������Ʈ, ��, �� ������ ��, ų ��, ���� ��
            Hashtable playerData = new Hashtable();

            // �����ִ� ������

            foreach(int num in redTeamActorNums)
            {
                if(player.ActorNumber==num)
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
            playerData.Add("Character", "Aeterna");

            playerData.Add("KillCount", 0);
            playerData.Add("DeadCount", 0);
            playerData.Add("AsisstCount", 0);
            playerData.Add("UltimateCount", 0);
            playerData.Add("Ping", 0);

            playerData.Add("TotalDamage", 0);
            playerData.Add("TotalHeal", 0);
            playerData.Add("IsAlive", true);
            playerData.Add("AngelStatueCoolTime", 0.0f);
            playerData.Add("KillerName", null);

            // �������� �ʴ� ������
            playerData.Add("CharacterViewID", 0);
            playerData.Add("ReSpawnTime", 100000000.0f);
            playerData.Add("SpawnPoint", new Vector3(0, 0, 0));

            // ����ȭ ���� �ʰ� ������ Ŭ���̾�Ʈ�� ������ Parameter / �÷��̾� ����� ���
            playerData.Add("ParameterName", null);

            playerData.Add("DamagePart", null);
            playerData.Add("DamageDirection", null);
            playerData.Add("DamageForce", null);

            playerData.Add("PlayerAssistViewID", null);

            playerData.Add("RoomInfo", null);

            // ������ Ŭ���̾�Ʈ�� �� ������ �ִ´�.
            if(player.IsMasterClient)
            {
                playerData["RoomInfo"] = "UserGame";
            }

            player.SetCustomProperties(playerData);
        }

    }

}
