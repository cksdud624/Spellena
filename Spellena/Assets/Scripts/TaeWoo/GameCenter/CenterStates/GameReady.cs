using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace GameCenterTest0
{
    public class GameReady : CenterState
    {
        bool isCheckTimer = false;
        float tempTimer = 0.0f;
        public override void StateExecution()
        {
            if (!isCheckTimer)
            {
                isCheckTimer = !isCheckTimer;
                tempTimer = GameCenterTest.globalTimer;
                gameCenter.globalDesiredTimer = tempTimer + gameCenter.readyTime;
                gameCenter.bgmManagerView.RPC("PlayAudio", RpcTarget.AllBuffered, "DuringRound", 0.05f, true, true, "BGM");
            }

            GameCenterTest.globalTimer += Time.deltaTime;

            if (GameCenterTest.globalTimer >= gameCenter.globalDesiredTimer)
            {

                if (GameCenterTest.roundA == 0 && GameCenterTest.roundB == 0)
                {
                    MakeEnemyShader();
                    StartCoroutine(SetAssistTimer());

                }

                else
                {
                    gameCenter.currentGameState = GameCenterTest.GameState.DuringRound;
                    isCheckTimer = !isCheckTimer;
                }
            }

        }

        void MakeEnemyShader()
        {
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                // �� ���̴� ����
                PhotonView view = PhotonView.Find((int)player.CustomProperties["CharacterViewID"]);
                if (view == null) return;
                view.RPC("SetEnemyLayer", player);
            }
        }

        IEnumerator SetAssistTimer()
        {
            // ��ý�Ʈ Ÿ�̸� ����
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                yield return new WaitForSeconds(0.2f);

                Hashtable playerData = player.CustomProperties;
                // ��ý�Ʈ Ÿ�̸� �Ҵ� -> AssistTime_ + �� �÷��̾� ĳ���� ViewID
                if ((string)player.CustomProperties["Team"] == "A")
                {
                    Dictionary<string, float> temp = new Dictionary<string, float>();
                    Dictionary<string, float> temp1 = new Dictionary<string, float>();

                    foreach (var enemyPlayer in gameCenter.playersB)
                    {
                        if (temp.ContainsKey("AssistTime_" + (int)enemyPlayer.CustomProperties["CharacterViewID"])) continue;
                        temp.Add("AssistTime_" + (int)enemyPlayer.CustomProperties["CharacterViewID"], 0.0f);
                    }

                    if (!playerData.ContainsKey("DealAssist")) playerData.Add("DealAssist", temp);

                    foreach (var enemyPlayer in gameCenter.playersA)
                    {
                        if (temp1.ContainsKey("AssistTime_" + (int)enemyPlayer.CustomProperties["CharacterViewID"])) continue;
                        temp1.Add("AssistTime_" + (int)enemyPlayer.CustomProperties["CharacterViewID"], 0.0f);
                    }

                    if (!playerData.ContainsKey("HealAssist")) playerData.Add("HealAssist", temp1);

                }

                else if ((string)player.CustomProperties["Team"] == "B")
                {
                    Dictionary<string, float> temp = new Dictionary<string, float>();
                    Dictionary<string, float> temp1 = new Dictionary<string, float>();

                    foreach (var enemyPlayer in gameCenter.playersA)
                    {
                        if (temp.ContainsKey("AssistTime_" + (int)enemyPlayer.CustomProperties["CharacterViewID"])) continue;
                        temp.Add("AssistTime_" + (int)enemyPlayer.CustomProperties["CharacterViewID"], 0.0f);
                    }

                    if (!playerData.ContainsKey("DealAssist")) playerData.Add("DealAssist", temp);

                    foreach (var enemyPlayer in gameCenter.playersB)
                    {
                        if (temp1.ContainsKey("AssistTime_" + (int)enemyPlayer.CustomProperties["CharacterViewID"])) continue;
                        temp1.Add("AssistTime_" + (int)enemyPlayer.CustomProperties["CharacterViewID"], 0.0f);
                    }

                    if (!playerData.ContainsKey("HealAssist")) playerData.Add("HealAssist", temp1);
                }

                player.SetCustomProperties(playerData);
            }

            gameCenter.currentGameState = GameCenterTest.GameState.DuringRound;
            isCheckTimer = !isCheckTimer;
        }

    }
}