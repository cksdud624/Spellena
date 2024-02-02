using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using DefineDatas;

public class AloyBT : BehaviorTree.Tree
{
    // ĳ���� ��ų ����
    public List<AbilityMaker.SkillName> characterSkills;
    // ĳ���� ��� ����(ex.�����̵�)
    public List<AbilityMaker.FunctionName> characterFunctions;

    [HideInInspector]
    public Transform lookTransform;

    private Transform aimingTrasform;
    private List<AbilityNode> skills = new List<AbilityNode>();
    private Dictionary<AbilityMaker.FunctionName, AbilityNode> functions
        = new Dictionary<AbilityMaker.FunctionName, AbilityNode>();
    private AbilityMaker abilityMaker;
    private Animator animator;

    void InitData()
    {
        Random.InitState(DefineNumber.RandonInitNum);
        animator = GetComponent<Animator>();
        abilityMaker = GetComponent<AbilityMaker>();
        aimingTrasform = abilityMaker.abilityObjectTransforms
            [(int)AbilityMaker.AbilityObjectName.AimingTransform];
        for (int i = 0; i < characterSkills.Count; i++)
            skills.Add(abilityMaker.MakeSkill(characterSkills[i]));      
        for (int i = 0; i < characterFunctions.Count; i++)
            functions[characterFunctions[i]] = abilityMaker.MakeFunction(characterFunctions[i]);
    }
    protected override Node SetupTree()
    {
        InitData();
        if (NullCheck()) return null;
        Node root = new Selector(this, NodeName.Selector ,new List<Node>
        {
            new EnemyDetector(this, EnemyDetector.EnemyDetectType.CheckEnemyInSight, 
                new Parallel(this, NodeName.Parallel, MakeSkillNode()),abilityMaker),
            functions[AbilityMaker.FunctionName.GotoOccupationArea]
        }
        );
        root.SetDataToRoot(DataContext.NodeStatus, root);
        StartCoroutine(CoolTimer());
        return root;
    }
    bool NullCheck()
    {
        if (animator == null)
        {
            Debug.LogError("animator�� �Ҵ���� �ʾҽ��ϴ�");
            return true;
        }
        if (abilityMaker == null)
        {
            Debug.LogError("abilityMaker�� �Ҵ���� �ʾҽ��ϴ�");
            return true;
        }
        if (aimingTrasform == null)
        {
            Debug.LogError("aimingTrasform�� �Ҵ���� �ʾҽ��ϴ�");
            return true;
        }
        return false;
    }

    List<Node> MakeSkillNode()
    {
        List<Node> temp = new List<Node>();
        for (int i = 0; i < skills.Count; i++)
            temp.Add(skills[i]);
        return temp;
    }

    protected override void Update()
    {
        base.Update();
        ShowNodeState();
    }

    IEnumerator CoolTimer()
    {
        while (true)
        {
            for (int i = 0; i < skills.Count; i++)
            {
                if(skills[i].coolTimer !=null)
                    skills[i].coolTimer.UpdateCoolTime(Time.deltaTime);
            }

            yield return null;
        }
    }

    void OnAnimatorIK()
    {
        SetLookAtObj();
    }

    void SetLookAtObj()
    {
        if (animator == null) return;
        animator.SetLookAtWeight(PlayerLookAtWeight.weight, PlayerLookAtWeight.bodyWeight);
        if (lookTransform == null) return;          
        animator.SetLookAtPosition(lookTransform.position);
        aimingTrasform.LookAt(lookTransform.position);
    }

    // ���� � Node�� �ִ��� Ȯ��
    void ShowNodeState()
    {
        Debug.Log("<color=orange>" + root.GetData(DataContext.NodeStatus).nodeName + "</color>");
    }
}
