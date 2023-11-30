using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Linq;
using HashTable = ExitGames.Client.Photon.Hashtable;
using UnityEditor;
using System.Security.Cryptography;

public class Cultist : Character
{
    public CultistData cultistData;
    public GameObject overlayCamera;
    public GameObject overlaySightRight;
    public GameObject overlaySightLeft;

    public GameObject minimapCamera;
    public GameObject aim;

    public GameObject dagger;
    public GameObject overlayDagger;

    public GameObject lungeEffect;
    public GameObject overlayInvocationEffect;
    public GameObject overlayInvocationEffectOrb;
    public GameObject invocationEffect;
    public GameObject invocationEffectOrb;

    public Animator overlayAnimator;

    BuffDebuffChecker buffDebuffChecker;

    int ownerNum;
    Vector3 defaultCameraLocalVec;

    //������ ���̺� �׽�Ʈ ��
    float invocationCastingTime;
    float lungeHoldingTime;
    float lungeAttackTime;
    float throwCastingTime;
    float skill1CastingTime;
    float skill1ChannelingTime;
    float skill2CastingTime;
    float skill2ChannelingTime;
    float skill3CastingTime;
    float skill3ChannelingTime;
    float skill4CastingTime;

    float skill1CoolDownTime;
    float skill2CoolDownTime;
    float skill3CoolDownTime;
    float skill4CoolDownTime;

    float skill4Weight = 0f;

    float overlayLeftHandWeight = 0.4f;
    float overlayRightHandWeight = 0.4f;

    public enum SkillStateCultist
    {
        None, Invocation,
        LungeHolding, LungeAttack,
        Throw,
        Skill1Ready, Skill1Casting, Skill1Channeling,
        Skill2Ready, Skill2Casting, Skill2Channeling,
        Skill3Ready, Skill3Casting, Skill3Channeling,
        Skill4Ready, Skill4Casting
    }

    SkillStateCultist skillState = SkillStateCultist.None;

    //0 : ��ų1, 1 : ��ų2, 2 : ��ų3, 3 : ��ų4
    float[] skillCoolDownTime = new float[4];
    float[] skillCastingTime = new float[4];
    bool[] skillCastingCheck = new bool[4];
    float[] skillChannelingTime = new float[4];
    bool[] skillChannelingCheck = new bool[4];

    //0 : �ǽ�, 1 : �޽� Ȧ��, 2 : �޽� ����, 3 : ��ô
    float[] normalCastingTime = new float[4];
    bool[] normalCastingCheck = new bool[4];

    //0 : ���� ���콺, 1 : ������ ���콺
    bool[] isClicked = new bool[2];

    Vector3 aimPos;
    Vector3 aimDirection;

    protected override void Awake()
    {
        base.Awake();
        if (photonView.IsMine)
        {
            //�׽�Ʈ ����
            HashTable _tempTable = new HashTable();
            _tempTable.Add("CharacterViewID", photonView.ViewID);
            _tempTable.Add("IsAlive", true);
            PhotonNetwork.LocalPlayer.SetCustomProperties(_tempTable);


            object[] _tempData = new object[2];
            _tempData[0] = "SetOwnerNum";
            _tempData[1] = photonView.OwnerActorNr;
            RequestRPCCall(_tempData);
        }
    }

    protected override void Start()
    {
        base.Start();
        Init();
    }

    void Init()
    {
        defaultCameraLocalVec = camera.transform.localPosition;
        dataHp = cultistData.hp;
        hp = cultistData.hp;
        moveSpeed = cultistData.moveSpeed;
        backSpeed = cultistData.backSpeed;
        sideSpeed = cultistData.sideSpeed;
        runSpeedRatio = cultistData.runSpeedRatio;
        runSpeedRatio = cultistData.runSpeedRatio;
        sitSpeed = cultistData.sitSpeed;
        sitSideSpeed = cultistData.sitSideSpeed;
        sitBackSpeed = cultistData.sitBackSpeed;
        jumpHeight = cultistData.jumpHeight;
        headShotRatio = cultistData.headShotRatio;

        invocationCastingTime = cultistData.invocationCastingTime;
        lungeHoldingTime = cultistData.lungeHoldingTime;
        lungeAttackTime = cultistData.lungeAttackTime;
        throwCastingTime = cultistData.throwTime;
        skill1CastingTime = cultistData.skill1CastingTime;
        skill1ChannelingTime = cultistData.skill1Time;
        skill2CastingTime = cultistData.skill2CastingTime;
        skill2ChannelingTime = cultistData.skill2ChannelingTime;
        skill3CastingTime = cultistData.skill3CastingTime;
        skill3ChannelingTime = cultistData.skill3ChannelingTime;
        skill4CastingTime = cultistData.skill4CastingTime;

        skill1CoolDownTime = cultistData.skill1CoolDownTime;
        skill2CoolDownTime = cultistData.skill2CoolDownTime;
        skill3CoolDownTime = cultistData.skill3CoolDownTime;
        skill4CoolDownTime = cultistData.skill4CoolDownTime;

        buffDebuffChecker = GetComponent<BuffDebuffChecker>();
    }

    [PunRPC]
    public override void IsLocalPlayer()
    {
        base.IsLocalPlayer();
        overlayCamera.SetActive(true);
        minimapCamera.SetActive(true);
        dagger.SetActive(false);

        dagger.layer = 6;
        lungeEffect.transform.GetChild(0).gameObject.layer = 6;

        overlayInvocationEffect.layer = 8;
        overlayInvocationEffectOrb.layer = 8;
        for (int i = 0; i < 3; i++)
        {
            overlayInvocationEffect.transform.GetChild(i).gameObject.layer = 8;
        }

    }

    protected override void Update()
    {
        base.Update();
        CheckCoolDownTimeForAll();

        if (PhotonNetwork.IsMasterClient)
            CheckOnMasterClient();

        if (photonView.IsMine)
        {
            CheckAnimationSpeed();
            CheckAnimatorExtra();
        }
    }

    protected override void FixedUpdate()
    {
        if (photonView.IsMine)
            CheckChanneling();
        base.FixedUpdate();
    }

    void CheckChanneling()
    {
        
        if (skillState == SkillStateCultist.Skill2Channeling
            || skillState == SkillStateCultist.Skill4Casting)
            moveVec = Vector3.zero;

        if (skillState == SkillStateCultist.LungeHolding)
        {
            moveVec = Vector3.zero;
            rigidbody.MovePosition(rigidbody.transform.position + transform.forward * moveSpeed * runSpeedRatio * Time.deltaTime);
            animator.SetInteger("VerticalSpeed", 1);
            animator.SetInteger("HorizontalSpeed", 0);
        }
        
    }



    //��� Ŭ���̾�Ʈ���� �۵�
    void CheckCoolDownTimeForAll()
    {
        CheckCoolDownTimeLoop(ref skillCoolDownTime);
        CheckCoolDownTimeLoop(ref skillCastingTime);
        CheckCoolDownTimeLoop(ref skillChannelingTime);
        CheckCoolDownTimeLoop(ref normalCastingTime);
    }

    void CheckCoolDownTimeLoop(ref float[] times)
    {
        for (int i = 0; i < times.Length; i++)
        {
            if (times[i] > 0f)
                times[i] -= Time.deltaTime;
        }
    }

    //���� Ŭ���̾�Ʈ���� �۵�

    //������ Ŭ���̾�Ʈ������ �۵�
    void CheckOnMasterClient()
    {
        if (skillState == SkillStateCultist.Invocation)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Invocation"))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f && dagger.active == false)
                {
                    dagger.SetActive(true);
                    CallRPCEvent("SetDagger", "Response", true);
                }
            }

            if (normalCastingTime[0] <= 0f && normalCastingCheck[0])
            {
                normalCastingCheck[0] = false;
                CallRPCEvent("SetInvocationEffect", "Response", false);
                CallRPCEvent("SetAnimation", "Response", "isInvocation", false);
                CallRPCEvent("UpdateData", "Response", skillState, "OnlySkillState", 0, 0f, false);
            }
        }
        else if (skillState == SkillStateCultist.LungeHolding)
        {
            if (normalCastingTime[1] <= 0f && normalCastingCheck[1])
            {
                normalCastingCheck[1] = false;
                skillState = SkillStateCultist.LungeAttack;
                CallRPCEvent("SetAnimation", "Response", "isLungeHolding", false);
                CallRPCEvent("SetAnimation", "Response", "isLungeAttack", true);
                CallRPCEvent("SetLungeCollider", "Response", true);
                CallRPCEvent("UpdateData", "Response", skillState, "normalCastingTime", 2, lungeAttackTime, true);
            }
        }
        else if (skillState == SkillStateCultist.LungeAttack)
        {
            if (normalCastingTime[2] <= 0f && normalCastingCheck[2])
            {
                normalCastingCheck[2] = false;
                skillState = SkillStateCultist.None;
                CallRPCEvent("ResetAnimation", "Response");
                CallRPCEvent("SetDagger", "Response", false);
                CallRPCEvent("SetLungeCollider", "Response", false);
                CallRPCEvent("UpdateData", "Response", skillState, "OnlySkillState", 0, 0f, false);
            }
        }
        else if (skillState == SkillStateCultist.Throw)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Throw"))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.35f)
                {
                    if (dagger.active == true)
                    {
                        dagger.SetActive(false);
                        CallRPCEvent("InstantiateObject", "Response" , "Dagger");
                        CallRPCEvent("SetDagger", "Response", false);
                    }
                }

                if (normalCastingTime[3] <= 0f && normalCastingCheck[3])
                {
                    normalCastingCheck[3] = false;
                    skillState = SkillStateCultist.None;
                    CallRPCEvent("ResetAnimation", "Response");
                    CallRPCEvent("UpdateData", "Response", skillState, "OnlySkillState", 0, 0f, false);
                }
            }
        }
        else if (skillState == SkillStateCultist.Skill1Casting)
        {
            if (skillCastingTime[0] <= 0f && skillCastingCheck[0])
            {
                skillCastingCheck[0] = false;
                skillState = SkillStateCultist.Skill1Channeling;
                CallRPCEvent("ResetAnimation", "Response");
                CallRPCEvent("UpdateData", "Response", skillState, "skillChannelingTime", 0, skill1ChannelingTime, true);
            }
        }
        else if (skillState == SkillStateCultist.Skill1Channeling)
        {
            if (skillChannelingTime[0] <= 0f)
            {
                skillState = SkillStateCultist.None;
                CallRPCEvent("UseSkill", "Response", 0);
                CallRPCEvent("UpdateData", "Response", skillState, "OnlySkillState", 0, 0f, false);
                CallRPCEvent("SetCoolDownTime", "Response", 0);
            }
        }
        else if (skillState == SkillStateCultist.Skill2Casting)
        {
            if (skillCastingTime[1] <= 0f)
            {
                skillState = SkillStateCultist.Skill2Channeling;
                CallRPCEvent("UpdateData", "Response", skillState, "skillChannelingTime", 1, skill2ChannelingTime, false);
            }
        }
        else if (skillState == SkillStateCultist.Skill2Channeling)
        {
            if (skillChannelingTime[1] <= 0f)
            {
                skillState = SkillStateCultist.None;
                CallRPCEvent("ResetAnimation", "Response");
                CallRPCEvent("UpdateData", "Response", skillState, "OnlySkillState", 0, 0f, false);
                //��Ÿ�� ����
            }
        }
        else if (skillState == SkillStateCultist.Skill3Casting)
        {
            if (skillCastingTime[2] <= 0f)
            {
                skillState = SkillStateCultist.Skill3Channeling;
                CallRPCEvent("UpdateData", "Response", skillState, "skillChannelingTime", 2, skill3ChannelingTime, false);
            }
        }
        else if (skillState == SkillStateCultist.Skill3Channeling)
        {
            if (skillChannelingTime[2] <= 0f)
            {
                skillState = SkillStateCultist.None;
                CallRPCEvent("ResetAnimation", "Response");
                CallRPCEvent("UpdateData", "Response", skillState, "OnlySkillState", 0, 0f, false);
                //��Ÿ�� ����
            }
        }
        else if (skillState == SkillStateCultist.Skill4Casting)
        {
            if (skillCastingTime[3] <= 0f)
            {
                skillState = SkillStateCultist.None;
                CallRPCEvent("ResetAnimation", "Response");
                CallRPCEvent("UpdateData", "Response", skillState, "OnlySkillState", 0, 0f, false);
                //��Ÿ�� ����
            }
        }
    }

    //�Է� �̺�Ʈ
    void OnSkill1()
    {
        if (photonView.IsMine)
            CallRPCEvent("SetSkill", "Request", 1);
    }

    void OnSkill2()
    {
        if (photonView.IsMine)
            CallRPCEvent("SetSkill", "Request", 2);
    }

    void OnSkill3()
    {
        if (photonView.IsMine)
            CallRPCEvent("SetSkill", "Request", 3);
    }

    void OnSkill4()
    {
        if (photonView.IsMine)
            CallRPCEvent("SetSkill", "Request", 4);
    }

    void OnMouseButton()
    {
        if (photonView.IsMine)
        {
            if (!isClicked[0])
                CallRPCEvent("ClickMouse", "Request", 0);
            else
                CallRPCEvent("CancelHolding", "Request");
            isClicked[0] = !isClicked[0];
        }
    }

    void OnMouseButton2()
    {
        if (photonView.IsMine)
        {
            if (!isClicked[1])
                CallRPCEvent("ClickMouse", "Request", 1);
            isClicked[1] = !isClicked[1];
        }
    }

    void OnButtonCancel()
    {
        if (photonView.IsMine)
            CallRPCEvent("CancelSkill", "Request", 0);
    }


    //��û �� ����
    void RequestRPCCall(object[] data)
    {
        photonView.RPC("CallRPCCultistToMasterClient", RpcTarget.MasterClient, data);
    }

    [PunRPC]
    public void CallRPCCultistToMasterClient(object[] data)
    {
        if ((string)data[0] == "SetSkill")
            SetSkill(data);
        else if ((string)data[0] == "ClickMouse")
            ClickMouse(data);
        else if ((string)data[0] == "CancelHolding")
            CancelHolding();
        else if ((string)data[0] == "CancelSkill")
            CancelSkill();
        else if ((string)data[0] == "SetOwnerNum")
            ResponseRPCCall(data);
        else if ((string)data[0] == "ProgressSkillLogic")
            ProgressSkillLogic(data);
    }

    void ResponseRPCCall(object[] data)
    {
        photonView.RPC("CallRPCCulTistToAll", RpcTarget.AllBuffered, data);
    }

    [PunRPC]
    public void CallRPCCulTistToAll(object[] data)
    {
        if ((string)data[0] == "UpdateData")
            UpdateData(data);
        if ((string)data[0] == "SetAnimation")
            SetAnimation(data);
        else if ((string)data[0] == "SetOwnerNum")
            SetOwnerNum(data);
        else if ((string)data[0] == "SetDagger")
            SetDagger(data);
        else if ((string)data[0] == "ResetAnimation")
            ResetAnimation();
        else if ((string)data[0] == "InstantiateObject")
            InstantiateObject(data);
        else if ((string)data[0] == "RunLungeEffect")
            RunLungeEffect();
        else if ((string)data[0] == "SetInvocationEffect")
            SetInvocationEffect(data);
        else if ((string)data[0] == "SetLungeCollider")
            SetLungeCollider(data);
        else if ((string)data[0] == "SetCoolDownTime")
            SetCoolDownTime(data);
        else if ((string)data[0] == "UseSkill")
            UseSkill(data);
    }

    //RPC ��û
    void CallRPCEvent(string command, string type, params object[] parameters)
    {
        object[] _sendData;
        if (parameters.Length >= 1)
        {
            _sendData = new object[parameters.Length + 1];
            for (int i = 0; i < parameters.Length; i++)
            {
                _sendData[i + 1] = parameters[i];
            }
        }
        else
            _sendData = new object[2];
        _sendData[0] = command;

        if(type == "Request")
            RequestRPCCall(_sendData);
        else if(type == "Response")
            ResponseRPCCall(_sendData);
    }

    //��û ó��

    void ProgressSkillLogic(object[] data)
    {
        int _index = (int)data[1];
        if (_index == 0)
        {
            Ray _tempRay = new Ray();
            _tempRay.origin = (Vector3)data[2];
            _tempRay.direction = (Vector3)data[3];

            RaycastHit[] _tempHits = Physics.RaycastAll(_tempRay, 10f);
            List<RaycastHit> _tempHitsList = _tempHits.ToList();

            _tempHitsList = _tempHitsList.OrderBy(hit => hit.distance).ToList();

            int _check = 0;

            for (int i = 0; i < _tempHitsList.Count; i++)
            {
                GameObject _tempObject = _tempHitsList[i].collider.gameObject;
                if (_tempObject.layer == 11 || _tempObject.tag == "Wall")
                    break;
                else if (_tempObject.transform.root.gameObject.GetComponent<Character>() != null)
                {
                    GameObject _rootObject = _tempObject.transform.root.gameObject;
                    Debug.Log(_rootObject + " " + _rootObject.tag);
                    if (gameObject == _rootObject)
                        continue;
                    if (_rootObject.tag != tag)
                        break;
                    else if (_rootObject.tag == tag)
                    {
                        _rootObject.GetComponent<PhotonView>().RPC("PlayerDamaged", RpcTarget.MasterClient,
                            playerName, -(int)(cultistData.skill1Damage), _tempObject.name, Vector3.zero, 0f);

                        _rootObject.GetComponent<BuffDebuffChecker>().SetNewBuffDebuff("TerribleTentacles", 1);

                        _check = 1;
                        break;
                    }
                }
            }

            if (_check == 0)
            {
                GetComponent<PhotonView>().RPC("PlayerDamaged", RpcTarget.MasterClient,
                            playerName, -(int)(cultistData.skill1Damage), "null", Vector3.zero, 0f);
                //üũ�κ�
                buffDebuffChecker.SetNewBuffDebuff("TerribleTentacles", 2);
            }


        }

    }

    void SetSkill(object[] data)
    {
        int _skillNum = (int)data[1];

        if (GetComponent<Cultist>() != null)
        {
            if (buffDebuffChecker.CheckBuffDebuff("TerribleTentacles", _skillNum - 1))//true�� ��ų ��� �Ұ�
            {
                buffDebuffChecker.UseTerribleTentacles(_skillNum - 1);
                return;
            }
        }
        

        if (skillCoolDownTime[_skillNum - 1] <= 0f)
        {
            {
                if (skillState == SkillStateCultist.Skill1Ready ||
                    skillState == SkillStateCultist.Skill2Ready ||
                    skillState == SkillStateCultist.Skill3Ready ||
                    skillState == SkillStateCultist.Skill4Ready ||
                    skillState == SkillStateCultist.None ||
                    (skillState == SkillStateCultist.Invocation && normalCastingTime[0] <= 0f))
                {
                    if (_skillNum == 1)
                        skillState = SkillStateCultist.Skill1Ready;
                    else if (_skillNum == 2)
                        skillState = SkillStateCultist.Skill2Ready;
                    else if (_skillNum == 3)
                        skillState = SkillStateCultist.Skill3Ready;
                    else if (_skillNum == 4)
                        skillState = SkillStateCultist.Skill4Ready;
                }
            }
        }
    }
    void ClickMouse(object[] data)
    {
        // 0�� ���� 1�� ������
        int mouseCode = (int)data[1];
        if (mouseCode == 0)
        {
            if (skillState == SkillStateCultist.None)
            {
                skillState = SkillStateCultist.Invocation;
                CallRPCEvent("SetInvocationEffect", "Response", true);
                CallRPCEvent("SetAnimation", "Response", "isInvocation", true);
                CallRPCEvent("UpdateData", "Response", skillState, "normalCastingTime", 0, invocationCastingTime, true);
            }
            else if (skillState == SkillStateCultist.Invocation)
            {
                if (normalCastingTime[0] <= 0f)
                {
                    skillState = SkillStateCultist.LungeHolding;
                    CallRPCEvent("SetAnimation","Response" ,"isLungeHolding", true);
                    CallRPCEvent("UpdateData", "Response", skillState, "normalCastingTime", 1, lungeHoldingTime, true);
                    CallRPCEvent("RunLungeEffect", "Response");
                }
            }
            else if (skillState == SkillStateCultist.Skill1Ready)
            {
                skillState = SkillStateCultist.Skill1Casting;
                CallRPCEvent("SetAnimation", "Response","isSkill1", true);
                CallRPCEvent("UpdateData", "Response", skillState, "skillCastingTime", 0, skill1CastingTime, true);
            }
            else if (skillState == SkillStateCultist.Skill2Ready)
            {
                skillState = SkillStateCultist.Skill2Casting;
                CallRPCEvent("SetAnimation", "Response","isSkill2", true);
                CallRPCEvent("UpdateData", "Response", skillState, "skillCastingTime", 1, skill2CastingTime, true);
            }
            else if (skillState == SkillStateCultist.Skill3Ready)
            {
                skillState = SkillStateCultist.Skill3Casting;
                CallRPCEvent("SetAnimation","Response" ,"isSkill3", true);
                CallRPCEvent("UpdateData", "Response", skillState, "skillCastingTime", 2, skill3CastingTime, true);
            }
            else if (skillState == SkillStateCultist.Skill4Ready)
            {
                skillState = SkillStateCultist.Skill4Casting;
                CallRPCEvent("SetAnimation", "Response", "isSkill4", true);
                CallRPCEvent("UpdateData", "Response", skillState, "skillCastingTime", 3, skill4CastingTime, true);
            }
        }
        else
        {
            if (skillState == SkillStateCultist.Invocation)
            {
                if (normalCastingTime[0] <= 0f)
                {
                    skillState = SkillStateCultist.Throw;
                    CallRPCEvent("SetAnimation", "Response","isThrow", true);
                    CallRPCEvent("UpdateData", "Response", skillState, "normalCastingTime", 3, throwCastingTime, true);
                }
            }
        }
    }

    void CancelHolding()
    {
        //Ȧ�� ĵ��
        if (skillState == SkillStateCultist.LungeHolding)
        {
            skillState = SkillStateCultist.LungeAttack;
            CallRPCEvent("SetAnimation", "Response","isLungeHolding", false);
            CallRPCEvent("SetAnimation", "Response", "isLungeAttack", true);
            CallRPCEvent("SetLungeCollider", "Response", true);
            CallRPCEvent("UpdateData", "Response", skillState, "normalCastingTime", 1, 0f, true);
            CallRPCEvent("UpdateData", "Response", skillState, "normalCastingTime", 2, lungeAttackTime, true);
        }
    }

    void CancelSkill()
    {
        if (skillState == SkillStateCultist.Skill1Ready || skillState == SkillStateCultist.Skill2Ready
            || skillState == SkillStateCultist.Skill3Ready || skillState == SkillStateCultist.Skill4Ready
            || (skillState == SkillStateCultist.Invocation && normalCastingTime[0] <= 0f))
        {
            skillState = SkillStateCultist.None;
            CallRPCEvent("UpdateData", "Response", skillState, "OnlySkillState", 0, 0f, true);
        }
    }

    //���� ó��
    void UpdateData(object[] data)
    {
        skillState = (SkillStateCultist)data[1];

        string _timeType = (string)data[2];
        int _index = (int)data[3];
        float _newTime = (float)data[4];
        bool _onceChecker = (bool)data[5];

        if (_timeType == "normalCastingTime")
        {
            normalCastingTime[_index] = _newTime;
            normalCastingCheck[_index] = _onceChecker;
        }
        else if (_timeType == "skillCastingTime")
        {
            skillCastingTime[_index] = _newTime;
            skillCastingCheck[_index] = _onceChecker;
        }
        else if (_timeType == "skillChannelingTime")
        {
            skillChannelingTime[_index] = _newTime;
            skillCastingCheck[_index] = false;
            skillChannelingCheck[_index] = _onceChecker;
        }
        else if(_timeType == "OnlySkillState")
        {
            for (int i = 0; i < normalCastingCheck.Length; i++)
                normalCastingCheck[i] = false;
            for (int i = 0; i < skillCastingCheck.Length; i++)
                skillCastingCheck[i] = false;
            for (int i = 0; i < skillChannelingCheck.Length; i++)
                skillChannelingCheck[i] = false;
        }
    }

    void SetCoolDownTime(object[] data)
    {
        int _index = (int)data[1];

        if (_index == 0)
            skillCoolDownTime[_index] = skill1CoolDownTime;
        else if(_index == 1)
            skillCoolDownTime[_index] = skill2CoolDownTime;
        else if (_index == 2)
            skillCoolDownTime[_index] = skill3CoolDownTime;
        else if (_index == 3)
            skillCoolDownTime[_index] = skill4CoolDownTime;

    }

    void UseSkill(object[] data)
    {
        if (photonView.IsMine)
        {
            int _index = (int)data[1];
            if (_index == 0)//��ų 1 ���
                UseSkill1();
        }
    }

    void UseSkill1()
    {
        Ray _tempRay = camera.GetComponent<Camera>().ScreenPointToRay(aim.transform.position);
        CallRPCEvent("ProgressSkillLogic", "Request", 0, _tempRay.origin, _tempRay.direction);
    }

    void SetAnimation(object[] data)
    {
        if (photonView.IsMine)
        {
            animator.SetBool((string)data[1], (bool)data[2]);
            overlayAnimator.SetBool((string)data[1], (bool)data[2]);
        }
    }

    void SetOwnerNum(object[] data)
    {
        ownerNum = (int)data[1];
    }

    void SetDagger(object[] data)
    {
        bool _isActive = (bool)data[1];
        if (photonView.IsMine)
            overlayDagger.SetActive(_isActive);
        dagger.SetActive(_isActive);
    }

    void ResetAnimation()
    {
        if (photonView.IsMine)
        {
            animator.SetBool("isSkill1", false);
            animator.SetBool("isSkill2", false);
            animator.SetBool("isSkill3", false);
            animator.SetBool("isSkill4", false);
            animator.SetBool("isLungeHolding", false);
            animator.SetBool("isLungeAttack", false);
            animator.SetBool("isThrow", false);
            animator.SetBool("isInvocation", false);

            overlayAnimator.SetBool("isSkill1", false);
            overlayAnimator.SetBool("isSkill2", false);
            overlayAnimator.SetBool("isSkill3", false);
            overlayAnimator.SetBool("isSkill4", false);
            overlayAnimator.SetBool("isLungeHolding", false);
            overlayAnimator.SetBool("isLungeAttack", false);
            overlayAnimator.SetBool("isThrow", false);
            overlayAnimator.SetBool("isInvocation", false);
        }
    }

    void InstantiateObject(object[] data)
    {
        if (photonView.IsMine)
        {
            if ((string)data[1] == "Dagger")
            {
                Ray _tempRay = camera.GetComponent<Camera>().ScreenPointToRay(aim.transform.position);
                Quaternion _tempQ = Quaternion.LookRotation(_tempRay.direction);

                object[] _data = new object[3];
                _data[0] = name;
                _data[1] = tag;
                _data[2] = "Dagger";

                PhotonNetwork.Instantiate("ChanYoung/Prefabs/Cultist/Dagger", _tempRay.origin + _tempRay.direction * 0.5f, _tempQ, data: _data);
            }
        }
    }


    void RunLungeEffect()
    {
        lungeEffect.transform.GetChild(0).GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        lungeEffect.transform.GetChild(0).GetComponent<ParticleSystem>().startLifetime = lungeHoldingTime + lungeAttackTime;
        lungeEffect.transform.GetChild(0).GetComponent<ParticleSystem>().Play(false);
    }

    void SetLungeCollider(object[] data)
    {
        GetComponent<CultistLungeAttack>().isColliderOn = (bool)data[1];
    }

    void SetInvocationEffect(object[] data)
    {
        if (!photonView.IsMine)
            invocationEffect.transform.position = invocationEffectOrb.transform.position;

        for (int i = 0; i < 3; i++)
        {
            if ((bool)data[1])
            {
                if (photonView.IsMine)
                    overlayInvocationEffect.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
                else
                    invocationEffect.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
            }
            else
            {
                if(photonView.IsMine)
                    overlayInvocationEffect.transform.GetChild(i).GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                else
                    invocationEffect.transform.GetChild(i).GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        if ((bool)data[1])
        {
            if (photonView.IsMine)
                overlayInvocationEffectOrb.transform.GetComponent<ParticleSystem>().Play();
            else
                invocationEffectOrb.transform.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            if (photonView.IsMine)
                overlayInvocationEffectOrb.transform.GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            else
                invocationEffectOrb.transform.GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    //�ִϸ��̼�
    void CheckAnimationSpeed()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Invocation"))
            SetAnimationSpeed("InvocationSpeed", invocationCastingTime);
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("LungeHolding"))
            SetAnimationSpeed("LungeHoldingSpeed", lungeHoldingTime);
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("LungeAttack"))
            SetAnimationSpeed("LungeAttackSpeed", lungeAttackTime);
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Throw"))
            SetAnimationSpeed("ThrowSpeed", throwCastingTime);
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Skill1Casting"))
            SetAnimationSpeed("Skill1CastingSpeed", skill1CastingTime);
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Skill1"))
            SetAnimationSpeed("Skill1Speed", skill1ChannelingTime);
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Skill2"))
            SetAnimationSpeed("Skill2CastingSpeed", skill2CastingTime);
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Skill3Casting"))
            SetAnimationSpeed("Skill3CastingSpeed", skill3CastingTime);
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Skill3"))
            SetAnimationSpeed("Skill3Speed", skill3ChannelingTime);
        else if (animator.GetCurrentAnimatorStateInfo(4).IsName("Skill4"))
            SetAnimationSpeedExtra("Skill4CastingSpeed", skill4CastingTime);

    }

    void SetAnimationSpeed(string state, float animationTime)
    {
        float _beforeSpeed = animator.GetCurrentAnimatorStateInfo(0).speed;
        float _animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        float _normalizedSpeed = _animationLength / animationTime;
        if (_normalizedSpeed != 1 && _normalizedSpeed != _beforeSpeed)
        {
            animator.SetFloat(state, _normalizedSpeed);
            overlayAnimator.SetFloat(state, _normalizedSpeed);
        }
    }

    void SetAnimationSpeedExtra(string state, float animationTime)
    {
        float _beforeSpeed = animator.GetCurrentAnimatorStateInfo(4).speed;
        float _animationLength = animator.GetCurrentAnimatorStateInfo(4).length;
        float _normalizedSpeed = _animationLength / animationTime;
        if (_normalizedSpeed != 1 && _normalizedSpeed != _beforeSpeed)
        {
            animator.SetFloat(state, _normalizedSpeed);
            overlayAnimator.SetFloat(state, _normalizedSpeed);
        }
    }

    void CheckAnimatorExtra()
    {
        if (skillState == SkillStateCultist.Skill4Casting)
            skill4Weight = Mathf.Lerp(skill4Weight, 1f, Time.deltaTime * 8);
        else
            skill4Weight = Mathf.Lerp(skill4Weight, 0f, Time.deltaTime * 8);
        animator.SetLayerWeight(4, skill4Weight);

        //���� Ư��
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("LungeAttack"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
                overlayDagger.SetActive(false);
        }
    }

    //�յ��� �ִϸ��̼ǿ� ���� IK����
    protected override void OnAnimatorIK()
    {
        base.OnAnimatorIK();
        if (overlayAnimator.GetCurrentAnimatorStateInfo(1).IsName("Idle"))
            LerpWeight(0.5f);
        else if (overlayAnimator.GetCurrentAnimatorStateInfo(1).IsName("Invocation"))
            LerpWeight(0f);
        else if (overlayAnimator.GetCurrentAnimatorStateInfo(1).IsName("LungeHolding"))
            LerpWeight(0.2f);
        else if (overlayAnimator.GetCurrentAnimatorStateInfo(1).IsName("LungeAttack"))
            LerpWeight(0.25f);
        else if (overlayAnimator.GetCurrentAnimatorStateInfo(1).IsName("Throw"))
            LerpWeight(0f);
        else if (overlayAnimator.GetCurrentAnimatorStateInfo(1).IsName("Skill1Casting"))
            LerpWeight(0f);
        else if (overlayAnimator.GetCurrentAnimatorStateInfo(1).IsName("Skill1"))
            LerpWeight(0.7f);
        else if (overlayAnimator.GetCurrentAnimatorStateInfo(1).IsName("Skill2"))
            LerpWeight(0.2f);
        else if (overlayAnimator.GetCurrentAnimatorStateInfo(1).IsName("Skill3Casting"))
            LerpWeight(0.2f);
        else if (overlayAnimator.GetCurrentAnimatorStateInfo(1).IsName("Skill3"))
            LerpWeight(0.2f);
        else if (overlayAnimator.GetCurrentAnimatorStateInfo(1).IsName("Skill4"))
            LerpWeight(0f);

        overlayAnimator.SetIKPosition(AvatarIKGoal.LeftHand, overlaySightLeft.transform.position);
        overlayAnimator.SetIKPosition(AvatarIKGoal.RightHand, overlaySightRight.transform.position);
        overlayAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, overlayLeftHandWeight);
        overlayAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, overlayRightHandWeight);
    }

    void LerpWeight(float weight)
    {
        overlayLeftHandWeight = Mathf.Lerp(overlayLeftHandWeight, weight, Time.deltaTime * 8f);
        overlayRightHandWeight = Mathf.Lerp(overlayRightHandWeight, weight, Time.deltaTime * 8f);
    }
}
