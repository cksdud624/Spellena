using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class WaitingPlayers : CenterState
{
    private int tempNum = 1;
    public override void StateExecution()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= tempNum)
        {
            gameCenter.globalTimer = 0.0f;

            SetPlayerDatas();

            if (gameCenter.inGameUIObj != null)
            {
                gameCenter.inGameUIView = gameCenter.inGameUIObj.GetComponent<PhotonView>();
                gameCenter.inGameUI = gameCenter.inGameUIObj.GetComponent<InGameUI>();
            }

            MakeSpawnPoint();

            gameCenter.currentGameState = GameCenterTest.GameState.DataLoading;

        }
    }

    void SetPlayerDatas()
    {
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            // �÷��̾� �̸�, ĳ������ ���� ������Ʈ, ��, �� ������ ��, ų ��, ���� ��
            Hashtable playerData = new Hashtable();

            // �����ִ� ������
            playerData.Add("Name", player.ActorNumber.ToString());
            playerData.Add("Team", "none");
            playerData.Add("Character", null);
            playerData.Add("TotalDamage", 0);
            playerData.Add("TotalHeal", 0);
            playerData.Add("KillCount", 0);
            playerData.Add("DeadCount", 0);
            playerData.Add("IsAlive", true);
            playerData.Add("AngelStatueCoolTime", 0.0f);
            playerData.Add("KillerName", null);

            // �������� �ʴ� ������
            playerData.Add("CharacterViewID", 0);
            playerData.Add("ReSpawnTime", 0.0f);
            playerData.Add("SpawnPoint", new Vector3(0, 0, 0));

            // ����ȭ ���� �ʰ� ������ Ŭ���̾�Ʈ�� ������ Parameter / �÷��̾� ����� ���
            playerData.Add("ParameterName", null);

            playerData.Add("DamagePart", null);
            playerData.Add("DamageDirection", null);
            playerData.Add("DamageForce", null);

            playerData.Add("PlayerAssistViewID", null);

            player.SetCustomProperties(playerData);
        }

    }

    void MakeSpawnPoint()
    {
        gameCenter.playerSpawnA = GameCenterTest.FindObject(gameCenter.playerSpawnPoints, "TeamA").GetComponentsInChildren<Transform>(true);
        gameCenter.playerSpawnB = GameCenterTest.FindObject(gameCenter.playerSpawnPoints, "TeamB").GetComponentsInChildren<Transform>(true);
    }


}
