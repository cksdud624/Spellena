using Photon.Pun;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSight : SpawnObject
{
    public TriggerEventer triggerEventer;
    public DracosonData dracosonData;

    List<string> hitObjects = new List<string>();

    private void Start()
    {
        Init();
    }

    void Update()
    {
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

        photonView.RPC("CallRPCTunnelDracosonDragonSpin", RpcTarget.AllBuffered, _tempData);
    }

    [PunRPC]
    public void CallRPCTunnelDracosonDragonSpin(object[] data)
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
            if (hitObject.transform.root.gameObject.name != hitObject.name)
            {
                GameObject _rootObject = hitObject.transform.root.gameObject;
                if (!hitObjects.Contains(_rootObject.name))
                {
                    hitObjects.Add(_rootObject.name);
                    if (_rootObject.GetComponent<Character>() != null)
                    {
                        //if(_rootObject.tag != tag)
                        {
                            _rootObject.GetComponent<PhotonView>().RPC("PlayerDamaged", RpcTarget.AllBuffered,
                                playerName, (int)(dracosonData.skill1Damage), hitObject.name, transform.forward, 20f);
                        }
                    }
                }
            }
        }
    }

    [PunRPC]
    void DestoryObject(GameObject hitObject)
    {
        // ��� Ŭ���̾�Ʈ���� �ش� ������Ʈ�� ����
        PhotonView _pv = hitObject.GetComponent<PhotonView>();
        if (_pv != null && _pv.IsMine)
        {
            PhotonNetwork.Destroy(hitObject);
        }
    }

}


