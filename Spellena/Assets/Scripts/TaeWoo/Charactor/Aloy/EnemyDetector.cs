using UnityEngine;
using System;
using BehaviorTree;
using DefineDatas;
public class EnemyDetector: Condition
{
    public enum EnemyDetectType
    {
        CheckEnemyInSight
    }

    private Transform playerTransform;
    private Animator animator;
    private Animator bowAnimator;
    private float viewAngle;
    private float viewRadius;
    private LayerMask targetMask;
    private Ray ray;
    private RaycastHit hit;
    private bool isNull;

    public EnemyDetector(BehaviorTree.Tree tree, EnemyDetectType detectType, Node _TNode, AbilityMaker abilityMaker)
        : base(tree,NodeName.Condition_1, null, _TNode)
    {
        condition += DefineDetectType(detectType);
        playerTransform = abilityMaker.abilityObjectTransforms[(int)AbilityMaker.AbilityObjectName.CharacterTransform];
        bowAnimator = abilityMaker.abilityObjectTransforms[(int)AbilityMaker.AbilityObjectName.BowAniObject].GetComponent<Animator>();
        animator = playerTransform.GetComponent<Animator>();
        viewAngle = abilityMaker.data.sightAngle;
        viewRadius = abilityMaker.data.sightDistance;
        targetMask = LayerMask.GetMask(LayerMaskName.Player);
        ray = new Ray();
        isNull = NullCheck();
    }

    Func<bool> DefineDetectType(EnemyDetectType detectType)
    {
        switch(detectType)
        {
            case EnemyDetectType.CheckEnemyInSight:
                return CheckEnemyInSight;
        }

        return null;
    }

    public bool CheckEnemyInSight()
    {
        if (isNull) return false;
        Vector3 SightPos = playerTransform.position + Vector3.up * 1.5f;
        float lookingAngle = playerTransform.eulerAngles.y;  //ĳ���Ͱ� �ٶ󺸴� ������ ����
        Vector3 rightDir = AngleToDir(playerTransform.eulerAngles.y + viewAngle * 0.5f);
        Vector3 leftDir = AngleToDir(playerTransform.eulerAngles.y - viewAngle * 0.5f);
        Vector3 lookDir = AngleToDir(lookingAngle);

        Debug.DrawRay(SightPos, rightDir * viewRadius, Color.blue);
        Debug.DrawRay(SightPos, leftDir * viewRadius, Color.blue);
        Debug.DrawRay(SightPos, lookDir * viewRadius, Color.cyan);

        Collider[] targets = Physics.OverlapSphere(SightPos, viewRadius, targetMask);

        if(targets.Length > 0)
        {
            foreach (Collider player in targets)
            {
                Vector3 targetPos = player.transform.position;
                Vector3 targetDir = (targetPos - SightPos).normalized;
                float targetAngle = Mathf.Acos(Vector3.Dot(lookDir, targetDir)) * Mathf.Rad2Deg;
                ray.origin = SightPos; ray.direction = targetDir;

                if (targetAngle <= viewAngle * 0.5f && Physics.Raycast(ray, out hit, viewRadius))
                {
                    if (hit.collider.gameObject.layer != LayerMask.NameToLayer(LayerMaskName.Wall))
                    {
                        // �±װ� �ٸ� �� ����
                        Debug.DrawLine(SightPos, targetPos, Color.red);
                        if (((AloyBT)tree).lookTransform == null)
                        {
                            ((AloyBT)tree).lookTransform = player.transform;
                            animator.SetBool(PlayerAniState.CheckEnemy, true);
                            bowAnimator.SetBool(PlayerAniState.Draw, true);
                        }
                        return true;
                    }
                }

            }
        }
        ((AloyBT)tree).lookTransform = null;
        animator.SetBool(PlayerAniState.CheckEnemy, false);
        bowAnimator.SetBool(PlayerAniState.Draw, false);
        return false;
    }
    private bool NullCheck()
    {
        if(condition == null)
        {
            Debug.LogError("condition�� �Ҵ���� �ʾҽ��ϴ�");
            return true;
        }
        if(playerTransform == null)
        {
            Debug.LogError("playerTransform �Ҵ���� �ʾҽ��ϴ�");
            return true;
        }
        if (bowAnimator == null)
        {
            Debug.LogError("bowAnimator�� �Ҵ���� �ʾҽ��ϴ�");
            return true;
        }
        if (animator == null)
        {
            Debug.LogError("Animator�� �Ҵ���� �ʾҽ��ϴ�");
            return true;
        }
        return false;
    }
    private Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }
}
