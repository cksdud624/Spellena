using Photon.Pun;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonShield : SpawnObject
{
    public TriggerEventer triggerEventer;
    public DracosonData dracosonData;
    public Transform center;
    List<string> hitObjects = new List<string>();
    private float deleteTime;
    private float shieldGage;

    private void Start()
    {
        Init();
        deleteTime = 5f;
        shieldGage = dracosonData.skill3ShieldGage;
    }

    void FixedUpdate()
    {
        deleteTime -= Time.deltaTime;
        if (shieldGage <= 0f || deleteTime <= 0f)
        {
            if(PhotonNetwork.IsMasterClient)
                CallRPCTunnel("RequestDestroy");
        }
    }

    void Init()
    {
        triggerEventer.hitTriggerEvent += TriggerEvent;
    }



    void CallRPCTunnel(string tunnelCommand)
    {
        object[] _tempData;
        _tempData = new object[2];
        _tempData[0] = tunnelCommand;

        photonView.RPC("CallRPCTunnelDracosonDragonShield", RpcTarget.AllBuffered, _tempData);
    }

    [PunRPC]
    public void CallRPCTunnelDracosonDragonShield(object[] data)
    {
        if ((string)data[0] == "RequestDestroy")
            RequestDestroy();
    }

    void RequestDestroy()
    {
        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }

    void TriggerEvent(GameObject hitObject)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (hitObject.gameObject.layer == 16)
            {
                if(!IsInternalProjectile(hitObject.transform.position))
                {
                    photonView.RPC("DestroyObject", RpcTarget.AllBuffered, hitObject);
                }
            }
        }
    }

    bool IsInternalProjectile(Vector3 position)
    {
        // ���ο��� ������ ����ü�� ���� ������ ������ �������� Ȯ��
        // ���� ���, ��ü �ݶ��̴��� �߽��� �������� ���� ������ ������ �� �ֽ��ϴ�.

        float internalRadius = 2.5f; // ���� ������ ������

        // ��ü �ݶ��̴��� �߽ɰ� �浹 ���� ���� �Ÿ��� ����Ͽ� �������� ���� �Ǵ�
        float distanceToCenter = Vector3.Distance(center.position, position);

        return distanceToCenter <= internalRadius;
    }

    [PunRPC]
    void DestroyObject(GameObject hitObject)
    {
        // ��� Ŭ���̾�Ʈ���� �ش� ������Ʈ�� ����
        PhotonView _pv = hitObject.GetComponent<PhotonView>();
        if (_pv != null && _pv.IsMine)
        {
            PhotonNetwork.Destroy(hitObject);
        }
    }

}


