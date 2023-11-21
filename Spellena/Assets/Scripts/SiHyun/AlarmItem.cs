using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlarmItem : MonoBehaviour
{
    public Text alarmType;
    public Text senderName;
    private string senderId;

    public void SetAlarmInfo(string _senderId ,string _senderName, string _alarmType)
    {
        senderId = _senderId;
        senderName.text = _senderName;
        alarmType.text = _alarmType;
    }

    public void OnClickAcceptButton()
    {
        if (alarmType.text == "ģ�� ��û")
        {
            FirebaseLoginManager.Instance.SetFriendRequestStatus(senderId, FirebaseLoginManager.Instance.GetUser().UserId, "accept");
        }
        else if (alarmType.text == "��Ƽ ��û")
        {

        }
    }

    public void OnClickRefuseButton()
    {
        if (alarmType.text == "ģ�� ��û")
        {
            FirebaseLoginManager.Instance.SetFriendRequestStatus(senderId, FirebaseLoginManager.Instance.GetUser().UserId, "refuse");
        }
        else if (alarmType.text == "��Ƽ ��û")
        {

        }
    }

    public void DestoryAlarm()
    {
        Destroy(this.gameObject);
    }
}
