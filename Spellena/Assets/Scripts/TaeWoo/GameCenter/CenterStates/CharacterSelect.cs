using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Player;

public class CharacterSelect : CenterState
{
    bool isCheckTimer = false;
    float tempTimer = 0.0f;
    public override void StateExecution()
    {
        if (!isCheckTimer)
        {
            isCheckTimer = !isCheckTimer;
            tempTimer = gameCenter.globalTimer;
            gameCenter.globalDesiredTimer = tempTimer + gameCenter.characterSelectTime;
        }

        gameCenter.gameStateString = "ĳ���� ����";

        gameCenter.globalTimer += Time.deltaTime;

        // ĳ���� ����

        if (gameCenter.globalTimer >= gameCenter.globalDesiredTimer)
        {
            gameCenter.currentGameState = GameCenterTest.GameState.GameReady;
            gameCenter.globalUIView.RPC("ActiveUI", RpcTarget.AllBufferedViaServer, "inGameUI", true);

            MakeCharacter();
            MakeTeamStateUI();
        }
    }
    void MakeCharacter()
    {
        int aTeamIndex = 1;
        int bTeamIndex = 1;

        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            //���� ĳ���ʹ� ���׸��� ����, ���� ������ ���� A,B �� ����
            //if((string)player.CustomProperties["Team"]=="A")
            string choseCharacter = "Aeterna";

            if (player.ActorNumber % 2 == 0)     // A �� (Red)
            {
                GameObject playerCharacter
                    = PhotonNetwork.Instantiate("TaeWoo/Prefabs/" + choseCharacter, gameCenter.playerSpawnA[aTeamIndex].position, Quaternion.identity);

                playerCharacter.GetComponent<PhotonView>().TransferOwnership(player.ActorNumber);
                playerCharacter.GetComponent<PhotonView>().RPC("IsLocalPlayer", player);
                playerCharacter.GetComponent<Character>().SetTagServer("TeamA");

                GameCenterTest.ChangePlayerCustomProperties(player, "CharacterViewID", playerCharacter.GetComponent<PhotonView>().ViewID);
                GameCenterTest.ChangePlayerCustomProperties(player, "Team", "A");
                GameCenterTest.ChangePlayerCustomProperties(player, "SpawnPoint", gameCenter.playerSpawnA[aTeamIndex].position);
                aTeamIndex++;
                gameCenter.playersA.Add(player);
            }

            else                // B �� (Blue)
            {
                GameObject playerCharacter
                    = PhotonNetwork.Instantiate("TaeWoo/Prefabs/" + choseCharacter, gameCenter.playerSpawnB[bTeamIndex].position, Quaternion.identity);

                playerCharacter.GetComponent<PhotonView>().TransferOwnership(player.ActorNumber);
                playerCharacter.GetComponent<PhotonView>().RPC("IsLocalPlayer", player);
                playerCharacter.GetComponent<Character>().SetTagServer("TeamB");

                GameCenterTest.ChangePlayerCustomProperties(player, "CharacterViewID", playerCharacter.GetComponent<PhotonView>().ViewID);
                GameCenterTest.ChangePlayerCustomProperties(player, "Team", "B");
                GameCenterTest.ChangePlayerCustomProperties(player, "SpawnPoint", gameCenter.playerSpawnB[bTeamIndex].position);
                bTeamIndex++;
                gameCenter.playersB.Add(player);
            }
        }
    }

    void MakeTeamStateUI()
    {
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if ((string)player.CustomProperties["Team"] == "A")
            {
                foreach (var playerA in gameCenter.playersA)
                {
                    gameCenter.globalUIView.RPC("ShowTeamState", player, playerA.CustomProperties["Name"], "Aeterna");
                }
            }

            else if ((string)player.CustomProperties["Team"] == "B")
            {
                foreach (var playerB in gameCenter.playersB)
                {
                    gameCenter.globalUIView.RPC("ShowTeamState", player, playerB.CustomProperties["Name"], "Aeterna");
                }
            }
        }
    }


}
