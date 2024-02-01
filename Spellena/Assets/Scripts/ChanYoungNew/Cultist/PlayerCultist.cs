using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using GlobalEnum;
using GameCenterDataType;
using static UnityEditor.Progress;

public class PlayerCultist : PlayerCommon
{
    private GameObject chaser;
    protected override void InitUniqueComponents()
    {
        for(int i = 0; i < skillDatas.Count; i++)
        {
            skillDatas[i].statesRoute.Add(SkillData.State.None);
            skillDatas[i].statesRoute.Add(SkillData.State.Casting);
        }

        //��ų 1
        skillDatas[0].statesRoute.Add(SkillData.State.Channeling);
        //��ų 2
        skillDatas[1].statesRoute.Add(SkillData.State.Channeling);
        //��ų 3
        skillDatas[2].statesRoute.Add(SkillData.State.Channeling);

        for(int i = 0; i < plainDatas.Count; i++)
        {
            plainDatas[i].statesRoute.Add(SkillData.State.None);
        }
        plainDatas[0].statesRoute.Add(SkillData.State.Casting);
        plainDatas[1].statesRoute.Add(SkillData.State.Holding);
        plainDatas[1].statesRoute.Add(SkillData.State.Casting);
        plainDatas[2].statesRoute.Add(SkillData.State.Casting);
    }

    protected override void PlayNormalSkillLogic(int index)
    {
        if (skillDatas[index].statesRoute[skillDatas[index].routeIndex] == SkillData.State.Casting)
        {
            skillDatas[index].progressTime = playerData.skillCastingTime[index];
            PlayLogic(CallType.Skill, SkillData.State.Casting, index);
            CallPlayAnimation(AnimationChangeType.Invoke, CallType.Skill, index);
        }
        else if (skillDatas[index].statesRoute[skillDatas[index].routeIndex] == SkillData.State.Channeling)
        {
            skillDatas[index].progressTime = playerData.skillChannelingTime[index];
            PlayLogic(CallType.Skill, SkillData.State.Channeling, index);
        }
        else if (skillDatas[index].statesRoute[skillDatas[index].routeIndex] == SkillData.State.None)
        {
            PlayLogic(CallType.Skill, SkillData.State.None, index);
        }
    }

    protected override void PlayLogic(CallType callType, SkillData.State state, int index)
    {
        if(photonView.IsMine)
        {
            if(callType == CallType.Skill)
            {
                if (state == SkillData.State.Channeling)
                {
                    if (index == 0)
                        PlaySkillLogic1();
                    else if (index == 1)
                        PlaySkillLogic2();
                    else if (index == 2)
                        PlaySkillLogic3();
                }
                else if(state == SkillData.State.Casting)
                {
                    if(index == 3)
                        PlaySkillLogic4();
                    Debug.Log("ĳ����");
                }
                else if(state == SkillData.State.None)
                {
                    if (index == 1)
                        EndSkillLogic2();
                }

            }
            else if(callType == CallType.Plain)
            {

            }
        }
    }

    private void PlaySkillLogic1()
    {
        Debug.Log("ũ�ƾ�");
        //��
        Ray _ray = cameraMain.ScreenPointToRay(aim.transform.position);
        RaycastHit _hit;
        if(Physics.Raycast(_ray, out _hit ,Mathf.Infinity, LayerMask.GetMask("Player") | layerMaskWall | layerMaskMap))
        {
            if(_hit.collider.transform.root.gameObject.layer == 15)
            {
                if(_hit.collider.transform.root.tag == tag)
                {
                    PhotonView _photonView = _hit.collider.transform.root.GetComponent<PhotonView>();

                    object[] _tempOther = new object[2];
                    _tempOther[0] = _photonView.ViewID;
                    _tempOther[1] = tag;

                    PhotonNetwork.Instantiate("ChanYoungNew/Cultist/CultistSkill1",
                        _photonView.transform.position + new Vector3(0, 1f, 0), Quaternion.identity, data: _tempOther);

                    return;
                    //_photonView.RPC(); -> ������
                }
            }
        }

        object[] _temp = new object[2];
        _temp[0] = photonView.ViewID;
        _temp[1] = tag;

        PhotonNetwork.Instantiate("ChanYoungNew/Cultist/CultistSkill1",
                        photonView.transform.position + new Vector3(0, 1f, 0), Quaternion.identity, data: _temp);

        //photonView.RPC()-> ������

    }

    private void PlaySkillLogic2()
    {
        object[] _temp = new object[2];
        _temp[0] = photonView.ViewID;
        _temp[1] = tag;

        chaser = PhotonNetwork.Instantiate("ChanYoungNew/Cultist/CultistSkill2",
                        cameraMain.transform.position + transform.forward,
                        transform.rotation * cameraMain.transform.localRotation, data: _temp);

        playerInput.enabled = false;
        //���ο� ������Ʈ ����
        //������Ʈ�� ī�޶� ����
        //������Ʈ �̵�
    }

    private void EndSkillLogic2()
    {
        PhotonNetwork.Destroy(chaser);
        chaser = null;

        playerInput.enabled = true;
        Debug.Log("��ų ������?");
    }

    private void PlaySkillLogic3()
    {
        //����� ī�޶� ������ ȿ���� Ŵ
        //��� �÷��̾�� Ray�� ���� �´� ����� �ü��� ������
    }

    private void PlaySkillLogic4()
    {
        //���� ��ο��� ���⸦ ��
    }

    protected override void PlayNormalPlainLogic(int index)
    {
    }

    [PunRPC]
    public void TeleportPlayer(int photonViewNum)
    {
        if (photonView.IsMine)
        {
            PhotonView _targetView = PhotonNetwork.GetPhotonView(photonViewNum);
            transform.position = _targetView.transform.position + _targetView.transform.forward * 2f;

            Vector3 _direction = -_targetView.transform.forward;

            transform.rotation = Quaternion.LookRotation(_direction);

            cameraMain.transform.localRotation = Quaternion.identity;

            _targetView.RPC("AddState", _targetView.Owner, "Horror", 1.5f, photonView.ViewID);

        }

        if (skillDatas[1].routeIndex == 2)
        {
            ChangeNextRoot(CallType.Skill, 1);
            skillDatas[1].progressTime = 0;
        }
    }
}
