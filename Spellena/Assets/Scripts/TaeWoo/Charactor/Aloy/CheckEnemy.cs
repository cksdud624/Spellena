using UnityEngine;
using System;
public class CheckEnemy
{
    private Transform playerTransform;

    private Animator animator;
    private float viewAngle;
    private float viewRadius;
    private LayerMask targetMask;
    private LayerMask obstacleMask;

    private Transform enemy;

    private Animator bowAnimator;

    [HideInInspector]
    public Transform Enemy { get { return enemy; } }
    
    public CheckEnemy()
    {
        viewAngle = 0;
        viewRadius = 0;
        targetMask = 0;
    }

    public CheckEnemy(Transform _playerTransform, GameObject _bowAniObj, float _viewAngle, float _viewRadius,
        LayerMask _targetMask, LayerMask _obstacleMask)
    {
        playerTransform = _playerTransform;
        bowAnimator = _bowAniObj.GetComponent<Animator>();
        if (bowAnimator == null) Debug.LogError("bowAnimator�� �Ҵ���� �ʾҽ��ϴ�");
        animator = playerTransform.GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator�� �Ҵ���� �ʾҽ��ϴ�");
        viewAngle = _viewAngle;
        viewRadius = _viewRadius;
        targetMask = 1 << _targetMask;
        obstacleMask = 1 << _obstacleMask;
    }

    public bool EnemyInRange()
    {
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

                if (targetAngle <= viewAngle * 0.5f &&
                    !Physics.Raycast(SightPos, targetDir, viewRadius, obstacleMask))
                {
                    // �±װ� �ٸ� �� ����
                    Debug.DrawLine(SightPos, targetPos, Color.red);
                    enemy = player.transform;
                    CheckEnemyAni(true);
                    return true;
                }
            }
        }

        enemy = null;
        CheckEnemyAni(false);
        return false;
    }

    private Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }

    private void CheckEnemyAni(bool setBool)
    {
        animator.SetBool("CheckEnemy", setBool);
        bowAnimator.SetBool("Draw", setBool);
    }
}
