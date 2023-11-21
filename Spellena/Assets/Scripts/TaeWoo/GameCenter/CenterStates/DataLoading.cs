using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoading : CenterState
{
    bool isCheckTimer = false;
    float tempTimer = 0.0f;
    public override void StateExecution()
    {
        if (!isCheckTimer)
        {
            isCheckTimer = !isCheckTimer;
            tempTimer = gameCenter.globalTimer;
            gameCenter.globalDesiredTimer = tempTimer + gameCenter.loadingTime;
        }

        gameCenter.gameStateString = "������ �ҷ����� ��...";

        gameCenter.globalTimer += Time.deltaTime;

        // �� �� ĳ���� ������ �ε�

        if (gameCenter.globalTimer >= gameCenter.globalDesiredTimer)
        {
            gameCenter.currentGameState = GameCenterTest.GameState.CharacterSelect;
        }
    }


}
