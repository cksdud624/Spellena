using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoading : CenterState
{
    public override void StateExecution()
    {
        gameCenter.gameStateString = "������ �ҷ����� ��...";

        // �� �� ĳ���� ������ �ε�

        gameCenter.globalTimer -= Time.deltaTime;
        if (gameCenter.globalTimer <= 0.0f)
        {
            gameCenter.currentGameState = GameCenterTest.GameState.CharacterSelect;
            gameCenter.globalTimer = gameCenter.characterSelectTime;
        }
    }


}
