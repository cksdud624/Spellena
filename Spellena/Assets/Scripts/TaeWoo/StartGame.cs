using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class StartGame : MonoBehaviour
{
    public GameObject redTeam;
    public GameObject blueTeam;

    public void GameStart()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            SetPlayerDatas();
            LoadSceneManager.LoadNextScene("TaeWooScene_3");
            Debug.Log("GameStart!!!");
        }
    }

    void SetPlayerDatas()
    {
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            // �÷��̾� �̸�, ĳ������ ���� ������Ʈ, ��, �� ������ ��, ų ��, ���� ��
            Hashtable playerData = new Hashtable();

            // �����ִ� ������
            playerData.Add("Name", player.NickName);

            //foreach(GameObject gameObject in redTeam.)

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

}
