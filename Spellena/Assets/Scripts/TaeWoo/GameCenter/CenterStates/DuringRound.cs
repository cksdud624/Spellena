using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class DuringRound : CenterState
{
    public override void StateExecution()
    {
        gameCenter.globalTimer += Time.deltaTime;

        //������ ���ɵǾ������� ������ ���� ���ɺ����� ��������.
        if (gameCenter.currentOccupationTeam == gameCenter.teamA)
        {
            gameCenter.occupyingA.rate += Time.deltaTime * gameCenter.occupyingRate;//�� 1.8�ʴ� 1�� ����

            if (gameCenter.occupyingA.rate >= gameCenter.occupyingComplete)
                gameCenter.occupyingA.rate = gameCenter.occupyingComplete;
        }

        else if (gameCenter.currentOccupationTeam == gameCenter.teamB)
        {
            gameCenter.occupyingB.rate += Time.deltaTime * gameCenter.occupyingRate;

            if (gameCenter.occupyingB.rate >= gameCenter.occupyingComplete)
                gameCenter.occupyingB.rate = gameCenter.occupyingComplete;
        }

        OccupyAreaCounts();
        CheckPlayerReSpawn();
        CheckRoundEnd();
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
         if (targetPlayer != null && changedProps != null)
         {
             //pararmeter�� ����� key���� ã�´�.
             string param = (string)targetPlayer.CustomProperties["ParameterName"];
             targetPlayer.CustomProperties["ParameterName"] = "";

            PhotonView view = PhotonView.Find((int)targetPlayer.CustomProperties["CharacterViewID"]);
            if (view == null) return;

            switch (param)
            {
                 case "TotalDamage":
                     if (gameCenter.inGameUIView == null) break;
                    gameCenter.inGameUIView.RPC("ShowDamageUI", targetPlayer);
                    // �ش� �÷��̾ ���� ��ý�Ʈ Ÿ�̸� ����
                    string victimViewID = (string)targetPlayer.CustomProperties["PlayerAssistViewID"];
                    if (victimViewID == null) break;
                    targetPlayer.CustomProperties["PlayerAssistViewID"] = "";

                    Dictionary<string, float> temp = (Dictionary<string, float>)targetPlayer.CustomProperties["DealAssist"];
                    temp["AssistTime_" + victimViewID] = gameCenter.globalTimer + gameCenter.assistTime;
                    GameCenterTest.ChangePlayerCustomProperties(targetPlayer, "DealAssist", temp);
                    break;
                case "TotalHeal":
                    string healedViewID = (string)targetPlayer.CustomProperties["PlayerAssistViewID"];
                    if (healedViewID == null) break;
                    targetPlayer.CustomProperties["PlayerAssistViewID"] = "";

                    Dictionary<string, float> temp1 = (Dictionary<string, float>)targetPlayer.CustomProperties["HealAssist"];
                    temp1["AssistTime_" + healedViewID] = gameCenter.globalTimer + gameCenter.assistTime;
                    GameCenterTest.ChangePlayerCustomProperties(targetPlayer, "HealAssist", temp1);
                    break;
                 case "KillCount":
                     if (gameCenter.inGameUIView == null) break;
                    gameCenter.inGameUIView.RPC("ShowKillUI", targetPlayer, gameCenter.tempVictim);
                    gameCenter.inGameUIView.RPC("ShowKillLog", RpcTarget.AllBufferedViaServer, targetPlayer.CustomProperties["Name"],
                         gameCenter.tempVictim, ((string)targetPlayer.CustomProperties["Team"] == "A"), targetPlayer.ActorNumber);
                    CheckPlayerHealAssist(targetPlayer);
                    view.RPC("SetUltimatePoint", targetPlayer);
                        break;
                 case "DeadCount":
                    GameCenterTest.ChangePlayerCustomProperties (targetPlayer, "IsAlive", false);
                    GameCenterTest.ChangePlayerCustomProperties (targetPlayer, "ReSpawnTime", gameCenter.globalTimer + gameCenter.playerRespawnTime);

                    gameCenter.tempVictim = (string)targetPlayer.CustomProperties["Name"];
                    gameCenter.ShowTeamMateDead((string)targetPlayer.CustomProperties["Team"], (string)targetPlayer.CustomProperties["Name"]);

                     view.RPC("PlayerDeadForAll", RpcTarget.AllBufferedViaServer, (string)targetPlayer.CustomProperties["DamagePart"],
                         (Vector3)targetPlayer.CustomProperties["DamageDirection"], (float)targetPlayer.CustomProperties["DamageForce"]);
                     view.RPC("PlayerDeadPersonal", targetPlayer);

                    CheckPlayerDealAssist(targetPlayer,(string)targetPlayer.CustomProperties["KillerName"]);
                    break;
                case "AngelStatueCoolTime":
                    if(gameCenter.globalTimer >= (float)targetPlayer.CustomProperties["AngelStatueCoolTime"])
                    {
                        GameCenterTest.ChangePlayerCustomProperties(targetPlayer, "AngelStatueCoolTime", gameCenter.globalTimer + gameCenter.angelStatueCoolTime);
                        StartCoroutine(AngelStatue(targetPlayer));
                    }
                    break;
                 default:
                     break;
             }

         }
        
    }

    IEnumerator AngelStatue(Photon.Realtime.Player player)
    {
        for(int i = 0; i < gameCenter.angelStatueContinueTime; i++)
        {
            PhotonView view = PhotonView.Find((int)player.CustomProperties["CharacterViewID"]);
            if (view == null) break;

            view.RPC("AngelStatueHP", RpcTarget.AllBufferedViaServer, gameCenter.angelStatueHpPerTime);
            yield return new WaitForSeconds(1);
        }
    }

    void OccupyAreaCounts()//���� ������ �÷��̾ �� �� �����ϰ� �ִ��� Ȯ��
    {
        gameCenter.teamAOccupying = 0;
        gameCenter.teamBOccupying = 0;

        GameObject temp;

        for (int i = 0; i < gameCenter.playersA.Count; i++)
        {
            temp = gameCenter.FindObjectWithViewID((int)gameCenter.playersA[i].CustomProperties["CharacterViewID"]);

            if (temp.GetComponent<Character>().isOccupying == true)
            {
                gameCenter.teamAOccupying++;
            }
        }

        for (int i = 0; i < gameCenter.playersB.Count; i++)
        {
            temp = gameCenter.FindObjectWithViewID((int)gameCenter.playersB[i].CustomProperties["CharacterViewID"]);

            if (temp.GetComponent<Character>().isOccupying == true)
            {
                gameCenter.teamBOccupying++;
            }
        }

        if (gameCenter.teamAOccupying > 0 && gameCenter.teamBOccupying > 0)
        {
            //���� ���� ���̶�� ���� �˸�
            gameCenter.occupyingReturnTimer = 0f;
            //gameCenter.inGameUIView.RPC("ActiveInGameUIObj", RpcTarget.AllBufferedViaServer, "fighting", true);
        }
        else if (gameCenter.teamAOccupying > 0)//A�� ����
        {
            gameCenter.ChangeOccupyingRate(gameCenter.teamAOccupying, gameCenter.teamA);
            gameCenter.occupyingReturnTimer = 0f;
            //gameCenter.inGameUIView.RPC("ActiveInGameUIObj", RpcTarget.AllBufferedViaServer, "fighting", false);
        }
        else if (gameCenter.teamBOccupying > 0)//B�� ����
        {
            gameCenter.ChangeOccupyingRate(gameCenter.teamBOccupying, gameCenter.teamB);
            gameCenter.occupyingReturnTimer = 0f;
            //gameCenter.inGameUIView.RPC("ActiveInGameUIObj", RpcTarget.AllBufferedViaServer, "fighting", false);
        }
        else
        {
            gameCenter.occupyingReturnTimer += Time.deltaTime;
            //gameCenter.inGameUIView.RPC("ActiveInGameUIObj", RpcTarget.AllBufferedViaServer, "fighting", false);
        }

        if (gameCenter.occupyingReturnTimer >= gameCenter.occupyingReturnTime)
        {
            if (gameCenter.occupyingTeam.rate > 0f)
            {
                gameCenter.occupyingTeam.rate -= Time.deltaTime;
                if (gameCenter.occupyingTeam.rate < 0f)
                {
                    gameCenter.occupyingTeam.rate = 0f;
                    gameCenter.occupyingTeam.name = "";
                }
            }
        }

    }

    void CheckPlayerReSpawn()
    {
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if ((bool)player.CustomProperties["IsAlive"] && (bool)player.CustomProperties["IsAlive"] == true) continue;
            if ((float)player.CustomProperties["ReSpawnTime"] <= gameCenter.globalTimer)
            {
                PhotonView view = PhotonView.Find((int)player.CustomProperties["CharacterViewID"]);
                GameCenterTest.ChangePlayerCustomProperties(player, "IsAlive", true);

                if ((string)player.CustomProperties["Team"] == "A")
                {
                    view.RPC("PlayerReBornForAll", RpcTarget.AllBufferedViaServer, (Vector3)player.CustomProperties["SpawnPoint"]);
                }

                else if ((string)player.CustomProperties["Team"] == "B")
                {
                    view.RPC("PlayerReBornForAll", RpcTarget.AllBufferedViaServer, (Vector3)player.CustomProperties["SpawnPoint"]);
                }

                view.RPC("PlayerReBornPersonal", player);

                // ���� ��Ȱ �˸���

                if ((string)player.CustomProperties["Team"] == "A")
                {
                    foreach (var playerA in gameCenter.playersA)
                    {
                        gameCenter.inGameUIView.RPC("ShowTeamLifeDead", playerA, (string)player.CustomProperties["Name"], false);
                    }
                }

                else if ((string)player.CustomProperties["Team"] == "B")
                {
                    foreach (var playerB in gameCenter.playersB)
                    {
                        gameCenter.inGameUIView.RPC("ShowTeamLifeDead", playerB, (string)player.CustomProperties["Name"], false);
                    }
                }
            }
        }
    }

    void CheckPlayerDealAssist(Photon.Realtime.Player player, string killerName)
    {
        // player�� ���� �÷��̾�
        if((string)player.CustomProperties["Team"]=="A")
        {
            foreach(var teamPlayer in gameCenter.playersB)
            {
                // ������ ����
                if ((string)teamPlayer.CustomProperties["Name"] == killerName)
                {
                    //Debug.LogError("killerName");
                    continue;
                }

                foreach (var assist in (Dictionary<string, float>)teamPlayer.CustomProperties["DealAssist"])
                {
                    if (assist.Value >= gameCenter.globalTimer)
                    {
                        //Debug.LogError("Deal Assist!!");
                        PhotonView view = PhotonView.Find((int)teamPlayer.CustomProperties["CharacterViewID"]);
                        if (view == null) continue;
                        view.RPC("SetChargePoint", teamPlayer);
                    }
                }
            }
        }

        else if((string)player.CustomProperties["Team"] == "B")
        {
            foreach (var teamPlayer in gameCenter.playersA)
            {
                // ������ ����
                if ((string)teamPlayer.CustomProperties["Name"] == killerName)
                {
                    //Debug.LogError("killerName");
                    continue;
                }

                foreach (var assist in (Dictionary<string, float>)teamPlayer.CustomProperties["DealAssist"])
                {
                    if (assist.Value >= gameCenter.globalTimer)
                    {
                        //Debug.LogError("Deal Assist!!");
                        PhotonView view = PhotonView.Find((int)teamPlayer.CustomProperties["CharacterViewID"]);
                        if (view == null) continue;
                        view.RPC("SetChargePoint", teamPlayer);
                    }
                }
            }
        }
        
    }

    void CheckPlayerHealAssist(Photon.Realtime.Player player)
    {
        if ((string)player.CustomProperties["Team"] == "A")
        {
            foreach (var teamPlayer in gameCenter.playersA)
            {
                foreach (var assist in (Dictionary<string, float>)teamPlayer.CustomProperties["HealAssist"])
                {
                    if (assist.Value >= gameCenter.globalTimer)
                    {
                        //Debug.LogError("Heal Assist!!");
                        PhotonView view = PhotonView.Find((int)teamPlayer.CustomProperties["CharacterViewID"]);
                        if (view == null) continue;
                        view.RPC("SetChargePoint", teamPlayer);
                    }
                }
            }
        }

        else if ((string)player.CustomProperties["Team"] == "B")
        {
            foreach (var teamPlayer in gameCenter.playersB)
            {
                foreach (var assist in (Dictionary<string, float>)teamPlayer.CustomProperties["HealAssist"])
                {
                    if (assist.Value >= gameCenter.globalTimer)
                    {
                        //Debug.LogError("Heal Assist!!");
                        PhotonView view = PhotonView.Find((int)teamPlayer.CustomProperties["CharacterViewID"]);
                        if (view == null) continue;
                        view.RPC("SetChargePoint", teamPlayer);
                    }
                }
            }
        }
    }

    void CheckRoundEnd()
    {
        if (gameCenter.occupyingA.rate >= gameCenter.occupyingComplete &&
            gameCenter.currentOccupationTeam == gameCenter.teamA && gameCenter.teamBOccupying <= 0)
        {
            gameCenter.inGameUIView.RPC("ActiveInGameUIObj", RpcTarget.AllBufferedViaServer, "extraObj", true);
            gameCenter.inGameUIView.RPC("ActiveInGameUIObj", RpcTarget.AllBufferedViaServer, "redExtraUI", false);
            gameCenter.inGameUIView.RPC("ActiveInGameUIObj", RpcTarget.AllBufferedViaServer, "redExtraObj", true);
            gameCenter.roundEndTimer -= Time.deltaTime;

        }
        else if (gameCenter.occupyingB.rate >= gameCenter.occupyingComplete &&
            gameCenter.currentOccupationTeam == gameCenter.teamB && gameCenter.teamAOccupying <= 0)
        {
            gameCenter.inGameUIView.RPC("ActiveInGameUIObj", RpcTarget.AllBufferedViaServer, "extraObj", true);
            gameCenter.inGameUIView.RPC("ActiveInGameUIObj", RpcTarget.AllBufferedViaServer, "blueExtraUI", false);
            gameCenter.inGameUIView.RPC("ActiveInGameUIObj", RpcTarget.AllBufferedViaServer, "blueExtraObj", true);
            gameCenter.roundEndTimer -= Time.deltaTime;
        }
        else
        {
            gameCenter.roundEndTimer = gameCenter.roundEndTime;
        }

        if (gameCenter.roundEndTimer <= 0.0f)
        {
            //���� ����
            if (gameCenter.currentOccupationTeam == gameCenter.teamA)
            {
                gameCenter.occupyingA.rate = 100;
                gameCenter.roundA++;

                if (gameCenter.roundA == 1)
                {
                    gameCenter.inGameUIView.RPC("ActiveInGameUIObj", RpcTarget.AllBufferedViaServer, "redFirstPoint", true);

                    foreach (var player in gameCenter.playersA)
                    {
                        gameCenter.inGameUIView.RPC("ShowRoundWin", player, gameCenter.roundA + gameCenter.roundB);
                    }

                    foreach (var player in gameCenter.playersB)
                    {
                        gameCenter.inGameUIView.RPC("ShowRoundLoose", player, gameCenter.roundA + gameCenter.roundB);
                    }
                }

                else if (gameCenter.roundA == 2)
                {
                    gameCenter.inGameUIView.RPC("ActiveInGameUIObj", RpcTarget.AllBufferedViaServer, "redSecondPoint", true);

                    foreach (var player in gameCenter.playersA)
                    {
                        gameCenter.inGameUIView.RPC("ActiveInGameUIObj", player, "victory", true);
                    }

                    foreach (var player in gameCenter.playersB)
                    {
                        gameCenter.inGameUIView.RPC("ActiveInGameUIObj", player, "defeat", true);
                    }
                }

            }

            else if (gameCenter.currentOccupationTeam == gameCenter.teamB)
            {
                gameCenter.occupyingB.rate = 100;
                gameCenter.roundB++;

                if (gameCenter.roundB == 1)
                {
                    gameCenter.inGameUIView.RPC("ActiveInGameUIObj", RpcTarget.AllBufferedViaServer, "blueFirstPoint", true);

                    foreach (var player in gameCenter.playersB)
                    {
                        gameCenter.inGameUIView.RPC("ShowRoundWin", player, gameCenter.roundA + gameCenter.roundB);
                    }

                    foreach (var player in gameCenter.playersA)
                    {
                        gameCenter.inGameUIView.RPC("ShowRoundLoose", player, gameCenter.roundA + gameCenter.roundB);
                    }
                }

                else if (gameCenter.roundB == 2)
                {
                    gameCenter.inGameUIView.RPC("ActiveInGameUIObj", RpcTarget.AllBufferedViaServer, "blueSecondPoint", true);

                    foreach (var player in gameCenter.playersB)
                    {
                        gameCenter.inGameUIView.RPC("ActiveInGameUIObj", player, "victory", true);
                    }

                    foreach (var player in gameCenter.playersA)
                    {
                        gameCenter.inGameUIView.RPC("ActiveInGameUIObj", player, "defeat", true);
                    }
                }
            }

            //���� ����
            gameCenter.currentGameState = GameCenterTest.GameState.RoundEnd;
            //gameCenter.globalTimer = gameCenter.roundEndResultTime;

        }
    }


}
