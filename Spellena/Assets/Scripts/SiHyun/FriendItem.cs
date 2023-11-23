using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Firebase.Database;

public class FriendItem : MonoBehaviour
{
    string userId;
    public Text userNickname;
    public GameObject offlineLayer;
    public GameObject functionButtons;
    public Button inviteButton;

    private string senderId;

    private void SetOfflineLayer(DataSnapshot _snapshot)
    {
        Debug.Log("�Լ� ȣ��");
        string _status = _snapshot.Value.ToString();
        if (_status == "�¶���")
        {
            offlineLayer.SetActive(false);
            functionButtons.SetActive(true);
            Debug.LogWarning("Unexpected status: " + _status);

        }
        else if (_status == "��������")
        {
            offlineLayer.SetActive(true);
            functionButtons.SetActive(false);
            Debug.LogWarning("Unexpected status: " + _status);
        }
    }

    public void SetFriendInfo(string _id, string _nickname, string _status)
    {
        userId = _id;
        userNickname.text = _nickname;
        var _sendUser = FirebaseLoginManager.Instance.GetUser();
        senderId = _sendUser.UserId;
        if (_status == "�¶���")
        {
            functionButtons.SetActive(true);
            offlineLayer.SetActive(false);
            Debug.LogWarning("Unexpected status: " + _status);
        }
        else if (_status == "��������")
        {
            functionButtons.SetActive(false);
            offlineLayer.SetActive(true);
            Debug.LogWarning("Unexpected status: " + _status);
        }

        DatabaseReference statusRef = FirebaseLoginManager.Instance.GetReference().Child("users").Child(_id).Child("status");
        statusRef.ValueChanged += (sender, args) =>
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            try
            {
                SetOfflineLayer(args.Snapshot);
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception in ValueChanged event handler: " + ex.Message);
            }
        };
    }

    public void SetUpButtons(GameObject _quickMatchUI, GameObject _mainUI, GameObject _friendUI, Action<string> action)
    {
        inviteButton.onClick.AddListener(() => OnButtonClick(_quickMatchUI, _mainUI, _friendUI, action));
        Debug.Log("��ư Ŭ�� ������ �߰�");
    }

    private void OnButtonClick(GameObject _quickMatchUI, GameObject _mainUI, GameObject _friendUI, Action<string> action)
    {
        FirebaseLoginManager.Instance.SendPartyRequest(senderId, userId);
        FirebaseLoginManager.Instance.SetLobbyMaster(senderId, true);
        _quickMatchUI.SetActive(true);
        _mainUI.SetActive(false);
        _friendUI.SetActive(false);
        action(senderId);
        Debug.Log("�ʴ� ��ư ����");
    }



    public void ExecuteFunction(Action<string> action, string _userId)
    {
        action(_userId);
    }

}
