using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class DeadCamMove : MonoBehaviour
{
    int index = 0;
    bool isButtonDown = false;

    List<Player.Character> players = new List<Player.Character>();

    // Ȱ��ȭ �� �� ���� ���� �����ִ� �÷��̾��� ����Ʈ�� �޴´� -> �߰� Ż�� ���
    void OnEnable()
    {
        Player.Character[] allPlayer = FindObjectsOfType<Player.Character>();

        foreach(var player in allPlayer)
        {
            if(CompareTag(player.gameObject.tag))
            {
                if(player.playerName == this.GetComponent<Player.Character>().playerName)
                {
                    index = players.Count;
                }

                players.Add(player);
            }
        }

    }

    void OnDisable()
    {
        Transform temp = players[index].transform.GetChild(0).GetChild(0).GetChild(0);

        for (int i = 0; i < temp.childCount; i++)
        {
            temp.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Other");
            temp.GetChild(i).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }

        players[index].camera.SetActive(false);

        Debug.Log("OnDisable");
    }

    void OnMove(InputValue value)
    {
        // ���� �÷��̾��� ī�޶� �ٸ� �Ʊ� �÷��̾��� ���� ��ġ�� �̵��Ѵ�
        // LookAt()���� �Ʊ� �÷��̾� �ü� ����
        // ���� �浹 �� ī�޶� collider����

        if (!isButtonDown && (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsAlive"]==false)
        {
            Debug.Log("OnButton");

            Transform temp = players[index].transform.GetChild(0).GetChild(0).GetChild(0);

            for (int i = 0; i < temp.childCount; i++)
            {
                temp.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Other");
                temp.GetChild(i).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
            }

            players[index].camera.SetActive(false);

            index += (int)value.Get<Vector2>().x;
            if (index < 0) index = 0;
            if (index >= players.Count) index = players.Count - 1;

            players[index].camera.SetActive(true);

            Transform temp1 = players[index].transform.GetChild(0).GetChild(0).GetChild(0);

            for (int i = 0; i < temp1.childCount; i++)
            {
                temp1.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Me");
                temp1.GetChild(i).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
            }

            isButtonDown = !isButtonDown;
        }
    }   
}
