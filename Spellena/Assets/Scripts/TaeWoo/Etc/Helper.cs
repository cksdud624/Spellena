using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Player;
using BehaviourTree;

public class Helper : MonoBehaviour
{
    public static Photon.Realtime.Player FindPlayerWithCustomProperty(string key, string value)
    {
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (player.CustomProperties.ContainsKey(key) && player.CustomProperties[key].ToString() == value)
            {
                return player;
            }
        }

        return null;
    }

    // �� �Լ��� ����ؾ� Ŭ���̾�Ʈ�� ��� ������ ����ȭ �ȴ�. / �׳� ������ ����ȭ �ȵ�
    public static void ChangePlayerCustomProperties(Photon.Realtime.Player player, string key, object value)
    {
        Hashtable temp = player.CustomProperties;

        if (temp[key] == null)
        {
            temp.Add(key, value);
        }

        else
        {
            temp[key] = value;
        }

        player.SetCustomProperties(temp);
    }

    public static GameObject FindObject(GameObject parrent, string name)
    {
        GameObject foundObject = null;
        Transform[] array = parrent.GetComponentsInChildren<Transform>(true);

        foreach (Transform transform in array)
        {
            if (transform.name == name)
            {
                foundObject = transform.gameObject;
                break; // ã������ ������ ����.
            }
        }

        if (foundObject == null)
        {
            Debug.LogError("�ش� �̸��� ���� ������Ʈ�� ã�� ���߽��ϴ� : " + name);
        }

        return foundObject;
    }

    public static List<GameObject> FindObjects(GameObject parrent, string name)
    {
        List<GameObject> foundObject = new List<GameObject>();
        Transform[] array = parrent.GetComponentsInChildren<Transform>(true);

        foreach (Transform transform in array)
        {
            if (transform.name == name)
            {
                foundObject.Add(transform.gameObject);
            }
        }

        if (foundObject == null)
        {
            Debug.LogError("�ش� �̸��� ���� ������Ʈ�� ã�� ���߽��ϴ� : " + name);
        }

        return foundObject;
    }

    public static GameObject FindObjectWithViewID(int viewID)
    {
        PhotonView photonView = PhotonView.Find(viewID);

        if (photonView == null)
        {
            Debug.LogError("�ش� " + viewID + "�� ���� ������Ʈ�� ã�� �� �����ϴ�.");
            return null;
        }

        else
        {
            return photonView.gameObject;
        }
    }

    public static GameObject FindGameObjectInParent(GameObject parent, string name)
    {
        Transform[] childTransforms = parent.GetComponentsInChildren<Transform>(true);

        foreach (Transform childTransform in childTransforms)
        {
            if (childTransform.name == name)
            {
                return childTransform.gameObject;
            }
        }

        Debug.LogError("<color=magenta>" + name + "</color> �̸��� GameObject�� ã�� �� �����ϴ�.");
        return null;
    }
}
